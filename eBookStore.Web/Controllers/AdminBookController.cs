using eBookStore.Domain.Entities;
using eBookStore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.Controllers;
public class AdminBookController : Controller
{
    private readonly Repository<Book> _repository;
    public AdminBookController(Repository<Book> repository)
    {
        _repository = repository;
    }
    public async Task<IActionResult> Index()
    {
        var books = await _repository.GetAllAsync(b => b.Price > 12);
        return View(books);
    }

    public async Task<IActionResult> Details(int id)
    {
        var book = await _repository.Get(b => b.Id == id);
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
            await _repository.Add(book);
            await _repository.Save();
            TempData["ToastrMessage"] = "Book created successfully.";
            TempData["ToastrType"] = "success";
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }
    public async Task<IActionResult> Edit(int id)
    {
        var book = await _repository.Get(b => b.Id == id);
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
            _repository.Update(book);
            await _repository.Save();
            TempData["ToastrMessage"] = "Book updated successfully.";
            TempData["ToastrType"] = "success";
            return RedirectToAction(nameof(Index));
        }
        return View(book);
    }
    public async Task<IActionResult> Delete(int id)
    {
        var book = await _repository.Get(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }
        return View(book);
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var book = await _repository.Get(b => b.Id == id);
        if (book == null)
        {
            return NotFound();
        }
        _repository.Remove(book);
        await _repository.Save();
        TempData["ToastrMessage"] = "Book deleted successfully.";
        TempData["ToastrType"] = "success";
        return RedirectToAction(nameof(Index));
    }

}