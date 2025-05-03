using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using eBookStore.Application.Common.Exceptions;
using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;
using eBookStore.Application.Validators;
using eBookStore.Domain.Entities;

namespace eBookStore.Application.Services;
public class CartService : ICartService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CartService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CartDto?> GetUserCartAsync(string userId)
    {
        try
        {
            InputValidator.ValidateUserId(userId);
            return await FetchUserCartAsync(userId);
        }
        catch (Exception ex) when (!(ex is CartServiceException))
        {
            throw new CartServiceException($"Failed to retrieve cart for user {userId}", ex);
        }
    }
    private async Task<CartDto?> FetchUserCartAsync(string userId)
    {
        var cart = await _unitOfWork.Cart.GetUserCartAsync(userId);
        return _mapper.Map<CartDto>(cart);
    }

    public async Task<CartDto> GetUserCartWithItemsAsync(string userId)
    {
        try
        {
            InputValidator.ValidateUserId(userId);
            return await FetchUserCartWithItemsAsync(userId);
        }
        catch (Exception ex) when (!(ex is CartServiceException))
        {
            throw new CartServiceException($"Failed to retrieve cart with items for user {userId}", ex);
        }
    }
    private async Task<CartDto> FetchUserCartWithItemsAsync(string userId)
    {
        var cart = await _unitOfWork.Cart.GetUserCartWithItemsAsync(userId)
            ?? new Cart { UserId = userId, CartItems = new List<CartItem>(), CreatedAt = DateTime.UtcNow };

        return _mapper.Map<CartDto>(cart);
    }

    public async Task AddToCartAsync(string userId, int bookId, int quantity = 1)
    {
        try
        {
            InputValidator.ValidateUserId(userId);
            InputValidator.ValidateBookId(bookId);
            InputValidator.ValidateCartItemQuantity(quantity);
            await AddBookToCartAsync(userId, bookId, quantity);
            await _unitOfWork.SaveAsync();
        }
        catch (Exception ex) when (!(ex is CartServiceException))
        {
            throw new CartServiceException($"Failed to add book {bookId} to cart for user {userId}", ex);
        }
    }
    private async Task AddBookToCartAsync(string userId, int bookId, int quantity)
    {
        var cart = await _unitOfWork.Cart.GetUserCartWithItemsAsync(userId);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                CartItems = new List<CartItem>()
            };
            await _unitOfWork.Cart.Add(cart);
        }

        decimal price = await _unitOfWork.Book.GetBookPriceAsync(bookId);

        cart.CartItems.Add(new CartItem
        {
            BookId = bookId,
            Quantity = quantity,
            UnitPrice = price
        });

    }

    public async Task<bool> IsBookInCartAsync(string userId, int bookId)
    {
        try
        {
            InputValidator.ValidateUserId(userId);
            InputValidator.ValidateBookId(bookId);
            return await CheckIsBookInCartAsync(userId, bookId);
        }
        catch (Exception ex) when (!(ex is CartServiceException))
        {
            throw new CartServiceException($"Failed to check if book {bookId} is in cart for user {userId}", ex);
        }
    }
    private async Task<bool> CheckIsBookInCartAsync(string userId, int bookId)
    {
        return await _unitOfWork.Cart.IsBookInCartAsync(userId, bookId);
    }

    public async Task IncreaseQuantityAsync(int cartId, int bookId)
    {
        try
        {
            InputValidator.ValidateCartId(cartId);
            InputValidator.ValidateBookId(bookId);
            await IncreaseCartItemQuantityAsync(cartId, bookId);
            await _unitOfWork.SaveAsync();
        }
        catch (Exception ex) when (!(ex is CartServiceException))
        {
            throw new CartServiceException($"Failed to increase quantity for book {bookId} in cart {cartId}", ex);
        }
    }
    private async Task IncreaseCartItemQuantityAsync(int cartId, int bookId)
    {
        var item = await _unitOfWork.Cart.GetCartItemAsync(cartId, bookId);

        if (item == null)
            throw new CartServiceException($"Cart item with book ID {bookId} not found in cart {cartId}");

        item.Quantity++;
        _unitOfWork.Cart.UpdateCartItem(item);
    }

    public async Task DecreaseQuantityAsync(int cartId, int bookId)
    {
        try
        {
            InputValidator.ValidateCartId(cartId);
            await DecreaseCartItemQuantityAsync(cartId, bookId);
            await _unitOfWork.SaveAsync();
        }
        catch (Exception ex) when (!(ex is CartServiceException))
        {
            throw new CartServiceException($"Failed to decrease quantity for book {bookId} in cart {cartId}", ex);
        }
    }
    private async Task DecreaseCartItemQuantityAsync(int cartId, int bookId)
    {
        var item = await _unitOfWork.Cart.GetCartItemAsync(cartId, bookId);
        if (item == null)
            throw new CartServiceException($"Cart item with book ID {bookId} not found in cart {cartId}");
        if (item.Quantity > 1)
        {
            item.Quantity--;
            _unitOfWork.Cart.UpdateCartItem(item);
        }
    }

    public async Task RemoveFromCartAsync(int cartId, int bookId)
    {
        try
        {
            InputValidator.ValidateCartId(cartId);
            InputValidator.ValidateBookId(bookId);
            await RemoveItemFromCartAsync(cartId, bookId);
            await _unitOfWork.SaveAsync();
        }
        catch (Exception ex) when (!(ex is CartServiceException))
        {
            throw new CartServiceException($"Failed to remove book {bookId} from cart {cartId}", ex);
        }
    }
    private async Task RemoveItemFromCartAsync(int cartId, int bookId)
    {
        var cartItem = await _unitOfWork.Cart.GetCartItemAsync(cartId, bookId);

        if (cartItem != null)
        {
            _unitOfWork.Cart.RemoveCartItem(cartItem);
        }
    }

    public async Task AddQuantityAsync(int cartId, int bookId, int quantity)
    {
        try
        {
            InputValidator.ValidateCartId(cartId);
            InputValidator.ValidateBookId(bookId);
            InputValidator.ValidateCartItemQuantity(quantity);
            await AddToCartItemQuantityAsync(cartId, bookId, quantity);
            await _unitOfWork.SaveAsync();  
        }
        catch (Exception ex) when (!(ex is CartServiceException))
        {
            throw new CartServiceException($"Failed to add {quantity} to book {bookId} in cart {cartId}", ex);
        }
    }
    private async Task AddToCartItemQuantityAsync(int cartId, int bookId, int quantity)
    {
        var item = await _unitOfWork.Cart.GetCartItemAsync(cartId, bookId);

        if (item == null)
            return;

        item.Quantity += quantity;
        _unitOfWork.Cart.UpdateCartItem(item);
    }

    public async Task MergeSessionCartWithDbCartAsync(string userId, CartDto? sessionCart, CartDto? dbCart)
    {
        try
        {
            InputValidator.ValidateUserId(userId);
            await MergeCartsAsync(userId, sessionCart, dbCart);
            await _unitOfWork.SaveAsync();
        }
        catch (Exception ex) when (!(ex is CartServiceException))
        {
            throw new CartServiceException($"Failed to merge session cart with database cart for user {userId}", ex);
        }
    }
    private async Task MergeCartsAsync(string userId, CartDto? sessionCart, CartDto? dbCart)
    {
        if (sessionCart == null)
            return;
        foreach (var item in sessionCart.CartItems)
        {
            if (dbCart == null || !await IsBookInCartAsync(userId, item.BookId))
            {
                await AddBookToCartAsync(userId, item.BookId, item.Quantity);
            }
            else
            {
                await AddToCartItemQuantityAsync(dbCart.Id, item.BookId, item.Quantity);
            }
        }
    }
}