using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using AutoMapper;
using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;
using eBookStore.Application.Common.Exceptions;

namespace eBookStore.Application.Services;

public class AdminService : IAdminService
{
    private readonly IAdminRepository _adminRepository;
    private readonly IMapper _mapper;
    private const int LOW_STOCK_THRESHOLD = 5;
    private const int MAX_RECORDS_FOR_DASHBOARD = 10;

    public AdminService(IAdminRepository adminRepository, IMapper mapper)
    {
        _adminRepository = adminRepository;
        _mapper = mapper;
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
        var totalUsers = await _adminRepository.GetTotalUsersCountAsync();
        var totalOrders = await _adminRepository.GetTotalOrdersCountAsync();
        var totalRevenue = await _adminRepository.GetTotalRevenueAsync();
        var totalBooks = await _adminRepository.GetTotalBooksCountAsync();

        return new AdminDashboardDto
        {
            TotalUsers = totalUsers,
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            TotalBooks = totalBooks
        };
    }
}