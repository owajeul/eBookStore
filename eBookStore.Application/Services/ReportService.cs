using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;

namespace eBookStore.Application.Services
{
    public class ReportService : IReportService
    {
        private readonly IOrderRepository _orderRepository;

        public ReportService(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        public async Task<List<UserPurchaseReportDto>> GetUserPurchaseReportsAsync()
        {
            return await _orderRepository.GetUserPurchaseReportsAsync();
        }
    }

}
