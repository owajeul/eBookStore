using AutoMapper;
using eBookStore.Application.Common.Utilily;
using eBookStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eBookStore.Web.Controllers
{
    public class AdminOrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly IMapper _mapper;

        public AdminOrderController(IOrderService orderService, IMapper mapper)
        {
            _orderService = orderService;
            _mapper = mapper;
        }
        public async Task<IActionResult> Index()
        {
            var customerOrders = await _orderService.GetAllOrdersAsync();
            return View(customerOrders);
        }
        public IActionResult Details(int id)
        {
            var order = _orderService.GetOrderById(id);
            return View(order);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int orderId, string orderStatus)
        {
            if(!AppConstant.ValidStatuses.Contains(orderStatus))
            {
                TempData["TostrMessage"] = "Invalid status!";
                TempData["TostrType"] = "error";
                return RedirectToAction("Details", new {id = orderId});
            }
            await _orderService.ChangeOrderStatus(orderId, orderStatus);
            return RedirectToAction("Index");
        }
    }
}
