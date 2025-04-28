using System.Diagnostics;
using AutoMapper;
using eBookStore.Application.Common.Exceptions;
using eBookStore.Application.Interfaces;
using eBookStore.Web.Constants;
using eBookStore.Web.Models;
using eBookStore.Web.ViewModels;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.Controllers;
public class HomeController : Controller
{
    private readonly IBookService _bookService;
    private readonly IMapper _mapper;

    public HomeController(IBookService bookService, IMapper mapper)
    {
        _bookService = bookService;
        _mapper = mapper;
    }
    public IActionResult Index()
    {
        return View();
    }
    public async Task<IActionResult> GetHomePageData()
    {
        var booksWithGenres = await _bookService.GetBooksWithGenresAsync();
        return Ok(booksWithGenres);
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        var exception = HttpContext.Features.Get<IExceptionHandlerFeature>()?.Error;

        var model = new ErrorViewModel
        {
            StatusCode = 500,
            Title = "Oops! Something went wrong.",
            Message = UserFriendlyErrorMessages.GeneralError,
            Details = exception?.Message
        };
        if(exception is BookNotFoundException)
        {
            model.StatusCode = 404;
            model.Title = "Book Not Found";
            model.Message = UserFriendlyErrorMessages.BookNotFoundError;
        }
        else if (exception is OrderNotFoundException)
        {
            model.StatusCode = 404;
            model.Title = "Order Not Found";
            model.Message = UserFriendlyErrorMessages.OrderNotFoundError;
        }
        else if (exception is ArgumentException)
        {
            model.StatusCode = 400;
            model.Title = "Invalid Request";
            model.Message = UserFriendlyErrorMessages.ArgumentError;
        }
        else if (exception is BookServiceException || exception is CartServiceException || exception is OrderServiceException)
        {
            model.Message = UserFriendlyErrorMessages.GeneralError;
        }


        return View("Error", model);
    }

}

