using System.Diagnostics;
using eBookStore.Application.Common.Interfaces;
using eBookStore.Web.Models;
using eBookStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.Controllers;
public class HomeController : Controller
{
    private readonly IBookService _bookService;
    public HomeController(IBookService bookService)
    {
        _bookService = bookService;
    }
    public async Task<IActionResult> Index(string? category)
    {
        var books = await _bookService.GetAllBooksAsync(category);
        var bookCategories = await _bookService.GetAllCategoriesAsync();
        var homeVM = new HomeVM
        {
            Books = books,
            BookCategories = bookCategories,
            SelectedCategory = category
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
