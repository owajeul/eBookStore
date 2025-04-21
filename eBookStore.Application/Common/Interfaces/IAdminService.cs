using eBookStore.Application.Common.Dto;

namespace eBookStore.Application.Services
{
    public interface IAdminService
    {
        Task<AdminDashboardDto> GetAdminDashboardDataAsync();
    }
}