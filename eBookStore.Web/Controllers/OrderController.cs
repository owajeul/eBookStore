using AutoMapper;
using eBookStore.Application.Common.Utilily;
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
            Items = _mapper.Map<List<CartItemVM>>(cart.CartItems)
        };

        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> PlaceOrder(CheckoutVM model)
    {
        if (model.Payment.Method == AppConstant.PaymentMethodCOD)
        {
            var cardPaymentErrors = ModelState.Keys
                .Where(k => k.StartsWith("Payment.CardPayment."))
                .ToList();

            foreach (var error in cardPaymentErrors)
            {
                ModelState.Remove(error);
            }
        }

        if (!ModelState.IsValid)
        {
            TempData["ToastrMessage"] = "Please fill in all required fields.";
            TempData["ToastrType"] = "error";
            return RedirectToAction("Index");
        }

        var user = await _userService.GetUserAsync();
        var cart = await _cartService.GetUserCartWithItemsAsync(user.UserId);

        if (!cart.CartItems.Any())
        {
            TempData["ToastrMessage"] = "Your cart is empty.";
            TempData["ToastrType"] = "error";
            return RedirectToAction("Index", "Home");
        }

        var orderDto = new OrderDto
        {
            UserId = user.UserId,
            OrderDate = DateTime.UtcNow,
            ShippingAddress = _mapper.Map<AddressDto>(model.ShippingAddress),
            BillingAddress = _mapper.Map<AddressDto>(model.BillingAddress),
            PaymentMethod = model.Payment.Method,
            PaymentStatus = AppConstant.PaymentStatusPending,
            Status = AppConstant.StatusPending,
            OrderItems = cart.CartItems.Select(ci => new OrderItemDto
            {
                BookId = ci.BookId,
                Book = new BookDto
                {
                    Id = ci.BookId,
                    Title = ci.Book.Title,
                    Author = ci.Book.Author,
                    ImageUrl = ci.Book.ImageUrl,
                    Price = ci.Book.Price
                },
                Quantity = ci.Quantity,
                UnitPrice = ci.UnitPrice
            }).ToList()
        };

        if(model.Payment.Method == AppConstant.PaymentMethodCreditCard)
        {
          orderDto.PaymentStatus = AppConstant.PaymentStatusPaid;
        }

        await _orderService.PlaceOrderAsync(orderDto, user.Email);

        TempData["ToastrMessage"] = "Order placed successfully.";
        TempData["ToastrType"] = "success";
        return RedirectToAction("Index", "Home");
    }
}
