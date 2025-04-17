using eBookStore.Application.Common.Dto;
using eBookStore.Domain.Entities;

namespace eBookStore.Application.Common.Interfaces;

public interface IOrderService
{
    Task<List<Order>> GetUserOrdersAsync(string userId);
    Task<Order> PlaceOrderAsync(OrderDto orderDto);
}