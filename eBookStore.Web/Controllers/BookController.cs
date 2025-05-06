using AutoMapper;
using eBookStore.Application.Common.Exceptions;
using eBookStore.Application.Interfaces;
using eBookStore.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
namespace eBookStore.Web.Controllers;

public class BookController : Controller
{
    private readonly IBookService _bookService;
    private readonly IMapper _mapper;

    public BookController(IBookService bookService, IMapper mapper)
    {
        _bookService = bookService;
        _mapper = mapper;
    }
    public async Task<IActionResult> Details(int id)
    {
        var bookDto = await _bookService.GetBookWithReviewsAsync(id);
        var bookViewModel = _mapper.Map<BookWithReviewsVM>(bookDto);
        return View(bookViewModel);
    }
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AddReview(int bookId, int rating, string comment)
    {
       var userPurchasedTheBook = await _bookService.HasUserPurchasedBookAsync(bookId);
        if (!userPurchasedTheBook)
        {
            return BadRequest("You must purchase the book before reviewing it.");
        }
        try
        {
            await _bookService.ReviewBookAsync(bookId, rating, comment);
        }
        catch(UserCanProvideOnlyOneBookReviewException ex)
        {
            return BadRequest("You can provide only one review in a book.");
        }
        var review = await _bookService.GetBookReviewOfCurrentUserAsync(bookId);
       return Ok(review);
    }
}
