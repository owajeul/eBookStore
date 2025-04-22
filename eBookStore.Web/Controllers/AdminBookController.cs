using eBookStore.Application.Common.Interfaces;
using eBookStore.Domain.Entities;
using eBookStore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.Controllers;
public class AdminBookController : Controller
{
    private readonly IBookRepository _bookRepository;
    public AdminBookController(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    public async Task<IActionResult> Index()
    {
        var books = await _bookRepository.GetAllAsync(b => b.Price > 12);
        return View(books);
    }

    public async Task<IActionResult> Details(int id)
    {
        var book = await _bookRepository.Get(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }
    public IActionResult Create()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Book book)
    {
        if (ModelState.IsValid)
        {
            await _bookRepository.Add(book);
            await _bookRepository.Save();
            TempData["ToastrMessage"] = "Book created successfully.";
            TempData["ToastrType"] = "success";
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _bookRepository.Get(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Book book)
    {
        if (ModelState.IsValid)
        {
            _bookRepository.Update(book);
            await _bookRepository.Save();
            TempData["ToastrMessage"] = "Book updated successfully.";
            TempData["ToastrType"] = "success";
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }

    [HttpPost]
    public async Task<IActionResult> RestockBook(int id, int quantityToAdd)
    {
        if (quantityToAdd <= 0)
        {
            TempData["ToastrMessage"] = "Invalid quantity!";
            TempData["ToastrType"] = "error";
            return RedirectToAction("Index", "Dashboard");
        }
        var book = await _bookRepository.Get(b => b.Id == id);

        if (book == null)
        {
            return NotFound();
        }
        book.Stock += quantityToAdd;
        _bookRepository.Update(book);
        await _bookRepository.Save();

        TempData["ToastrMessage"] = "Book restocked successfully.";
        TempData["ToastrType"] = "success";

        return RedirectToAction("Index", "Dashboard");
    }

}