using eBookStore.Application.DTOs;
using eBookStore.Domain.Entities;

namespace eBookStore.Web.ViewModels
{
    public class AdminDashboardVM
    {
        public int TotalUsers { get; set; }
        public int TotalOrders { get; set; }
        public decimal TotalRevenue { get; set; }
        public int TotalBooks { get; set; }
    }
}
