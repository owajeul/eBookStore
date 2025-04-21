using System.Threading.Tasks;
using eBookStore.Application.Common.Dto;

namespace eBookStore.Application.Services
{
    public interface IAdminService
    {
        Task<AdminDashboardDto> GetAdminDashboardDataAsync();
        Task<List<BookDto>> GetLowStockBooksAsync();
    }
}