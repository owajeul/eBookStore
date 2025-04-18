using eBookStore.Application.Common.Dto;
using eBookStore.Application.Common.Interfaces;
using eBookStore.Application.Services;
using eBookStore.Domain.Entities;
using eBookStore.Infrastructure.Data.Identity;
using eBookStore.Infrastructure.Services;
using eBookStore.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICartService _cartService;
        private readonly IOrderService _orderService;

        public OrderController(
            UserManager<ApplicationUser> userManager,
            ICartService cartService,
            IOrderService orderService)
        {
            _userManager = userManager;
            _cartService = cartService;
            _orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var cart = await _cartService.GetUserCartWithItemsAsync(userId);

            if (!cart.CartItems.Any())
            {
                return RedirectToAction("Index", "Home");
            }

            var viewModel = new CheckoutVM
            {
                ShippingAddress = "",
                PhoneNumber = "",
                Items = cart.CartItems.ToList()
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PlaceOrder(CheckoutVM model)
        {
            if (!ModelState.IsValid)
            {
                TempData["ToastrMessage"] = "Please fill in all required fields.";
                TempData["ToastrType"] = "error";
                return RedirectToAction("Index");
            }

            var userId = _userManager.GetUserId(User);
            var cart = await _cartService.GetUserCartWithItemsAsync(userId);

            if (!cart.CartItems.Any())
            {
                TempData["ToastrMessage"] = "Your cart is empty.";
                TempData["ToastrType"] = "error";
                return RedirectToAction("Index", "Home");
            }

            var orderDto = new OrderDto
            {
                UserId = userId,
                ShippingAddress = model.ShippingAddress,
                PhoneNumber = model.PhoneNumber,
                OrderItems = cart.CartItems.Select(ci => new OrderItemDto
                {
                    BookId = ci.BookId,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice
                }).ToList()
            };
            await _orderService.PlaceOrderAsync(orderDto);

            TempData["ToastrMessage"] = "Order placed successfully.";
            TempData["ToastrType"] = "success";
            return RedirectToAction("Index", "Home");
        }
    }
}
