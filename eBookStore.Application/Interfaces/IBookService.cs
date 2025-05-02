using eBookStore.Application.DTOs;
using eBookStore.Domain.Entities;

namespace eBookStore.Application.Interfaces;

public interface IBookService
{
    Task<List<BookDto>> GetAllBooksAsync();
    Task<List<BookDto>> GetFilteredBooksAsync(BookFilterDto filter);
    Task<BookWithDescriptionDto> GetBookAsync(int id);
    Task<BookWithGenresDto> GetBooksWithGenresAsync();
    Task<BookStockAndSalesDto> GetBookStockAsync(int id);
    Task UpdateBookStockAsync(int id, int stock);
    Task AddNewBookAsync(BookDto bookDto);
    Task UpdateBookAsync(BookDto bookDto);
    Task<bool> HasUserPurchasedBookAsync(int bookId);
    Task ReviewBookAsync(int bookId, int rating, string comment);
    Task<BookWithReviewsDto> GetBookWithReviewsAsync(int bookId);
    Task<BookReviewDto> GetBookReviewOfCurrentUserAsync(int bookId);
}