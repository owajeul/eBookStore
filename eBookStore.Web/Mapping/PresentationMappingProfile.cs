using AutoMapper;
using eBookStore.Application.Common.Dto;
using eBookStore.Web.Models;
using eBookStore.Web.ViewModels;

namespace eBookStore.Web.Mapping
{
    public class PresentationMappingProfile: Profile
    {
        public PresentationMappingProfile()
        {
            CreateMap<AdminDashboardDto, AdminDashboardVM>();
            CreateMap<TopSellingBookDto, TopSellingBookVM>();
            CreateMap<BookDto, BookVM>();
            CreateMap<BookFilterModel, BookFilterDto>();
            CreateMap<BookStockAndSalesDto, BookStockAndSalesVM>();
        }
    }
}
