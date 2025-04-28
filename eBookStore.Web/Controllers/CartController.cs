using AutoMapper;
using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;
using eBookStore.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace eBookStore.Web.Controllers;

public class CartController : Controller
{
    private readonly ICartService _cartService;
    private readonly IUserService _userService;
    private readonly IBookService _bookService;
    private readonly IMapper _mapper;

    public CartController(ICartService cartService, 
        IUserService userService, 
        IBookService bookService,
        IMapper mapper
        )
    {
        _cartService = cartService;
        _userService = userService;
        _bookService = bookService;
        _mapper = mapper;
    }
    public async Task<IActionResult> Index()
    {
        var cart = new CartVM();

        if (User.Identity.IsAuthenticated)
        {
            var cartDto = await _cartService.GetUserCartWithItemsAsync(_userService.GetUserId());
            cart = _mapper.Map<CartVM>(cartDto);
        }
        else
        {
            var sessionCartJson = HttpContext.Session.GetString("Cart");

            if (!string.IsNullOrEmpty(sessionCartJson))
            {
                cart = JsonConvert.DeserializeObject<CartVM>(sessionCartJson);
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
            var userId = _userService.GetUserId();
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
            var cart = new CartVM();
            if (!string.IsNullOrEmpty(sessionCartJson))
            {
                cart = JsonConvert.DeserializeObject<CartVM>(sessionCartJson);
            }

            var bookExistInCart = cart.CartItems.FirstOrDefault(c => c.BookId == bookId);

            if(bookExistInCart != null)
            {
                return Ok(new { message = "Book already in cart!" , status = "warning"});
            }
            else
            {
                var bookDto = await _bookService.GetBookAsync(bookId);
                var book = _mapper.Map<BookVM>(bookDto);
                cart.CartItems.Add(new CartItemVM
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
            var cart = new CartVM();
            if (!string.IsNullOrEmpty(sessionCartJson))
            {
                cart = JsonConvert.DeserializeObject<CartVM>(sessionCartJson);
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
            var cart = new CartVM();
            if (!string.IsNullOrEmpty(sessionCartJson))
            {
                cart = JsonConvert.DeserializeObject<CartVM>(sessionCartJson);
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
        var userId = _userService.GetUserId();
        await _cartService.RemoveFromCartAsync(cartId, bookId);
        return Ok();
    }

}
