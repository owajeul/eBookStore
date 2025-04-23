using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using eBookStore.Application.Common.Dto;
using eBookStore.Application.Common.Interfaces;
using eBookStore.Application.Common.Utilily;
using eBookStore.Domain.Entities;
using eBookStore.Infrastructure.Repositories;

namespace eBookStore.Application.Services
{
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
            var totalUsers = await _adminRepository.GetTotalUsersCountAsync();
            var totalOrders = await _adminRepository.GetTotalOrdersCountAsync();
            var totalRevenue = await _adminRepository.GetTotalRevenueAsync();
            var totalBooks = await _adminRepository.GetTotalBooksCountAsync();
            var topSellingBooks = await GetTopSellingBooksAsync(MAX_RECORDS_FOR_DASHBOARD);
            var lowStockBooks = await GetLowStockBooksAsync(LOW_STOCK_THRESHOLD, MAX_RECORDS_FOR_DASHBOARD);

            return new AdminDashboardDto
            {
                TotalUsers = totalUsers,
                TotalOrders = totalOrders,
                TotalRevenue = totalRevenue,
                TotalBooks = totalBooks,
                TopSellingBooks = topSellingBooks.Take(10).ToList(),
                LowStockBooks = lowStockBooks
            };
        }

        public async Task<List<BookDto>> GetLowStockBooksAsync(int threshold, int? recordToFetch)
        {
            var lowStockBooks = await _adminRepository.GetLowStockBooksAsync(threshold, recordToFetch);
            return _mapper.Map<List<BookDto>>(lowStockBooks);
        }

        public async Task<List<TopSellingBookDto>> GetTopSellingBooksAsync(int count)
        {
            var topSellingBooksRaw = await _adminRepository.GetTopSellingBooksAsync(count);
            return topSellingBooksRaw.Select(b => new TopSellingBookDto
            {
                Id = b.book.Id,
                Title = b.book.Title,
                Author = b.book.Author,
                Price = b.book.Price,
                CopiesSold = b.copiesSold,
                Revenue = b.revenue
            }).ToList();

        }
    }
}
