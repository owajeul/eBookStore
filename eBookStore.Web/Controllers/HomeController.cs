using System.Diagnostics;
using eBookStore.Application.Services.Implementations;
using eBookStore.Web.Models;
using eBookStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.Controllers;
public class HomeController : Controller
{
    private readonly BookService _bookService;
    public HomeController(BookService bookService)
    {
        _bookService = bookService;
    }
    public async Task<IActionResult> Index(string? genre)
    {
        var books = await _bookService.GetAllBooksAsync(genre);
        var bookGenres = await _bookService.GetAllGenresAsync();
        var homeVM = new HomeVM
        {
            Books = books,
            BookGenres = bookGenres,
            SelectedGenre = genre
        };
        return View(homeVM);
    }
    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
