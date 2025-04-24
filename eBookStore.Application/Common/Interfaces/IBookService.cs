using eBookStore.Application.Common.Dto;
using eBookStore.Domain.Entities;

namespace eBookStore.Application.Common.Interfaces;

public interface IBookService
{
    Task<List<Book>> GetAllBooksAsync();
    Task<List<Book>> GetFilteredBooksAsync(BookFilterDto filter);
    Task<Book> GetBookAsync(int id);
    Task<BookWithGenresDto> GetBooksWithGenresAsync();
    Task<BookStockAndSalesDto> GetBookStockAsync(int id);
    Task UpdateBookStockAsync(int id, int stock);
}