using eBookStore.Application.Common.Utilily;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.Controllers
{
    [Authorize(Roles = AppConstant.Role_Admin)]
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
