using System.Threading.Tasks;
using eBookStore.Application.DTOs;
namespace eBookStore.Application.Interfaces;

public interface IAdminService
{
    Task<AdminDashboardDto> GetAdminDashboardDataAsync();
    Task<List<BookDto>> GetLowStockBooksAsync(int threshold, int? recordToFetch = null);
    Task<List<TopSellingBookDto>> GetTopSellingBooksAsync(int count);

}