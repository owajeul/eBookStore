using eBookStore.Application.Common.Dto;
using eBookStore.Domain.Entities;

namespace eBookStore.Application.Common.Interfaces;

public interface IBookService
{
    Task<List<Book>> GetAllBooksAsync(string? category = null);
    Task<List<Book>> GetFilteredBooksAsync(BookFilterDto filter);
    Task<IEnumerable<string>> GetAllGenresAsync();
    Task<Book> GetBookAsync(int id);
}