using AutoMapper;
using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;
using eBookStore.Application.Services;
using eBookStore.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.Controllers;

[Authorize]
public class OrderController : Controller
{
    private readonly ICartService _cartService;
    private readonly IOrderService _orderService;
    private readonly IUserService _userService;
    private readonly IMapper _mapper;

    public OrderController(
        ICartService cartService,
        IOrderService orderService,
        IUserService userService,
        IMapper mapper
        )
    {
        _cartService = cartService;
        _orderService = orderService;
        _userService = userService;
        _mapper = mapper;
    }

    public async Task<IActionResult> Index()
    {
        var userId = _userService.GetUserId();
        var cart = await _cartService.GetUserCartWithItemsAsync(userId);

        if (!cart.CartItems.Any())
        {
            return RedirectToAction("Index", "Home");
        }

        var viewModel = new CheckoutVM
        {
            ShippingAddress = "",
            PhoneNumber = "",
            Items = _mapper.Map<List<CartItemVM>>(cart.CartItems)
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

        var userId = _userService.GetUserId();
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
