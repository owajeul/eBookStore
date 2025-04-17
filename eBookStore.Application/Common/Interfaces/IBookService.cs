using eBookStore.Domain.Entities;

namespace eBookStore.Application.Common.Interfaces;

public interface IBookService
{
    Task<List<Book>> GetAllBooksAsync(string? category = null);
    Task<IEnumerable<string>> GetAllCategoriesAsync();
    Task<Book> GetBookAsync(int id);
}