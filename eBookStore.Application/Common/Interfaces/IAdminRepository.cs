using eBookStore.Domain.Entities;

namespace eBookStore.Infrastructure.Repositories
{
    public interface IAdminRepository
    {
        Task<List<Book>> GetLowStockBooksAsync(int threshold);
        Task<List<(Book book, int copiesSold, decimal revenue)>> GetTopSellingBooksAsync(int count);
        Task<int> GetTotalBooksCountAsync();
        Task<int> GetTotalOrdersCountAsync();
        Task<decimal> GetTotalRevenueAsync();
        Task<int> GetTotalUsersCountAsync();
    }
}