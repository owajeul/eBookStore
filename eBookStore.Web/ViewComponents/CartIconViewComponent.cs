using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.Components
{
    public class CartIconViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}
