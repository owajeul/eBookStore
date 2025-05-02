using eBookStore.Application.DTOs;

namespace eBookStore.Application.Interfaces
{
    public interface IReportService
    {
        Task<List<UserPurchaseReportDto>> GetUserPurchaseReportsAsync();
        Task<List<BookDto>> GetLowStockBooksAsync(int threshold, int? recordToFetch = null);
        Task<List<TopSellingBookDto>> GetTopSellingBooksAsync(int count);
    }
}