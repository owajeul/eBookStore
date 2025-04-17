using eBookStore.Domain.Entities;

namespace eBookStore.Application.Common.Interfaces;

public interface ICartRepository: IRepository<Cart>
{
    Task<CartItem?> GetCartItemAsync(int cartId, int bookId);
    Task<Cart?> GetUserCartAsync(string userId);
    Task<Cart?> GetUserCartWithItemsAsync(string userId);
    void RemoveCartItem(CartItem cartItem);
    void UpdateCartItem(CartItem cartItem);
}