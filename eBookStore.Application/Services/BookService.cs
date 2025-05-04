using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using eBookStore.Application.Common.Exceptions;
using eBookStore.Application.Common.Utilily;
using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;
using eBookStore.Application.Validators;
using eBookStore.Domain.Entities;

namespace eBookStore.Infrastructure.Services;

public class BookService : IBookService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserService _userService;

    public BookService(IUnitOfWork unitOfWork, IMapper mapper, IUserService userService)
    {
        _unitOfWork = unitOfWork;
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
            return await _unitOfWork.Book.GetDistinctGenresAsync();
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
            InputValidator.ValidateBookId(id);
            return await FetchBookByIdAsync(id);
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
            InputValidator.ValidateBookId(bookId);
            return await FetchBookReviewOfCurrentUser(bookId);
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
            InputValidator.ValidateBookId(bookId);
            return await FetchBookWithReviewsAsync(bookId);
        }
        catch (Exception ex) when (!(ex is BookServiceException))
        {
            throw new BookServiceException($"Failed to retrieve book with ID {bookId}", ex);
        }
    }
    
    public async Task<BookStockAndSalesDto> GetBookStockAsync(int bookId)
    {
        try
        {
            InputValidator.ValidateBookId(bookId);
            return await FetchBookStockAsync(bookId);
        }
        catch (Exception ex) when (!(ex is BookServiceException))
        {
            throw new BookServiceException($"Failed to retrieve stock for book with ID {bookId}", ex);
        }
    }

    public async Task UpdateBookStockAsync(int id, int stock)
    {
        try
        {
            InputValidator.ValidateBookId(id);
            InputValidator.ValidateStock(stock);
            await UpdateStockForBookAsync(id, stock);
            await _unitOfWork.SaveAsync();
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
            await AddBookAsync(bookDto);
            await _unitOfWork.SaveAsync();
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
            InputValidator.ValidateBookId(bookDto.Id);
            await UpdateExistingBookAsync(bookDto);
            await _unitOfWork.SaveAsync();
        }
        catch (Exception ex) when (!(ex is BookServiceException))
        {
            throw new BookServiceException($"Failed to update book with ID {bookDto?.Id}", ex);
        }
    }

    public async Task<bool> HasUserPurchasedBookAsync(int bookId)
    {
        try
        {
            InputValidator.ValidateBookId(bookId);
            return await _unitOfWork.Book.HasUserPurchasedBookAsync(bookId, _userService.GetUserId());
        }
        catch (Exception ex) when (!(ex is BookServiceException))
        {
            throw new BookServiceException($"Failed to check if user has purchased book with ID {bookId}", ex);
        }
    }
    public async Task ReviewBookAsync(int bookId, int rating, string comment)
    {
        try
        {
            InputValidator.ValidateBookId(bookId);
            InputValidator.ValidateRating(rating);
            InputValidator.ValidateComment(comment);
            await AddBookReviewOfUserAsync(bookId, rating, comment);
           await _unitOfWork.SaveAsync();
        }
        catch(UserCanProvideOnlyOneBookReviewException)
        {
            throw;
        }
        catch(Exception ex) when (!(ex is BookServiceException))
        {
            throw new BookServiceException($"Failed to review book with ID {bookId}", ex);
        }
    }   

    private async Task<List<BookDto>> FetchAllBooksAsync()
    {
        var books = await _unitOfWork.Book.GetAllAsync();

        if (books == null)
            return new List<BookDto>();

        return _mapper.Map<List<BookDto>>(books);
    }
    private async Task<BookWithDescriptionDto> FetchBookByIdAsync(int id)
    {
        var book = await _unitOfWork.Book.Get(b => b.Id == id);

        if (book == null)
            throw new BookNotFoundException($"Book with ID {id} not found");

        return _mapper.Map<BookWithDescriptionDto>(book);
    }

    private async Task<BookWithGenresDto> FetchBooksWithGenresAsync()
    {
        var books = await _unitOfWork.Book.GetAllAsync();

        if (books == null)
            books = new List<Book>();

        var genres = books.Select(b => b.Genre).Distinct().ToList();

        return new BookWithGenresDto
        {
            Books = _mapper.Map<List<BookDto>>(books),
            Genres = genres
        };
    }

    private async Task<BookReviewDto> FetchBookReviewOfCurrentUser(int bookId)
    {
        var bookReview = await _unitOfWork.Book.GetBookReviewAsync(bookId, _userService.GetUserId());
        if (bookReview == null)
            throw new BookNotFoundException($"No review found for book with ID {bookId}");
        return _mapper.Map<BookReviewDto>(bookReview);
    }
    private async Task<BookStockAndSalesDto> FetchBookStockAsync(int id)
    {
        var book = await _unitOfWork.Book.Get(b => b.Id == id);

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
        var book = await _unitOfWork.Book.Get(b => b.Id == id);

        if (book == null)
            throw new BookNotFoundException($"Book with ID {id} not found");

        book.Stock = stock;
        _unitOfWork.Book.Update(book);
    }

    private async Task AddBookAsync(BookDto bookDto)
    {
        ValidateBookData(bookDto);
        var book = _mapper.Map<Book>(bookDto);
        await _unitOfWork.Book.Add(book);
    }

    private async Task UpdateExistingBookAsync(BookDto bookDto)
    {
        ValidateBookData(bookDto);

        var existingBook = await _unitOfWork.Book.Get(b => b.Id == bookDto.Id);
        if (existingBook == null)
            throw new BookNotFoundException($"Book with ID {bookDto.Id} not found");

        var book = _mapper.Map<Book>(bookDto);
        _unitOfWork.Book.Update(book);
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
        var user = await _userService.GetUserAsync();
        var review = await _unitOfWork.Book.GetBookReviewAsync(bookId, user.UserId);
        if (review != null)
        {
            throw new UserCanProvideOnlyOneBookReviewException($"User with userId {user.UserId} already reviwed the book.");
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

        await _unitOfWork.Book.AddBookReviewAsync(bookReview);
    }

    private async Task<BookWithReviewsDto> FetchBookWithReviewsAsync(int bookId)
    {
        var book = await _unitOfWork.Book.GetBookWithReviewsAsync(bookId);
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