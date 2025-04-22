using AutoMapper;
using eBookStore.Application.Common.Dto;
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
        }
    }
}
