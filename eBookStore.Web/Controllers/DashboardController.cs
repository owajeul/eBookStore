using AutoMapper;
using eBookStore.Application.Common.Utilily;
using eBookStore.Application.Services;
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
    }
}
