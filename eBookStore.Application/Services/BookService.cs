using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using eBookStore.Application.Common.Exceptions;
using eBookStore.Application.Common.Utilily;
using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;
using eBookStore.Domain.Entities;

namespace eBookStore.Infrastructure.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public BookService(IBookRepository bookRepository, IMapper mapper, IUserService userService)
    {
        _bookRepository = bookRepository;
        _mapper = mapper;
        _userService = userService;
    }

    public async Task<List<BookDto>> GetAllBooksAsync()
    {
        try
        {
            return await FetchAllBooksAsync();
        }
        catch (Exception ex)
        {
            throw new BookServiceException("Failed to retrieve all books", ex);
        }
    }

    public async Task<IEnumerable<string>> GetAllGenresAsync()
    {
        try
        {
            return await FetchAllGenresAsync();
        }
        catch (Exception ex)
        {
            throw new BookServiceException("Failed to retrieve book genres", ex);
        }
    }

    public async Task<BookWithDescriptionDto> GetBookAsync(int id)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("Book ID must be greater than zero", nameof(id));

            return await FetchBookByIdAsync(id);
        }
        catch (BookNotFoundException)
        {
            throw;
        }
        catch (Exception ex) when (!(ex is BookServiceException))
        {
            throw new BookServiceException($"Failed to retrieve book with ID {id}", ex);
        }
    }

    public async Task<BookWithGenresDto> GetBooksWithGenresAsync()
    {
        try
        {
            return await FetchBooksWithGenresAsync();
        }
        catch (Exception ex)
        {
            throw new BookServiceException("Failed to retrieve books with genres", ex);
        }
    }

    public async Task<BookReviewDto> GetBookReviewOfCurrentUserAsync(int bookId)
    {
        try
        {
            if (bookId <= 0)
                throw new ArgumentException("Book ID must be greater than zero", nameof(bookId));
            var bookReview = await _bookRepository.GetBookReviewAsync(bookId, _userService.GetUserId());
            if (bookReview == null)
                throw new BookNotFoundException($"No review found for book with ID {bookId}");
            return _mapper.Map<BookReviewDto>(bookReview);
        }
        catch (Exception ex) when (!(ex is BookServiceException))
        {
            throw new BookServiceException($"Failed to retrieve review for book with ID {bookId}", ex);
        }
    }

    public async Task<BookWithReviewsDto> GetBookWithReviewsAsync(int bookId)
    {
        try
        {
           return await FetchBookWithReviewsAsync(bookId);
        }
        catch (Exception ex) when (!(ex is BookServiceException))
        {
            throw new BookServiceException($"Failed to retrieve book with ID {bookId}", ex);
        }
    }
    public async Task<List<BookDto>> GetFilteredBooksAsync(BookFilterDto filter)
    {
        try
        {
            if (filter == null)
                throw new ArgumentNullException(nameof(filter));

            return await FilterBooksAsync(filter);
        }
        catch (Exception ex) when (!(ex is BookServiceException))
        {
            throw new BookServiceException("Failed to retrieve filtered books", ex);
        }
    }

    public async Task<BookStockAndSalesDto> GetBookStockAsync(int id)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("Book ID must be greater than zero", nameof(id));

            return await FetchBookStockAsync(id);
        }
        catch (Exception ex) when (!(ex is BookServiceException))
        {
            throw new BookServiceException($"Failed to retrieve stock for book with ID {id}", ex);
        }
    }

    public async Task UpdateBookStockAsync(int id, int stock)
    {
        try
        {
            if (id <= 0)
                throw new ArgumentException("Book ID must be greater than zero", nameof(id));

            if (stock < 0)
                throw new ArgumentException("Stock value cannot be negative", nameof(stock));

            await UpdateStockForBookAsync(id, stock);
        }
        catch (Exception ex) when (!(ex is BookServiceException))
        {
            throw new BookServiceException($"Failed to update stock for book with ID {id}", ex);
        }
    }

    public async Task AddNewBookAsync(BookDto bookDto)
    {
        try
        {
            if (bookDto == null)
                throw new ArgumentNullException(nameof(bookDto));

            await AddBookAsync(bookDto);
        }
        catch (Exception ex) when (!(ex is BookServiceException))
        {
            throw new BookServiceException("Failed to add new book", ex);
        }
    }

    public async Task UpdateBookAsync(BookDto bookDto)
    {
        try
        {
            if (bookDto == null)
                throw new ArgumentNullException(nameof(bookDto));

            if (bookDto.Id <= 0)
                throw new ArgumentException("Book ID must be greater than zero", nameof(bookDto.Id));

            await UpdateExistingBookAsync(bookDto);
        }
        catch (Exception ex) when (!(ex is BookServiceException))
        {
            throw new BookServiceException($"Failed to update book with ID {bookDto?.Id}", ex);
        }
    }

    public async Task<bool> HasUserPurchasedBookAsync(int bookId)
    {
        return await _bookRepository.HasUserPurchasedBookAsync(bookId, _userService.GetUserId());
    }
    public async Task ReviewBookAsync(int bookId, int rating, string comment)
    {
        try
        {
           await AddBookReviewOfUserAsync(bookId, rating, comment);
           await _bookRepository.Save();
        }
        catch(Exception ex) when (!(ex is BookServiceException))
        {
            throw new BookServiceException($"Failed to review book with ID {bookId}", ex);
        }
    }   

    private async Task<List<BookDto>> FetchAllBooksAsync()
    {
        var books = await _bookRepository.GetAllAsync();

        if (books == null)
            return new List<BookDto>();

        return _mapper.Map<List<BookDto>>(books);
    }

    private async Task<IEnumerable<string>> FetchAllGenresAsync()
    {
        var books = await _bookRepository.GetAllAsync();

        if (books == null)
            return new List<string>();

        return books.Select(b => b.Genre).Distinct().ToList();
    }

    private async Task<BookWithDescriptionDto> FetchBookByIdAsync(int id)
    {
        var book = await _bookRepository.Get(b => b.Id == id);

        if (book == null)
            throw new BookNotFoundException($"Book with ID {id} not found");

        return _mapper.Map<BookWithDescriptionDto>(book);
    }

    private async Task<BookWithGenresDto> FetchBooksWithGenresAsync()
    {
        var books = await _bookRepository.GetAllAsync();

        if (books == null)
            books = new List<Book>();

        var genres = books.Select(b => b.Genre).Distinct().ToList();

        return new BookWithGenresDto
        {
            Books = _mapper.Map<List<BookDto>>(books),
            Genres = genres
        };
    }

    private async Task<List<BookDto>> FilterBooksAsync(BookFilterDto filter)
    {
        var books = await _bookRepository.GetAllAsync();

        if (books == null)
            return new List<BookDto>();

        if (!string.IsNullOrEmpty(filter.Genre))
        {
            books = books.Where(b => b.Genre.ToLower() == filter.Genre.ToLower()).ToList();
        }

        if (filter.MaxPrice.HasValue)
        {
            books = books.Where(b => b.Price <= filter.MaxPrice.Value).ToList();
        }

        if (!string.IsNullOrEmpty(filter.SortBy))
        {
            if (filter.SortBy.ToLower() == AppConstant.SortByPriceAsc.ToLower())
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

    private async Task<BookStockAndSalesDto> FetchBookStockAsync(int id)
    {
        var book = await _bookRepository.Get(b => b.Id == id);

        if (book == null)
            throw new BookNotFoundException($"Book with ID {id} not found");

        return new BookStockAndSalesDto
        {
            Id = book.Id,
            Stock = book.Stock
        };
    }

    private async Task UpdateStockForBookAsync(int id, int stock)
    {
        var book = await _bookRepository.Get(b => b.Id == id);

        if (book == null)
            throw new BookNotFoundException($"Book with ID {id} not found");

        book.Stock = stock;
        _bookRepository.Update(book);
        await _bookRepository.Save();
    }

    private async Task AddBookAsync(BookDto bookDto)
    {
        ValidateBookData(bookDto);

        var book = _mapper.Map<Book>(bookDto);
        await _bookRepository.Add(book);
        await _bookRepository.Save();
    }

    private async Task UpdateExistingBookAsync(BookDto bookDto)
    {
        ValidateBookData(bookDto);

        var existingBook = await _bookRepository.Get(b => b.Id == bookDto.Id);
        if (existingBook == null)
            throw new BookNotFoundException($"Book with ID {bookDto.Id} not found");

        var book = _mapper.Map<Book>(bookDto);
        _bookRepository.Update(book);
        await _bookRepository.Save();
    }

    private void ValidateBookData(BookDto bookDto)
    {
        if (string.IsNullOrWhiteSpace(bookDto.Title))
            throw new ArgumentException("Book title cannot be empty", nameof(bookDto.Title));

        if (string.IsNullOrWhiteSpace(bookDto.Author))
            throw new ArgumentException("Book author cannot be empty", nameof(bookDto.Author));

        if (bookDto.Price < 0)
            throw new ArgumentException("Book price cannot be negative", nameof(bookDto.Price));
    }

    private async Task AddBookReviewOfUserAsync(int bookId, int rating, string comment)
    {
        if(bookId <= 0)
        {
            throw new ArgumentException("Book ID must be greater than zero");
        }
        if (rating < 1 || rating > 5)
        {
            throw new ArgumentException("Rating must be between 1 and 5");
        }
        if (string.IsNullOrEmpty(comment))
        {
            throw new ArgumentException("Comment cannot be empty");
        }
        var user = await _userService.GetUserAsync();
        var review = await _bookRepository.GetBookReviewAsync(bookId, user.UserId);
        if (review != null)
        {
            throw new BookServiceException($"User {user.UserId} has not already reviewed the book");
        }
        var bookReview = new BookReview
        {
            BookId = bookId,
            UserId = user.UserId,
            UserName = user.Name,
            Rating = rating,
            Comment = comment ?? string.Empty,
            CreatedAt = DateTime.UtcNow
        };

        await _bookRepository.AddBookReviewAsync(bookReview);
    }

    private async Task<BookWithReviewsDto> FetchBookWithReviewsAsync(int bookId)
    {
        if (bookId <= 0)
            throw new ArgumentException("Book ID must be greater than zero", nameof(bookId));
        var book = await _bookRepository.GetBookWithReviewsAsync(bookId);
        if (book == null)
            throw new BookNotFoundException($"Book with ID {bookId} not found");
        var bookDto = _mapper.Map<BookWithReviewsDto>(book);
        double averageRating = book.Reviews.Any() ? book.Reviews.Average(r => r.Rating): 0;
        int totalReviews = book.Reviews.Count;
        bookDto.AverageRating = averageRating;
        bookDto.ReviewCount = totalReviews;
        return bookDto;
    }

}