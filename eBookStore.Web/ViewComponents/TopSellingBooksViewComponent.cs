using AutoMapper;
using eBookStore.Application.Interfaces;
using eBookStore.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace eBookStore.Web.ViewComponents
{
    public class TopSellingBooksViewComponent: ViewComponent
    {
        private readonly IReportService _reportService;
        private readonly IMapper _mapper;
        private const int TOP_SELLING_BOOKS_COUNT = 100;
        public TopSellingBooksViewComponent(IReportService reportService, IMapper mapper)
        {
            _reportService = reportService;
            _mapper = mapper;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var topSellingBooks = await _reportService.GetTopSellingBooksAsync(TOP_SELLING_BOOKS_COUNT);
            var topSellingBooksViewModel = _mapper.Map<List<TopSellingBookVM>>(topSellingBooks);
            return View(topSellingBooksViewModel);
        }
    }
}
