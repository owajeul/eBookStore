using AutoMapper;
using eBookStore.Application.Interfaces;
using eBookStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.ViewComponents
{
    public class LowStockBooksViewComponent: ViewComponent
    {
        private readonly IReportService _reportService;
        private readonly IMapper _mapper;
        private const int LOW_STOCK_THRESHOLD = 5;
        private const int MAX_RECORDS_FOR_DASHBOARD = 10;
        public LowStockBooksViewComponent(IReportService reportService,IMapper mapper)
        {
            _reportService = reportService;
            _mapper = mapper;
        }
        public async Task<IViewComponentResult> InvokeAsync(bool showAll = false)
        {
            var lowStockBooks = await _reportService.GetLowStockBooksAsync(
                LOW_STOCK_THRESHOLD, 
                showAll ? null : MAX_RECORDS_FOR_DASHBOARD
                );
            var lowStockBooksViewModel = _mapper.Map<List<BookVM>>(lowStockBooks);
            return View(lowStockBooksViewModel);
        }
    }
}
