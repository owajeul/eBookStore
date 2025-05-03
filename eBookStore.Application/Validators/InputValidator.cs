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
    }
}
