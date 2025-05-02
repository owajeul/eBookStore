using System.Threading.Tasks;
using eBookStore.Application.DTOs;
namespace eBookStore.Application.Interfaces;

public interface IAdminService
{
    Task<AdminDashboardDto> GetAdminDashboardDataAsync();

}