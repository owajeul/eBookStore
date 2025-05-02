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
        private readonly IDashboardService _adminService;
        private readonly IMapper _mapper;
        private const int LOW_STOCK_THRESHOLD = 5;
        private const int TOP_SELLING_BOOKS_COUNT = 100;
        public DashboardController(IDashboardService adminService, IMapper mapper)
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

        public IActionResult LowStockBook()
        {
            return View();
        }

        public IActionResult TopSellingBook()
        {
            return View();
        }
        public IActionResult UserPurchaseReport()
        {
            return View();
        }
    }
}
