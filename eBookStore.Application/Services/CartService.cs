using AutoMapper;
using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;
using eBookStore.Domain.Entities;
using System.Net.Http;
using System.Security.Claims;

namespace eBookStore.Application.Services;
public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;

    public CartService(ICartRepository cartRepository, IBookRepository bookRepository, IMapper mapper)
    {
        _cartRepository = cartRepository;
        _bookRepository = bookRepository;
        _mapper = mapper;
    }
    public async Task<CartDto?> GetUserCartAsync(string userId)
    {
        var cart =  await _cartRepository.GetUserCartAsync(userId);
        return _mapper.Map<CartDto>(cart);
    }

    public async Task<CartDto> GetUserCartWithItemsAsync(string userId)
    {
        var cart = await _cartRepository.GetUserCartWithItemsAsync(userId)
            ?? new Cart { UserId = userId, CartItems = new List<CartItem>(), CreatedAt = DateTime.UtcNow };
        return _mapper.Map<CartDto>(cart);
    }

    public async Task AddToCartAsync(string userId, int bookId, int quantity = 1)
    {
        var cart = await _cartRepository.GetUserCartWithItemsAsync(userId);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                CartItems = new List<CartItem>()
            };
            await _cartRepository.Add(cart);
        }

        decimal price = await _bookRepository.GetBookPriceAsync(bookId);


        cart.CartItems.Add(new CartItem
        {
            BookId = bookId,
            Quantity = quantity,
            UnitPrice = price
        });

        await _cartRepository.Save();
    }

    public async Task<bool> IsBookInCartAsync(string userId, int bookId)
    {
        return await _cartRepository.IsBookInCartAsync(userId, bookId);
    }

    public async Task IncreaseQuantityAsync(int cartId, int bookId)
    {
        var item = await _cartRepository.GetCartItemAsync(cartId, bookId);
        if (item == null) return;
        item.Quantity++;
        _cartRepository.UpdateCartItem(item);
        await _cartRepository.Save();
    }

    public async Task DecreaseQuantityAsync(int cartId, int bookId)
    {
        var item = await _cartRepository.GetCartItemAsync(cartId, bookId);

        if (item != null && item.Quantity > 1)
        {
            item.Quantity--;
            _cartRepository.UpdateCartItem(item);
        }
        await _cartRepository.Save();
    }

    public async Task RemoveFromCartAsync(int cartId, int bookId)
    {
        var cartItem = await _cartRepository.GetCartItemAsync(cartId, bookId);
        if (cartItem != null)
        {
            _cartRepository.RemoveCartItem(cartItem);
        }
        await _cartRepository.Save();
    }

    public void UpdateCart(Cart cart)
    {
       _cartRepository.Update(cart);
    }

    public async Task AddQuantityAsync(int cartId, int bookId, int quantity)
    {
        var item = await _cartRepository.GetCartItemAsync(cartId, bookId);
        if (item == null) return;
        item.Quantity += quantity;
        _cartRepository.UpdateCartItem(item);
        await _cartRepository.Save();
    }

    public async Task MergeSessionCartWithDbCartAsync(string userId, CartDto sessionCart, CartDto? dbCart)
    {
        if (dbCart == null)
        {
            var cart = _mapper.Map<Cart>(sessionCart);
            UpdateCart(cart);
        }
        else
        {
            await MergeSessionCartItemsIntoDbCartAsync(userId, dbCart.Id, sessionCart);
        }
    }

    public async Task MergeSessionCartItemsIntoDbCartAsync(string userId, int dbCartId, CartDto sessionCart)
    {
        foreach (var item in sessionCart.CartItems)
        {
            if (await IsBookInCartAsync(userId, item.BookId))
            {
                await AddQuantityAsync(dbCartId, item.BookId, item.Quantity);
            }
            else
            {
                await AddToCartAsync(userId, item.BookId, item.Quantity);
            }
        }
    }
}
