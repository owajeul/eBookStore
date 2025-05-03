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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public ReportService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<List<UserPurchaseReportDto>> GetUserPurchaseReportsAsync()
        {
            return await _unitOfWork.Order.GetUserPurchaseReportsAsync();
        }
        public async Task<List<BookDto>> GetLowStockBooksAsync(int threshold, int? recordsToFetch)
        {
            try
            {
                return await FetchLowStockBooksAsync(threshold, recordsToFetch);
            }
            catch (Exception ex) when (!(ex is AdminServiceException))
            {
                throw new AdminServiceException($"Failed to retrieve low stock books with threshold {threshold}", ex);
            }
        }
        private async Task<List<BookDto>> FetchLowStockBooksAsync(int threshold, int? recordsToFetch)
        {
            var lowStockBooks = await _unitOfWork.Admin.GetLowStockBooksAsync(threshold, recordsToFetch);

            if (lowStockBooks == null)
                return new List<BookDto>();

            return _mapper.Map<List<BookDto>>(lowStockBooks);
        }

        public async Task<List<TopSellingBookDto>> GetTopSellingBooksAsync(int count)
        {
            try
            {
                return await FetchTopSellingBooksAsync(count);
            }
            catch (Exception ex) when (!(ex is AdminServiceException))
            {
                throw new AdminServiceException($"Failed to retrieve top {count} selling books", ex);
            }
        }
        private async Task<List<TopSellingBookDto>> FetchTopSellingBooksAsync(int count)
        {
            var topSellingBooks = await _unitOfWork.Admin.GetTopSellingBooksAsync(count);

            if (topSellingBooks == null)
                return new List<TopSellingBookDto>();

            return topSellingBooks
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
