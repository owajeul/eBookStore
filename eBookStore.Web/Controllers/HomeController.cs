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
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
