using eBookStore.Application.DTOs;

namespace eBookStore.Application.Interfaces
{
    public interface IReportService
    {
        Task<List<UserPurchaseReportDto>> GetUserPurchaseReportsAsync();
    }
}