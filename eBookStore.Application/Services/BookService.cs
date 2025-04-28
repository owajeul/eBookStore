using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using eBookStore.Application.Common.Utilily;
using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;
using eBookStore.Domain.Entities;

namespace eBookStore.Infrastructure.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;

    public BookService(IBookRepository bookRepository, IMapper mapper)
    {
        _bookRepository = bookRepository;
        _mapper = mapper;
    }

    public async Task<List<BookDto>> GetAllBooksAsync()
    {
        var books =  await _bookRepository.GetAllAsync();
        return _mapper.Map<List<BookDto>>(books);
    }

    public async Task<IEnumerable<string>> GetAllGenresAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        return books.Select(b => b.Genre).Distinct().ToList();
    }

    public async Task<BookWithDescriptionDto> GetBookAsync(int id)
    {
        var book =  await _bookRepository.Get(b => b.Id == id);
        return _mapper.Map<BookWithDescriptionDto>(book);
    }

    public async Task<BookWithGenresDto> GetBooksWithGenresAsync()
    {
        var books = await _bookRepository.GetAllAsync();
        var genres = books.Select(b => b.Genre).Distinct().ToList();
        return new BookWithGenresDto
        {
            Books = _mapper.Map<List<BookDto>>(books),
            Genres = genres
        };
    }
    public async Task<List<BookDto>> GetFilteredBooksAsync(BookFilterDto filter)
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
        return _mapper.Map<List<BookDto>>(books);
    }

    public async Task<BookStockAndSalesDto> GetBookStockAsync(int id)
    {
        var book = await _bookRepository.Get(b => b.Id == id);
        return new BookStockAndSalesDto
        {
            Id = book.Id,
            Stock = book.Stock
        };
    }

    public async Task UpdateBookStockAsync(int id, int stock)
    {
        var book = await _bookRepository.Get(b => b.Id == id);
        book.Stock = stock;
        _bookRepository.Update(book);
        await _bookRepository.Save();
    }

    public async Task AddNewBookAsync(BookDto bookDto)
    {
        var book = _mapper.Map<Book>(bookDto);
        await _bookRepository.Add(book);
        await _bookRepository.Save();
    }

    public async Task UpdateBookAsync(BookDto bookDto)
    {
        var book = _mapper.Map<Book>(bookDto);
        _bookRepository.Update(book);
        await _bookRepository.Save();
    }
}
