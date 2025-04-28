using eBookStore.Domain.Entities;

namespace eBookStore.Application.Interfaces;

public interface ICartRepository : IRepository<Cart>
{
    Task<CartItem?> GetCartItemAsync(int cartId, int bookId);
    Task<Cart?> GetUserCartAsync(string userId);
    Task<Cart?> GetUserCartWithItemsAsync(string userId);
    Task<bool> IsBookInCartAsync(string userId, int bookId);
    void RemoveCartItem(CartItem cartItem);
    void UpdateCartItem(CartItem cartItem);
    Task ClearCartAsync(string userId);
}