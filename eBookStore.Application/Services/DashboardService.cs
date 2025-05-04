using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;
using eBookStore.Application.Common.Exceptions;

namespace eBookStore.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private const int LOW_STOCK_THRESHOLD = 5;
    private const int MAX_RECORDS_FOR_DASHBOARD = 10;

    public DashboardService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<AdminDashboardDto> GetAdminDashboardDataAsync()
    {
        try
        {
            return await FetchAdminDashboardDataAsync();
        }
        catch (Exception ex)
        {
            throw new AdminServiceException("Failed to retrieve admin dashboard data", ex);
        }
    }

    private async Task<AdminDashboardDto> FetchAdminDashboardDataAsync()
    {
        var totalUsers = await _unitOfWork.Admin.GetTotalUsersCountAsync();
        var totalOrders = await _unitOfWork.Admin.GetTotalOrdersCountAsync();
        var totalRevenue = await _unitOfWork.Admin.GetTotalRevenueAsync();
        var totalBooks = await _unitOfWork.Admin.GetTotalBooksCountAsync();

        return new AdminDashboardDto
        {
            TotalUsers = totalUsers,
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            TotalBooks = totalBooks
        };
    }
}