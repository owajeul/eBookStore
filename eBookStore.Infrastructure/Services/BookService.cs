using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eBookStore.Application.Common.Interfaces;
using eBookStore.Domain.Entities;

namespace eBookStore.Infrastructure.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<List<Book>> GetAllBooksAsync(string? category = null)
    {
        if (!string.IsNullOrEmpty(category))
        {
            return await _bookRepository.GetAllAsync(b => b.Genre.ToLower() == category.ToLower());
        }
        return await _bookRepository.GetAllAsync();
    }

    public async Task<IEnumerable<string>> GetAllGenresAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        return books.Select(b => b.Genre).Distinct().ToList();
    }

    public async Task<Book> GetBookAsync(int id)
    {
        return await _bookRepository.Get(b => b.Id == id);
    }
}
