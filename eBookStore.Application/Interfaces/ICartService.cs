using eBookStore.Application.DTOs;
using eBookStore.Domain.Entities;

namespace eBookStore.Application.Interfaces;
public interface ICartService
{
    Task AddToCartAsync(string userId, int bookId, int quantity = 1);
    Task DecreaseQuantityAsync(int cartId, int bookId);
    Task<CartDto?> GetUserCartAsync(string userId);
    Task<CartDto> GetUserCartWithItemsAsync(string userId);
    Task IncreaseQuantityAsync(int cartId, int bookId);
    Task AddQuantityAsync(int cartId, int bookId, int quantity);
    Task<bool> IsBookInCartAsync(string userId, int bookId);
    Task RemoveFromCartAsync(int cartId, int bookId);
    Task MergeSessionCartWithDbCartAsync(string userId, CartDto? sessionCart, CartDto? dbCart);
}