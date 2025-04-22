using System.Diagnostics;
using AutoMapper;
using eBookStore.Application.Common.Dto;
using eBookStore.Application.Common.Interfaces;
using eBookStore.Domain.Entities;
using eBookStore.Web.Models;
using eBookStore.Web.ViewModels;
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
    public async Task<IActionResult> GetHomePageData(BookFilterModel filter)
    {
        var bookFilterDto = _mapper.Map<BookFilterDto>(filter);
        var books = await _bookService.GetFilteredBooksAsync(bookFilterDto);
        var bookGenres = await _bookService.GetAllGenresAsync();
        var homeVM = new HomeVM
        {
            Books = books,
            BookGenres = bookGenres,
            SelectedGenre = filter.Genre,
            MaxPrice = filter.MaxPrice,
        };
        return Ok(homeVM);
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
