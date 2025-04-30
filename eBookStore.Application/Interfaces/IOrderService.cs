using eBookStore.Application.DTOs;
using eBookStore.Domain.Entities;

namespace eBookStore.Application.Interfaces;

public interface IOrderService
{
    Task<List<OrderDto>> GetUserOrdersAsync(string userId);
    Task PlaceOrderAsync(OrderDto orderDto, string userEmail);
    Task<List<OrderDto>> GetAllOrdersAsync();
    Task<OrderDto> GetOrderById(int id);
    Task ChangeOrderStatus(int orderId, string orderStatus);
}
