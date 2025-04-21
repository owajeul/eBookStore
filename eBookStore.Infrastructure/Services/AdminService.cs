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
        private const int TOP_SELLING_BOOKS_COUNT = 10;
        private const int LOW_STOCK_THRESHOLD = 5;

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
            var topSellingBooksRaw = await _adminRepository.GetTopSellingBooksAsync(TOP_SELLING_BOOKS_COUNT);
            var topSellingBooks = topSellingBooksRaw.Select(b => new TopSellingBookDto
            {
                Id = b.book.Id,
                Title = b.book.Title,
                Author = b.book.Author,
                Price = b.book.Price,
                CopiesSold = b.copiesSold,
                Revenue = b.revenue
            }).ToList();

            var lowStockBooksRaw = await _adminRepository.GetLowStockBooksAsync(LOW_STOCK_THRESHOLD);

            var lowStockBooks = _mapper.Map<List<BookDto>>(lowStockBooksRaw.Take(10));

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

        public async Task<List<BookDto>> GetLowStockBooksAsync()
        {
            var lowStockBooks = await _adminRepository.GetLowStockBooksAsync(LOW_STOCK_THRESHOLD);
            return _mapper.Map<List<BookDto>>(lowStockBooks);
        }

    }
}