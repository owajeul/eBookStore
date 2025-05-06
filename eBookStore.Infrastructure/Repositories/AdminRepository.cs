using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eBookStore.Application.Common.Utilily;
using eBookStore.Application.Interfaces;
using eBookStore.Domain.Entities;
using eBookStore.Infrastructure.Data;
using eBookStore.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace eBookStore.Infrastructure.Repositories
{
    public class AdminRepository : IAdminRepository
    {
        private readonly AppDbContext _dbContext;

        public AdminRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<int> GetTotalUsersCountAsync()
        {
            return await _dbContext.Users.CountAsync();
        }

        public async Task<int> GetTotalOrdersCountAsync()
        {
            return await _dbContext.Orders.CountAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _dbContext.Orders
                .Where(o => o.PaymentStatus == AppConstant.PaymentStatusPaid)
                .SumAsync(o => o.TotalPrice);
        }

        public async Task<int> GetTotalBooksCountAsync()
        {
            return await _dbContext.Books.CountAsync();
        }

        public async Task<List<(Book book, int copiesSold, decimal revenue)>> GetTopSellingBooksAsync(int count)
        {
            var bookSalesData = await GetBookSalesDataAsync(count);

            var bookIds = bookSalesData.Select(b => b.BookId).ToList();

            var books = await GetBooksByIdsAsync(bookIds);

            var result = books.Join(
                bookSalesData,
                book => book.Id,
                bookSalesData => bookSalesData.BookId,
                (book, sale) => (book, sale.CopiesSold, sale.Revenue)
            ).ToList();
            return result;
        }

        private async Task<List<BookSales>> GetBookSalesDataAsync(int count)
        {
            return await _dbContext.Orders
                  .Where(o => o.Status == AppConstant.StatusDelivered)
                  .SelectMany(o => o.OrderItems)
                  .GroupBy(oi => oi.BookId)
                  .Select(g => new BookSales
                  {
                      BookId = g.Key,
                      CopiesSold = g.Sum(oi => oi.Quantity),
                      Revenue = g.Sum(oi => oi.Quantity * oi.UnitPrice)
                  })
                  .OrderByDescending(b => b.CopiesSold)
                  .Take(count)
                  .ToListAsync();
        }

        private async Task<List<Book>> GetBooksByIdsAsync(List<int> bookIds)
        {
            return await _dbContext.Books
                .Where(b => bookIds.Contains(b.Id))
                .ToListAsync();
        }

        public async Task<List<Book>> GetLowStockBooksAsync(int threshold, int? maxResults = null)
        {
            var query = _dbContext.Books
                .Where(b => b.Stock <= threshold)
                .OrderBy(b => b.Stock);

            if (maxResults.HasValue)
            {
                query.Take(maxResults.Value);
            }
            return await query.ToListAsync();
        }
    }
}