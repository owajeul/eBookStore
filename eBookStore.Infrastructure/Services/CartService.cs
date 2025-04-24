using eBookStore.Application.Common.Interfaces;
using eBookStore.Domain.Entities;
using eBookStore.Infrastructure.Repositories;

namespace eBookStore.Infrastructure.Services;
public class CartService : ICartService
{
    private readonly ICartRepository _cartRepository;
    private readonly IBookRepository _bookRepository;
    public CartService(ICartRepository cartRepository, IBookRepository bookRepository)
    {
        _cartRepository = cartRepository;
        _bookRepository = bookRepository;
    }
    public async Task<Cart?> GetUserCartAsync(string userId)
    {
        return await _cartRepository.GetUserCartAsync(userId);
    }

    public async Task<Cart> GetUserCartWithItemsAsync(string userId)
    {
        return await _cartRepository.GetUserCartWithItemsAsync(userId)
            ?? new Cart { UserId = userId, CartItems = new List<CartItem>(), CreatedAt = DateTime.UtcNow };
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
}
