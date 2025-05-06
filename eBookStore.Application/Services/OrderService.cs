using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using eBookStore.Application.Common.Exceptions;
using eBookStore.Application.Common.Utilily;
using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;
using eBookStore.Application.Validators;
using eBookStore.Domain.Entities;
using eBookStore.Infrastructure.Services;

namespace eBookStore.Application.Services;

public class OrderService : IOrderService
{
    private readonly IMapper _mapper;
    private readonly IOrderNotificationService _orderNotificationService;
    private readonly IUnitOfWork _unitOfWork;


    public OrderService(
        IMapper mapper,
        IOrderNotificationService orderNotificationService,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _orderNotificationService = orderNotificationService;
        _unitOfWork = unitOfWork;
    }

    public async Task<int> PlaceOrderAsync(OrderDto orderDto, string userEmail)
    {
        var orderId = 0;
        try
        {
            await _unitOfWork.BeginTransactionAsync();
            await CheckStockAsync(orderDto);
            orderId = await CreateOrderAsync(orderDto);
            await UpdateStockAfterOrderAsync(orderDto);
            await _unitOfWork.Cart.ClearCartAsync(orderDto.UserId);
            await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitTransactionAsync();
        }
        catch (BookOutOfStockException)
        {
            throw;
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw new OrderServiceException($"Failed to place order for user {orderDto?.UserId}", ex);
        }
        var order = await _unitOfWork.Order.GetOrderWithAddressAsync(orderId);
        await _orderNotificationService.SendOrderConfirmationEmailAsync(userEmail, _mapper.Map<OrderDto>(order));
        return orderId;
    }
    private async Task CheckStockAsync(OrderDto orderDto)
    {
        foreach (var item in orderDto.OrderItems)
        {
            var book = await _unitOfWork.Book.Get(b => b.Id == item.BookId);
            if (book.Stock < item.Quantity)
                throw new BookOutOfStockException($"Not enough stock for book id: {book.Id} title:{book.Title}");
        }
    }
    private async Task<int> CreateOrderAsync(OrderDto orderDto)
    {
        _unitOfWork.DetachAllEntities();
        var order = _mapper.Map<Order>(orderDto);
        var savedOrderId = await _unitOfWork.Order.AddOrderAsync(order);
        return savedOrderId;
    }
    private async Task UpdateStockAfterOrderAsync(OrderDto orderDto)
    {
        foreach (var item in orderDto.OrderItems)
        {
            var book = await _unitOfWork.Book.Get(b => b.Id == item.BookId);
            if (book == null)
            {
                throw new BookNotFoundException($"Book with ID {item.BookId} not found.");
            }
            book.Stock -= item.Quantity;
            _unitOfWork.Book.Update(book);
        }
    }

    public async Task<List<OrderDto>> GetUserOrdersAsync(string userId)
    {
        try
        {
            InputValidator.ValidateUserId(userId);
            return await FetchUserOrdersAsync(userId);
        }
        catch (Exception ex) when (!(ex is OrderServiceException))
        {
            throw new OrderServiceException($"Failed to retrieve orders for user {userId}", ex);
        }
    }
    private async Task<List<OrderDto>> FetchUserOrdersAsync(string userId)
    {
        var orders = await _unitOfWork.Order.GetOrdersByUserIdAsync(userId);
        if (orders == null)
            return new List<OrderDto>();
        return _mapper.Map<List<OrderDto>>(orders);
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
    private async Task<List<OrderDto>> FetchAllOrdersAsync()
    {
        var orders = await _unitOfWork.Order.GetAllOrdersAsync();
        if (orders == null)
            return new List<OrderDto>();
        return _mapper.Map<List<OrderDto>>(orders);
    }

    public async Task<OrderDto> GetOrderById(int id)
    {
        try
        {
            InputValidator.ValidateOrderId(id);
            return await FetchOrderByIdAsync(id);
        }
        catch (Exception ex) when (!(ex is OrderServiceException))
        {
            throw new OrderServiceException($"Failed to retrieve order with ID {id}", ex);
        }
    }
    private async Task<OrderDto> FetchOrderByIdAsync(int id)
    {
        var order = await _unitOfWork.Order.GetOrderWithAddressAsync(id);
        if (order == null)
            throw new OrderNotFoundException($"Order with ID {id} not found");
        return _mapper.Map<OrderDto>(order);
    }

    public async Task ChangeOrderStatus(int orderId, string orderStatus)
    {
        try
        {
            InputValidator.ValidateOrderId(orderId);
            InputValidator.ValidateOrderStatus(orderStatus);
            await UpdateOrderStatusAsync(orderId, orderStatus);
            await _unitOfWork.SaveAsync();
        }
        catch (Exception ex) when (!(ex is OrderServiceException))
        {
            throw new OrderServiceException($"Failed to change status for order {orderId} to {orderStatus}", ex);
        }
    }
    private async Task UpdateOrderStatusAsync(int orderId, string orderStatus)
    {
        var order = await _unitOfWork.Order.Get(o => o.Id == orderId);
        if (order == null)
            throw new OrderNotFoundException($"Order with ID {orderId} not found");

        order.Status = orderStatus;
        _unitOfWork.Order.Update(order);
        await _unitOfWork.SaveAsync();
    }

    public async Task ChangePaymentStatus(int orderId, string paymentStatus)
    {
        try
        {
            InputValidator.ValidateOrderId(orderId);
            InputValidator.ValidatePaymentStatus(paymentStatus);
            await UpdatePaymentStatusAsync(orderId, paymentStatus);
            await _unitOfWork.SaveAsync();
        }
        catch (Exception ex) when (!(ex is OrderServiceException))
        {
            throw new OrderServiceException($"Failed to change status for order {orderId} to {paymentStatus}", ex);
        }
    }

    private async Task UpdatePaymentStatusAsync(int orderId, string paymentStatus)
    {
        var order = await _unitOfWork.Order.Get(o => o.Id == orderId);
        if (order == null)
            throw new OrderNotFoundException($"Order with ID {orderId} not found");

        order.PaymentStatus = paymentStatus;
        _unitOfWork.Order.Update(order);
        await _unitOfWork.SaveAsync();
    }
}
