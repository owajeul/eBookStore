using System.Threading.Tasks;
using eBookStore.Application.Common.Interfaces;
using eBookStore.Domain.Entities;
using eBookStore.Infrastructure.Data;
using eBookStore.Infrastructure.Data.Identity;
using eBookStore.Infrastructure.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace eBookStore.Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;
        public CartController(ICartService cartService, UserManager<ApplicationUser> userManager)
        {
            _cartService = cartService;
            _userManager = userManager;
        }
        public async Task<IActionResult> Index()
        {
            var cart = await _cartService.GetUserCartWithItemsAsync(_userManager.GetUserId(User));
            decimal subtotal = 0;
            decimal shippingCosts = 0;
            foreach (var item in cart.CartItems)
            {
                subtotal += item.Quantity * item.UnitPrice;
            }

            ViewBag.Subtotal = subtotal;
            ViewBag.ShippingCosts = shippingCosts;
            ViewBag.TotalCosts = subtotal + shippingCosts;
            return View(cart);
        }

        public async Task<IActionResult> Add(int bookId)
        {
            var userId = _userManager.GetUserId(User);

            if(await _cartService.IsBookInCartAsync(userId, bookId))
            {
                TempData["ToastrMessage"] = "Book already in cart!";
                TempData["ToastrType"] = "warning";
            }
            else
            {
                await _cartService.AddToCartAsync(userId, bookId);           
                TempData["ToastrMessage"] = "Book added to cart successfully!";
                TempData["ToastrType"] = "success";
            }
            return RedirectToAction("Details", "Book", new {id = bookId});
        }

        public async Task<IActionResult> IncreaseBookQuantity(int cartId, int bookId)
        {
            await _cartService.IncreaseQuantityAsync(cartId, bookId);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> DecreaseBookQuantity(int cartId, int bookId)
        {
            await _cartService.DecreaseQuantityAsync(cartId, bookId);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteCartItem(int cartId, int bookId)
        {
            var userId = _userManager.GetUserId(User);
            await _cartService.RemoveFromCartAsync(cartId, bookId);
            TempData["ToastrMessage"] = "Book removed from cart successfully!";
            TempData["ToastrType"] = "success";
            return RedirectToAction("Index");
        }
    }
}
