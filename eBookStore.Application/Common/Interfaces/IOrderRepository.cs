using eBookStore.Domain.Entities;

namespace eBookStore.Application.Common.Interfaces;

public interface IOrderRepository: IRepository<Order>
{
    Task<List<Order>> GetOrdersByUserIdAsync(string userId);
}