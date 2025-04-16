using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using eBookStore.Domain.Entities;
using eBookStore.Infrastructure.Repositories;

namespace eBookStore.Application.Services.Implementations;

public class BookService
{
    private readonly BookRepository _bookRepository;
    public BookService(BookRepository bookRepository)
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
}
