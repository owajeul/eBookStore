using eBookStore.Domain.Entities;

namespace eBookStore.Application.Interfaces;

public interface IBookRepository : IRepository<Book>
{
    Task<decimal> GetBookPriceAsync(int bookId);
    Task AddBookReviewAsync(BookReview bookReview);
    Task<bool> HasUserPurchasedBookAsync(int bookId, string userId);
    Task<BookReview?> GetBookReviewAsync(int bookId, string userId);
    Task<Book?> GetBookWithReviewsAsync(int bookId);
    Task<List<string>> GetDistinctGenresAsync();
}