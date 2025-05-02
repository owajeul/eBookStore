using eBookStore.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.ViewComponents;

public class UserPurchaseReportViewComponent : ViewComponent
{
    private readonly IReportService _reportService;

    public UserPurchaseReportViewComponent(IReportService reportService)
    {
        _reportService = reportService;
    }

    public async Task<IViewComponentResult> InvokeAsync(bool showAll = false)
    {
        var reports = await _reportService.GetUserPurchaseReportsAsync();

        if (!showAll)
        {
            reports = reports.Take(10).ToList();
        }
        ViewBag.ShowAll = showAll;
        return View(reports);
    }
}
