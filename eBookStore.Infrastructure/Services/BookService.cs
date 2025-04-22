using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using eBookStore.Application.Common.Dto;
using eBookStore.Application.Common.Interfaces;
using eBookStore.Application.Common.Utilily;
using eBookStore.Domain.Entities;

namespace eBookStore.Infrastructure.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    public BookService(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }

    public async Task<List<Book>> GetAllBooksAsync(string? genre = null)
    {
        if (!string.IsNullOrEmpty(genre))
        {
            return await _bookRepository.GetAllAsync(b => b.Genre.ToLower() == genre.ToLower());
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

    public async Task<List<Book>> GetFilteredBooksAsync(BookFilterDto filter)
    {
        var books = await _bookRepository.GetAllAsync();
        if (!string.IsNullOrEmpty(filter.Genre))
        {
            books = books.Where(b => b.Genre.ToLower() == filter.Genre.ToLower()).ToList();
        }
        if (filter.MaxPrice.HasValue)
        {
            books = books.Where(b => b.Price <= filter.MaxPrice.Value).ToList();
        }
        if(!string.IsNullOrEmpty(filter.SortBy))
        {
            if(filter.SortBy.ToLower() == AppConstant.SortByPriceAsc.ToLower())
            {
                books = books.OrderBy(b => b.Price).ToList();
            }
            else
            {
                books = books.OrderByDescending(b => b.Price).ToList();
            }
        }
        return books;
    }
}
