using eBookStore.Application.Common.Utilily;
using eBookStore.Domain.Entities;
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
        public IActionResult Details(int id)
        {
            var order = _dbContext.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Book)
                .FirstOrDefault(o => o.Id == id);
            return View(order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Update(int orderId, string orderStatus)
        {
            var order = _dbContext.Orders.FirstOrDefault(o => o.Id == orderId);
            if (order == null)
            {
                return NotFound();
            }
            if(!AppConstant.ValidStatuses.Contains(orderStatus))
            {
                TempData["TostrMessage"] = "Invalid status!";
                TempData["TostrType"] = "error";
                return RedirectToAction("Details", new {id = orderId});
            }
            order.Status = orderStatus;
            _dbContext.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}
