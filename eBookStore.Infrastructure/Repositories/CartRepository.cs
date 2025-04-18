using eBookStore.Application.Common.Interfaces;
using eBookStore.Domain.Entities;
using eBookStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eBookStore.Infrastructure.Repositories;

public class CartRepository : Repository<Cart>, ICartRepository
{
    private readonly AppDbContext _dbContext;

    public CartRepository(AppDbContext dbContext) : base(dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Cart?> GetUserCartAsync(string userId)
    {
        return await _dbContext.Carts.FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<Cart?> GetUserCartWithItemsAsync(string userId)
    {
        return await _dbContext.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Book)
            .FirstOrDefaultAsync(c => c.UserId == userId);
    }

    public async Task<CartItem?> GetCartItemAsync(int cartId, int bookId)
    {
        return await _dbContext.CartItems.FirstOrDefaultAsync(ci => ci.CartId == cartId && ci.BookId == bookId);
    }
    public async Task<bool> IsBookInCartAsync(string userId, int bookId)
    {
        return await _dbContext.Carts
            .Include(c => c.CartItems)
            .AnyAsync(c => c.UserId == userId && c.CartItems.Any(ci => ci.BookId == bookId));
    }
    public void UpdateCartItem(CartItem cartItem)
    {
        _dbContext.CartItems.Update(cartItem);
    }
    public void RemoveCartItem(CartItem cartItem)
    {
        _dbContext.CartItems.Remove(cartItem);
    }
}
