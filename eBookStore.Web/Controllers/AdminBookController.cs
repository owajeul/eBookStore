﻿using AutoMapper;
using eBookStore.Application.DTOs;
using eBookStore.Application.Interfaces;
using eBookStore.Infrastructure.Repositories;
using eBookStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.Controllers;
public class AdminBookController : Controller
{
    private readonly IBookService _bookService;
    private readonly IMapper _mapper;

    public AdminBookController(IBookRepository bookRepository, IBookService bookService, IMapper mapper)
    {
        _bookService = bookService;
        _mapper = mapper;
    }
    public async Task<IActionResult> Index()
    {
        var books = await _bookService.GetAllBooksAsync();
        var booksViewModel = _mapper.Map<List<BookVM>>(books);
        return View(booksViewModel);
    }
    public async Task<IActionResult> Details(int id)
    {
        var book = await _bookService.GetBookAsync(id);
        var bookViewModel = _mapper.Map<BookVM>(book);
        return View(bookViewModel);
    }
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(BookVM book)
    {
        if (ModelState.IsValid)
        {
            var bookDto = _mapper.Map<BookWithDescriptionDto>(book);
            await _bookService.AddNewBookAsync(bookDto);
            TempData["ToastrMessage"] = "Book created successfully.";
            TempData["ToastrType"] = "success";
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _bookService.GetBookAsync(id);
        var bookViewModel = _mapper.Map<BookVM>(book);  
        return View(bookViewModel);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(BookVM book)
    {
        if (ModelState.IsValid)
        {
            var bookDto = _mapper.Map<BookWithDescriptionDto>(book);
            await _bookService.UpdateBookAsync(bookDto);
            TempData["ToastrMessage"] = "Book updated successfully.";
            TempData["ToastrType"] = "success";
            return RedirectToAction("Details", "Book", new {id = book.Id});
        }
        return View(book);
    }

    [HttpPost]
    public async Task<IActionResult> RestockBook(int id, int quantity)
    {
        if (quantity <= 0)
        {
            TempData["ToastrMessage"] = "Invalid quantity!";
            TempData["ToastrType"] = "error";
            return RedirectToAction("Index", "Dashboard");
        }

        await _bookService.UpdateBookStockAsync(id, quantity);

        TempData["ToastrMessage"] = "Book restocked successfully.";
        TempData["ToastrType"] = "success";

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    public async Task<IActionResult> UpdateBookStock(int id, int stock)
    {
        await _bookService.UpdateBookStockAsync(id, stock);
        return Ok(new {message = "Stock updated successfully", status = "success"});
    }
}