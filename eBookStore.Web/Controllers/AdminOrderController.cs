using AutoMapper;
using eBookStore.Application.Common.Utilily;
using eBookStore.Application.Interfaces;
using eBookStore.Web.ViewModels;
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
            var orderViewModel = _mapper.Map<List<OrderVM>>(customerOrders);
            return View(orderViewModel);
        }
        public IActionResult Details(int id)
        {
            var order = _orderService.GetOrderById(id);
            var orderViewModel = _mapper.Map<OrderVM>(order);
            return View(orderViewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int orderId, string orderStatus)
        {
            await _orderService.ChangeOrderStatus(orderId, orderStatus);
            return RedirectToAction("Index");
        }
    }
}
