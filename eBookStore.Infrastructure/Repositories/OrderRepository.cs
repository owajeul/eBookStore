using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;
using eBookStore.Domain.Entities;
using eBookStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;

namespace eBookStore.Infrastructure.Repositories
{
    public class OrderRepository : Repository<Order>, IOrderRepository
    {
        private readonly AppDbContext _dbContext;

        public OrderRepository(AppDbContext dbContext) : base(dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            return await _dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .ToListAsync();
        }

        public async Task<List<Order>> GetOrdersByUserIdAsync(string userId)
        {
            return await _dbContext.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .ToListAsync();
        }
        public async Task<Order?> GetOrderById(int id)
        {
            return await _dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<UserPurchaseReportDto>> GetUserPurchaseReportsAsync()
        {
            var reports = await _dbContext.Orders
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .GroupBy(o => o.UserId)
                .Select(g => new UserPurchaseReportDto
                {
                    UserId = g.Key,
                    UserName = _dbContext.Users.Where(u => u.Id == g.Key).Select(u => u.Name).FirstOrDefault(),
                    PhoneNumber = _dbContext.Users.Where(u => u.Id == g.Key).Select(u => u.PhoneNumber).FirstOrDefault(),
                    MostPurchasedGenre = g.SelectMany(o => o.OrderItems)
                                          .GroupBy(oi => oi.Book.Genre)
                                          .OrderByDescending(gr => gr.Sum(oi => oi.Quantity))
                                          .Select(gr => gr.Key)
                                          .FirstOrDefault(),
                    TotalSpend = g.Sum(o => o.TotalPrice),
                    TotalBooksPurchased = g.SelectMany(o => o.OrderItems).Sum(oi => oi.Quantity)
                })
                .OrderByDescending(r => r.TotalSpend)
                .ToListAsync();

            return reports;
        }

        public async Task<int> AddOrderAsync(Order order)
        {
            await _dbContext.Orders.AddAsync(order);
            await _dbContext.SaveChangesAsync();
            return order.Id;
        }

        public async Task<Order?> GetOrderWithAddressAsync(int orderId)
        {
            return await _dbContext.Orders
                .Include(o => o.ShippingAddress)
                .Include(o => o.BillingAddress)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Book)
                .FirstOrDefaultAsync(o => o.Id == orderId);
        }

    }
}
