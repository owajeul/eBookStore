using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using eBookStore.Application.Common.Exceptions;
using eBookStore.Application.Common.Utilily;
using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;
using eBookStore.Domain.Entities;

namespace eBookStore.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IBookService _bookService;
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;

    public OrderService(IOrderRepository orderRepository,
        ICartRepository cartRepository, 
        IBookService bookService,
        IMapper mapper,
        IUnitOfWork unitOfWork)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _bookService = bookService;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task PlaceOrderAsync(OrderDto orderDto)
    {
        try
        {
            ValidateOrderDto(orderDto);
            await _unitOfWork.BeginTransactionAsync();
            await CheckStockAsync(orderDto);
            await CreateOrderAsync(orderDto);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (Exception ex) when (!(ex is OrderServiceException))
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw new OrderServiceException($"Failed to place order for user {orderDto?.UserId}", ex);
        }
    }

    public async Task CheckStockAsync(OrderDto orderDto)
    {
        foreach (var item in orderDto.OrderItems)
        {
            var book = await _bookService.GetBookAsync(item.BookId);
            if(book.Stock < item.Quantity)
                throw new OrderServiceException($"Not enough stock for book id: {book.Id} title:{book.Title}");
        }
    }

    public async Task<List<OrderDto>> GetUserOrdersAsync(string userId)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            return await FetchUserOrdersAsync(userId);
        }
        catch (Exception ex) when (!(ex is OrderServiceException))
        {
            throw new OrderServiceException($"Failed to retrieve orders for user {userId}", ex);
        }
    }

    public async Task<List<OrderDto>> GetAllOrdersAsync()
    {
        try
        {
            return await FetchAllOrdersAsync();
        }
        catch (Exception ex)
        {
            throw new OrderServiceException("Failed to retrieve all orders", ex);
        }
    }

    public async Task<OrderDto> GetOrderById(int id)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("Order ID must be greater than zero", nameof(id));

            return await FetchOrderByIdAsync(id);
        }
        catch (Exception ex) when (!(ex is OrderServiceException))
        {
            throw new OrderServiceException($"Failed to retrieve order with ID {id}", ex);
        }
    }

    public async Task ChangeOrderStatus(int orderId, string orderStatus)
    {
        try
        {
            if (orderId <= 0)
                throw new ArgumentException("Order ID must be greater than zero", nameof(orderId));

            if (string.IsNullOrEmpty(orderStatus))
                throw new ArgumentException("Order status cannot be empty", nameof(orderStatus));

            ValidateOrderStatus(orderStatus);
            await UpdateOrderStatusAsync(orderId, orderStatus);
        }
        catch (Exception ex) when (!(ex is OrderServiceException))
        {
            throw new OrderServiceException($"Failed to change status for order {orderId} to {orderStatus}", ex);
        }
    }

    private void ValidateOrderStatus(string status)
    {
        if ( AppConstant.ValidStatuses.Contains(status))
            throw new ArgumentException($"Invalid order status: {status}. Valid statuses are: {string.Join(", ", AppConstant.ValidStatuses)}", nameof(status));
    }

    private void ValidateOrderDto(OrderDto orderDto)
    {
        if (orderDto == null)
            throw new ArgumentNullException(nameof(orderDto));

        if (string.IsNullOrEmpty(orderDto.UserId))
            throw new ArgumentException("User ID cannot be empty", nameof(orderDto.UserId));

        if (string.IsNullOrEmpty(orderDto.ShippingAddress))
            throw new ArgumentException("Shipping address cannot be empty", nameof(orderDto.ShippingAddress));

        if (string.IsNullOrEmpty(orderDto.PhoneNumber))
            throw new ArgumentException("Phone number cannot be empty", nameof(orderDto.PhoneNumber));

        if (orderDto.OrderItems == null || !orderDto.OrderItems.Any())
            throw new ArgumentException("Order must contain at least one item", nameof(orderDto.OrderItems));

        foreach (var item in orderDto.OrderItems)
        {
            if (item.BookId <= 0)
                throw new ArgumentException($"Invalid book ID: {item.BookId}", nameof(orderDto.OrderItems));

            if (item.Quantity <= 0)
                throw new ArgumentException($"Invalid quantity for book ID {item.BookId}: {item.Quantity}", nameof(orderDto.OrderItems));

            if (item.UnitPrice <= 0)
                throw new ArgumentException($"Invalid unit price for book ID {item.BookId}: {item.UnitPrice}", nameof(orderDto.OrderItems));
        }
    }

    private async Task CreateOrderAsync(OrderDto orderDto)
    {
        var order = new Order
        {
            UserId = orderDto.UserId,
            ShippingAddress = orderDto.ShippingAddress,
            PhoneNumber = orderDto.PhoneNumber,
            Status = AppConstant.StatusPending,
            OrderDate = DateTime.UtcNow,
            OrderItems = orderDto.OrderItems.Select(ci => new OrderItem
            {
                BookId = ci.BookId,
                Quantity = ci.Quantity,
                UnitPrice = ci.UnitPrice
            }).ToList(),
            TotalPrice = orderDto.OrderItems.Sum(ci => ci.UnitPrice * ci.Quantity)
        };

        await _orderRepository.Add(order);

        await _cartRepository.ClearCartAsync(orderDto.UserId);

        await _orderRepository.Save();
    }

    private async Task<List<OrderDto>> FetchUserOrdersAsync(string userId)
    {
        var orders = await _orderRepository.GetOrdersByUserIdAsync(userId);

        if (orders == null)
            return new List<OrderDto>();

        return _mapper.Map<List<OrderDto>>(orders);
    }

    private async Task<List<OrderDto>> FetchAllOrdersAsync()
    {
        var orders = await _orderRepository.GetAllOrdersAsync();

        if (orders == null)
            return new List<OrderDto>();

        return _mapper.Map<List<OrderDto>>(orders);
    }

    private async Task<OrderDto> FetchOrderByIdAsync(int id)
    {
        var order = await _orderRepository.Get(o => o.Id == id);

        if (order == null)
            throw new OrderNotFoundException($"Order with ID {id} not found");

        return _mapper.Map<OrderDto>(order);
    }

    private async Task UpdateOrderStatusAsync(int orderId, string orderStatus)
    {
        var order = await _orderRepository.Get(o => o.Id == orderId);

        if (order == null)
            throw new OrderNotFoundException($"Order with ID {orderId} not found");

        order.Status = orderStatus;
        _orderRepository.Update(order);
        await _orderRepository.Save();
    }
}
