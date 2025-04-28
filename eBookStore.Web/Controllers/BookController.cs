using AutoMapper;
using eBookStore.Application.Interfaces;
using eBookStore.Web.ViewModels;
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
        var bookDto = await _bookService.GetBookAsync(id);
        var bookViewModel = _mapper.Map<BookVM>(bookDto);
        return View(bookViewModel);
    }
}
