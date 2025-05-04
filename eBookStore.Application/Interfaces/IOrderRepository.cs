using eBookStore.Application.DTOs;
using eBookStore.Domain.Entities;

namespace eBookStore.Application.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<List<Order>> GetAllOrdersAsync();
    Task<List<Order>> GetOrdersByUserIdAsync(string userId);
    Task<Order?> GetOrderById(int id);
    Task<List<UserPurchaseReportDto>> GetUserPurchaseReportsAsync();
    Task<int> AddOrderAsync(Order order);
    Task<Order?> GetOrderWithAddressAsync(int orderId);
}