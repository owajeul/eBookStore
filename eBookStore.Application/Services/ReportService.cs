using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using eBookStore.Application.Common.Exceptions;
using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;

namespace eBookStore.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly IMapper _mapper;
        public ReportService(IOrderRepository orderRepository, IAdminRepository adminRepository, IMapper mapper)
        {
            _orderRepository = orderRepository;
            _adminRepository = adminRepository;
            _mapper = mapper;
        }

        public async Task<List<UserPurchaseReportDto>> GetUserPurchaseReportsAsync()
        {
            return await _orderRepository.GetUserPurchaseReportsAsync();
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

}
