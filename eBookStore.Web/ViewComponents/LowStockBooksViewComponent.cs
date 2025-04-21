using eBookStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.ViewComponents
{
    public class LowStockBooksViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<BookVM> lowStockBooks)
        {
            return View(lowStockBooks);
        }
    }
}
