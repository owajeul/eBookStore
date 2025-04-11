
using eBookStore.Application.Services.Implementations;
using Microsoft.AspNetCore.Mvc;
namespace eBookStore.Web.Controllers;

public class BookController : Controller
{
    private readonly BookService _bookService;

    public BookController(BookService bookService)
    {
        _bookService = bookService;
    }
    public async Task<IActionResult> Details(int id)
    {
        var book = await _bookService.GetBookAsync(id);
        return View(book);
    }
}
