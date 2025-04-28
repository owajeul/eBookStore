using AutoMapper;
using eBookStore.Application.Interfaces;
using eBookStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.ViewComponents
{
    public class AdminBookControlViewComponent: ViewComponent
    {
        private readonly IBookService _bookService;
        private readonly IMapper _mapper;

        public AdminBookControlViewComponent(IBookService bookService, IMapper mapper)
        {
            _bookService = bookService;
            _mapper = mapper;
        }
        public async Task<IViewComponentResult> InvokeAsync(int bookId)
        {
            var bookStock = await _bookService.GetBookStockAsync(bookId);
            var bookStockViewModel = _mapper.Map<BookStockAndSalesVM>(bookStock);
            return View(bookStockViewModel);
        }
    }
}
