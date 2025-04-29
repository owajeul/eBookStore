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

    public async Task<List<BookDto>> GetLowStockBooksAsync(int threshold, int? recordsToFetch)
    {
        try
        {
            if (threshold < 0)
                throw new ArgumentException("Threshold cannot be negative", nameof(threshold));

            return await FetchLowStockBooksAsync(threshold, recordsToFetch);
        }
        catch (Exception ex) when (!(ex is AdminServiceException))
        {
            throw new AdminServiceException($"Failed to retrieve low stock books with threshold {threshold}", ex);
        }
    }

    public async Task<List<TopSellingBookDto>> GetTopSellingBooksAsync(int count)
    {
        try
        {
            if (count <= 0)
                throw new ArgumentException("Count of top books must be greater than zero", nameof(count));

            return await FetchTopSellingBooksAsync(count);
        }
        catch (Exception ex) when (!(ex is AdminServiceException))
        {
            throw new AdminServiceException($"Failed to retrieve top {count} selling books", ex);
        }
    }

    private async Task<AdminDashboardDto> FetchAdminDashboardDataAsync()
    {
        var totalUsers = await _adminRepository.GetTotalUsersCountAsync();
        var totalOrders = await _adminRepository.GetTotalOrdersCountAsync();
        var totalRevenue = await _adminRepository.GetTotalRevenueAsync();
        var totalBooks = await _adminRepository.GetTotalBooksCountAsync();

        var topSellingBooks = await FetchTopSellingBooksAsync(MAX_RECORDS_FOR_DASHBOARD);
        var lowStockBooks = await FetchLowStockBooksAsync(LOW_STOCK_THRESHOLD, MAX_RECORDS_FOR_DASHBOARD);

        return new AdminDashboardDto
        {
            TotalUsers = totalUsers,
            TotalOrders = totalOrders,
            TotalRevenue = totalRevenue,
            TotalBooks = totalBooks,
            TopSellingBooks = topSellingBooks,
            LowStockBooks = lowStockBooks
        };
    }

    private async Task<List<BookDto>> FetchLowStockBooksAsync(int threshold, int? recordsToFetch)
    {
        var lowStockBooks = await _adminRepository.GetLowStockBooksAsync(threshold, recordsToFetch);

        if (lowStockBooks == null)
            return new List<BookDto>();

        return _mapper.Map<List<BookDto>>(lowStockBooks);
    }

    private async Task<List<TopSellingBookDto>> FetchTopSellingBooksAsync(int count)
    {
        var topSellingBooksRaw = await _adminRepository.GetTopSellingBooksAsync(count);

        if (topSellingBooksRaw == null)
            return new List<TopSellingBookDto>();

        return topSellingBooksRaw
            .Select(b => new TopSellingBookDto
            {
                Id = b.book.Id,
                Title = b.book.Title,
                Author = b.book.Author,
                Genre = b.book.Genre,
                Price = b.book.Price,
                CopiesSold = b.copiesSold,
                Revenue = b.revenue
            })
            .ToList();
    }
}