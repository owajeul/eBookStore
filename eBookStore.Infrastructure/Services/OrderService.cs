using eBookStore.Application.Common.Dto;
using eBookStore.Application.Common.Interfaces;
using eBookStore.Application.Common.Utilily;
using eBookStore.Domain.Entities;

namespace eBookStore.Application.Services;

public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;

    public OrderService(IOrderRepository orderRepository, ICartRepository cartRepository)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
    }

    public async Task<Order> PlaceOrderAsync(OrderDto orderDto)
    {
        var order = new Order
        {
            UserId = orderDto.UserId,
            ShippingAddress = orderDto.ShippingAddress,
            PhoneNumber = orderDto.PhoneNumber,
            Status = AppConstant.StatusPending,
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
        return order;
    }

    public async Task<List<Order>> GetUserOrdersAsync(string userId)
    {
        return await _orderRepository.GetOrdersByUserIdAsync(userId);
    }
    
}
