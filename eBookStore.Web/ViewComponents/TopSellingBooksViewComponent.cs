using eBookStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.ViewComponents
{
    public class TopSellingBooksViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(List<TopSellingBookVM> topSellingBooks)
        {
            return View(topSellingBooks);
        }
    }
}
