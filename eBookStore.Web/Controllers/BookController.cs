
using eBookStore.Application.Common.Interfaces;
using Microsoft.AspNetCore.Mvc;
namespace eBookStore.Web.Controllers;

public class BookController : Controller
{
    private readonly IBookService _bookService;

    public BookController(IBookService bookService)
    {
        _bookService = bookService;
    }
    public async Task<IActionResult> Details(int id)
    {
        var book = await _bookService.GetBookAsync(id);
        return View(book);
    }
}
