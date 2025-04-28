using AutoMapper;
using eBookStore.Application.Common.Utilily;
using eBookStore.Application.Interfaces;
using eBookStore.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.Controllers
{
    [Authorize(Roles = AppConstant.Role_Admin)]
    public class DashboardController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IMapper _mapper;
        private const int LOW_STOCK_THRESHOLD = 5;
        private const int TOP_SELLING_BOOKS_COUNT = 100;
        public DashboardController(IAdminService adminService, IMapper mapper)
        {
            _adminService = adminService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var dashboardData = await _adminService.GetAdminDashboardDataAsync();
            var dashBoardViewModel = _mapper.Map<AdminDashboardVM>(dashboardData);
            return View(dashBoardViewModel);
        }

        public async Task<IActionResult> LowStockBook()
        {
            var lowStockBooks = await _adminService.GetLowStockBooksAsync(LOW_STOCK_THRESHOLD);
            var lowStockBooksViewModel = _mapper.Map<List<BookVM>>(lowStockBooks);
            return View(lowStockBooksViewModel);
        }

        public async Task<IActionResult> TopSellingBook()
        {
            var topSellingBooks = await _adminService.GetTopSellingBooksAsync(TOP_SELLING_BOOKS_COUNT);
            var topSellingBooksViewModel = _mapper.Map<List<TopSellingBookVM>>(topSellingBooks);
            return View(topSellingBooksViewModel);
        }
    }
}
