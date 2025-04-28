using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using eBookStore.Application.Common.Exceptions;
using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;
using eBookStore.Domain.Entities;

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
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new ArgumentException("User ID cannot be empty", nameof(userId));

                return await FetchUserCartAsync(userId);
            }
            catch (Exception ex) when (!(ex is CartServiceException))
            {
                throw new CartServiceException($"Failed to retrieve cart for user {userId}", ex);
            }
        }

        public async Task<CartDto> GetUserCartWithItemsAsync(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new ArgumentException("User ID cannot be empty", nameof(userId));

                return await FetchUserCartWithItemsAsync(userId);
            }
            catch (Exception ex) when (!(ex is CartServiceException))
            {
                throw new CartServiceException($"Failed to retrieve cart with items for user {userId}", ex);
            }
        }

        public async Task AddToCartAsync(string userId, int bookId, int quantity = 1)
        {
            try
            {
                await ValidateCartParameters(userId, bookId, quantity);
                await AddBookToCartAsync(userId, bookId, quantity);
            }
            catch (Exception ex) when (!(ex is CartServiceException))
            {
                throw new CartServiceException($"Failed to add book {bookId} to cart for user {userId}", ex);
            }
        }

        public async Task<bool> IsBookInCartAsync(string userId, int bookId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new ArgumentException("User ID cannot be empty", nameof(userId));

                if (bookId <= 0)
                    throw new ArgumentException("Book ID must be greater than zero", nameof(bookId));

                return await CheckIsBookInCartAsync(userId, bookId);
            }
            catch (Exception ex) when (!(ex is CartServiceException))
            {
                throw new CartServiceException($"Failed to check if book {bookId} is in cart for user {userId}", ex);
            }
        }

        public async Task IncreaseQuantityAsync(int cartId, int bookId)
        {
            try
            {
                await ValidateCartIdAndBookId(cartId, bookId);
                await IncreaseCartItemQuantityAsync(cartId, bookId);
            }
            catch (Exception ex) when (!(ex is CartServiceException))
            {
                throw new CartServiceException($"Failed to increase quantity for book {bookId} in cart {cartId}", ex);
            }
        }

        public async Task DecreaseQuantityAsync(int cartId, int bookId)
        {
            try
            {
                await ValidateCartIdAndBookId(cartId, bookId);
                await DecreaseCartItemQuantityAsync(cartId, bookId);
            }
            catch (Exception ex) when (!(ex is CartServiceException))
            {
                throw new CartServiceException($"Failed to decrease quantity for book {bookId} in cart {cartId}", ex);
            }
        }

        public async Task RemoveFromCartAsync(int cartId, int bookId)
        {
            try
            {
                await ValidateCartIdAndBookId(cartId, bookId);
                await RemoveItemFromCartAsync(cartId, bookId);
            }
            catch (Exception ex) when (!(ex is CartServiceException))
            {
                throw new CartServiceException($"Failed to remove book {bookId} from cart {cartId}", ex);
            }
        }

        public void UpdateCart(Cart cart)
        {
            try
            {
                if (cart == null)
                    throw new ArgumentNullException(nameof(cart));

                UpdateCartEntity(cart);
            }
            catch (Exception ex) when (!(ex is CartServiceException))
            {
                throw new CartServiceException($"Failed to update cart {cart?.Id}", ex);
            }
        }

        public async Task AddQuantityAsync(int cartId, int bookId, int quantity)
        {
            try
            {
                if (quantity <= 0)
                    throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

                await ValidateCartIdAndBookId(cartId, bookId);
                await AddToCartItemQuantityAsync(cartId, bookId, quantity);
            }
            catch (Exception ex) when (!(ex is CartServiceException))
            {
                throw new CartServiceException($"Failed to add {quantity} to book {bookId} in cart {cartId}", ex);
            }
        }

        public async Task MergeSessionCartWithDbCartAsync(string userId, CartDto sessionCart, CartDto? dbCart)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new ArgumentException("User ID cannot be empty", nameof(userId));

                if (sessionCart == null)
                    throw new ArgumentNullException(nameof(sessionCart));

                await MergeCartsAsync(userId, sessionCart, dbCart);
            }
            catch (Exception ex) when (!(ex is CartServiceException))
            {
                throw new CartServiceException($"Failed to merge session cart with database cart for user {userId}", ex);
            }
        }

        public async Task MergeSessionCartItemsIntoDbCartAsync(string userId, int dbCartId, CartDto sessionCart)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                    throw new ArgumentException("User ID cannot be empty", nameof(userId));

                if (dbCartId <= 0)
                    throw new ArgumentException("Database cart ID must be greater than zero", nameof(dbCartId));

                if (sessionCart == null)
                    throw new ArgumentNullException(nameof(sessionCart));

                await MergeCartItemsAsync(userId, dbCartId, sessionCart);
            }
            catch (Exception ex) when (!(ex is CartServiceException))
            {
                throw new CartServiceException($"Failed to merge session cart items into database cart {dbCartId} for user {userId}", ex);
            }
        }

        private async Task<CartDto?> FetchUserCartAsync(string userId)
        {
            var cart = await _cartRepository.GetUserCartAsync(userId);
            return _mapper.Map<CartDto>(cart);
        }

        private async Task<CartDto> FetchUserCartWithItemsAsync(string userId)
        {
            var cart = await _cartRepository.GetUserCartWithItemsAsync(userId)
                ?? new Cart { UserId = userId, CartItems = new List<CartItem>(), CreatedAt = DateTime.UtcNow };

            return _mapper.Map<CartDto>(cart);
        }

        private async Task ValidateCartParameters(string userId, int bookId, int quantity)
        {
            if (string.IsNullOrEmpty(userId))
                throw new ArgumentException("User ID cannot be empty", nameof(userId));

            if (bookId <= 0)
                throw new ArgumentException("Book ID must be greater than zero", nameof(bookId));

            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));

            var book = await _bookRepository.Get(b => b.Id == bookId);
            if (book == null)
                throw new BookNotFoundException($"Book with ID {bookId} not found");
        }

        private async Task AddBookToCartAsync(string userId, int bookId, int quantity)
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

        private async Task<bool> CheckIsBookInCartAsync(string userId, int bookId)
        {
            return await _cartRepository.IsBookInCartAsync(userId, bookId);
        }

        private async Task ValidateCartIdAndBookId(int cartId, int bookId)
        {
            if (cartId <= 0)
                throw new ArgumentException("Cart ID must be greater than zero", nameof(cartId));

            if (bookId <= 0)
                throw new ArgumentException("Book ID must be greater than zero", nameof(bookId));
        }

        private async Task IncreaseCartItemQuantityAsync(int cartId, int bookId)
        {
            var item = await _cartRepository.GetCartItemAsync(cartId, bookId);

            if (item == null)
                return;

            item.Quantity++;
            _cartRepository.UpdateCartItem(item);
            await _cartRepository.Save();
        }

        private async Task DecreaseCartItemQuantityAsync(int cartId, int bookId)
        {
            var item = await _cartRepository.GetCartItemAsync(cartId, bookId);

            if (item != null && item.Quantity > 1)
            {
                item.Quantity--;
                _cartRepository.UpdateCartItem(item);
                await _cartRepository.Save();
            }
            else
            {
                await _cartRepository.Save();
            }
        }

        private async Task RemoveItemFromCartAsync(int cartId, int bookId)
        {
            var cartItem = await _cartRepository.GetCartItemAsync(cartId, bookId);

            if (cartItem != null)
            {
                _cartRepository.RemoveCartItem(cartItem);
            }

            await _cartRepository.Save();
        }

        private void UpdateCartEntity(Cart cart)
        {
            _cartRepository.Update(cart);
        }

        private async Task AddToCartItemQuantityAsync(int cartId, int bookId, int quantity)
        {
            var item = await _cartRepository.GetCartItemAsync(cartId, bookId);

            if (item == null)
                return;

            item.Quantity += quantity;
            _cartRepository.UpdateCartItem(item);
            await _cartRepository.Save();
        }

        private async Task MergeCartsAsync(string userId, CartDto sessionCart, CartDto? dbCart)
        {
            if (dbCart == null)
            {
                var cart = _mapper.Map<Cart>(sessionCart);
                cart.UserId = userId;
                UpdateCartEntity(cart);
            }
            else
            {
                await MergeCartItemsAsync(userId, dbCart.Id, sessionCart);
            }
        }

        private async Task MergeCartItemsAsync(string userId, int dbCartId, CartDto sessionCart)
        {
            if (sessionCart.CartItems == null || sessionCart.CartItems.Count == 0)
                return;

            foreach (var item in sessionCart.CartItems)
            {
                if (item.BookId <= 0 || item.Quantity <= 0)
                    continue;

                if (await CheckIsBookInCartAsync(userId, item.BookId))
                {
                    await AddToCartItemQuantityAsync(dbCartId, item.BookId, item.Quantity);
                }
                else
                {
                    await AddBookToCartAsync(userId, item.BookId, item.Quantity);
                }
            }
        }
    }