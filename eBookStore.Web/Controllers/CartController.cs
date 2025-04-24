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
using Newtonsoft.Json;

namespace eBookStore.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IBookService _bookService;

    public CartController(ICartService cartService, UserManager<ApplicationUser> userManager, IBookService bookService)
    {
        _cartService = cartService;
        _userManager = userManager;
        _bookService = bookService;
    }
    public async Task<IActionResult> Index()
    {
        Cart cart = new Cart();

        if (User.Identity.IsAuthenticated)
        {
            cart = await _cartService.GetUserCartWithItemsAsync(_userManager.GetUserId(User));
        }
        else
        {
            var sessionCartJson = HttpContext.Session.GetString("Cart");

            if (!string.IsNullOrEmpty(sessionCartJson))
            {
                cart = JsonConvert.DeserializeObject<Cart>(sessionCartJson);
            }
        }

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

    [HttpPost]
    public async Task<IActionResult> Add(int bookId)
    {
        var response = new { message = "" , status = ""};

        if (User.Identity.IsAuthenticated)
        {
            var userId = _userManager.GetUserId(User);
            if (await _cartService.IsBookInCartAsync(userId, bookId))
            {
                return Ok(new {message = "Book already in cart!" , status = "warning"});
            }
            else
            {
                await _cartService.AddToCartAsync(userId, bookId);
                return Ok(new { message = "Book successfully added to cart" , status = "success"});
            }
        }
        else
        {
            var sessionCartJson = HttpContext.Session.GetString("Cart");
            var cart = new Cart();
            if (!string.IsNullOrEmpty(sessionCartJson))
            {
                cart = JsonConvert.DeserializeObject<Cart>(sessionCartJson);
            }

            var bookExistInCart = cart.CartItems.FirstOrDefault(c => c.BookId == bookId);

            if(bookExistInCart != null)
            {
                return Ok(new { message = "Book already in cart!" , status = "warning"});
            }
            else
            {
                var book = await _bookService.GetBookAsync(bookId);
                cart.CartItems.Add(new CartItem
                {
                    BookId = book.Id,
                    Book = book,
                    Quantity = 1,
                    UnitPrice = book.Price
                });
            }
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
            return Ok(new { message = "Book successfully added to cart" , status = "success" });
        }
    }

    public async Task<IActionResult> IncreaseBookQuantity(int cartId, int bookId)
    {
        if(User.Identity.IsAuthenticated)
        {
            await _cartService.IncreaseQuantityAsync(cartId, bookId);
        }
        else
        {
            var sessionCartJson = HttpContext.Session.GetString("Cart");
            var cart = new Cart();
            if (!string.IsNullOrEmpty(sessionCartJson))
            {
                cart = JsonConvert.DeserializeObject<Cart>(sessionCartJson);
            }

            var cartItem = cart.CartItems.FirstOrDefault(c => c.BookId == bookId);
            cartItem.Quantity += 1;

            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
        }
        return Ok();
    }
    public async Task<IActionResult> DecreaseBookQuantity(int cartId, int bookId)
    {
        if(User.Identity.IsAuthenticated)
        {
            await _cartService.DecreaseQuantityAsync(cartId, bookId);
        }
        else
        {
            var sessionCartJson = HttpContext.Session.GetString("Cart");
            var cart = new Cart();
            if (!string.IsNullOrEmpty(sessionCartJson))
            {
                cart = JsonConvert.DeserializeObject<Cart>(sessionCartJson);
            }

            var cartItem = cart.CartItems.FirstOrDefault(c => c.BookId == bookId);

            if(cartItem.Quantity > 1)
            {
                cartItem.Quantity -= 1;
            }

            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
        }
        return Ok();
    }


    public async Task<IActionResult> DeleteCartItem(int cartId, int bookId)
    {
        var userId = _userManager.GetUserId(User);
        await _cartService.RemoveFromCartAsync(cartId, bookId);
        return Ok();
    }

}
