using eBookStore.Domain.Entities;

namespace eBookStore.Application.Common.Interfaces;
public interface ICartService
{
    Task AddToCartAsync(string userId, int bookId, int quantity = 1);
    Task DecreaseQuantityAsync(int cartId, int bookId);
    Task<Cart?> GetUserCartAsync(string userId);
    Task<Cart> GetUserCartWithItemsAsync(string userId);
    Task IncreaseQuantityAsync(int cartId, int bookId);
    Task AddQuantityAsync(int cartId, int bookId, int quantity);
    Task<bool> IsBookInCartAsync(string userId, int bookId);
    Task RemoveFromCartAsync(int cartId, int bookId);
    void UpdateCart(Cart cart);
}