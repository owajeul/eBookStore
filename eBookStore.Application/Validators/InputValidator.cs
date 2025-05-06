using eBookStore.Application.Common.Utilily;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eBookStore.Application.Validators
{
    public static class InputValidator
    {
        public static void ValidateBookId(int id)
        {
            if (id <= 0)
                throw new ArgumentException("Book ID must be greater than zero", nameof(id));
        }

        public static void ValidateStock(int stock)
        {
            if (stock < 0)
                throw new ArgumentException("Stock value cannot be negative", nameof(stock));
        }
        public static void ValidateRating(int rating)
        {
            if (rating < 0 || rating > 5)
                throw new ArgumentOutOfRangeException(nameof(rating), "Rating must be between 0 and 5");
        }
        public static void ValidateComment(string comment)
        {
            if (string.IsNullOrWhiteSpace(comment))
                throw new ArgumentException("Comment cannot be empty or whitespace.", nameof(comment));

            if (comment.Length > 500)
                throw new ArgumentException("Comment cannot exceed 1000 characters.", nameof(comment));
        }

        public static void ValidateCartId(int cartId)
        {
            if (cartId <= 0)
                throw new ArgumentException("Cart ID must be greater than zero", nameof(cartId));
        }
        public static void ValidateCartItemQuantity(int quantity)
        {
            if (quantity <= 0)
                throw new ArgumentException("Quantity must be greater than zero", nameof(quantity));
        }
        public static void ValidateUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException("User ID cannot be empty or whitespace.", nameof(userId));

        }

        public static void ValidateOrderId(int orderId)
        {
            if (orderId <= 0)
                throw new ArgumentException("Order ID must be greater than zero", nameof(orderId));
        }
        public static void ValidateOrderStatus(string status)
        {
            if (!AppConstant.ValidStatuses.Contains(status))
                throw new ArgumentException($"Invalid order status: {status}. Valid statuses are: {string.Join(", ", AppConstant.ValidStatuses)}", nameof(status));
        }
        public static void ValidatePaymentStatus(string status)
        {
            if (status != AppConstant.PaymentStatusPaid && status != AppConstant.PaymentStatusPending)
                throw new ArgumentException($"Invalid payment status: {status}");
        }

    }
}
