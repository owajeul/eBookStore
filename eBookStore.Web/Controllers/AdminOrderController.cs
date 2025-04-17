using eBookStore.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eBookStore.Web.Controllers
{
    public class AdminOrderController : Controller
    {
        private readonly AppDbContext _dbContext;

        public AdminOrderController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public IActionResult Index()
        {
            var customerOrders = _dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .ToList();
            return View(customerOrders);
        }
    }
}
