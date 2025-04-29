using AutoMapper;
using eBookStore.Application.DTOs;
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
            CreateMap<BookWithDescriptionDto, BookVM>();
            CreateMap<BookFilterModel, BookFilterDto>();
            CreateMap<BookStockAndSalesDto, BookStockAndSalesVM>();
            CreateMap<ProfileDto, ProfileVM>();
            CreateMap<UserDto, UserVM>();
            CreateMap<CartDto, CartVM>();
            CreateMap<CartVM, CartDto>();
            CreateMap<CartItemDto, CartItemVM>();
            CreateMap<CartItemVM, CartItemDto>();
            CreateMap<BookDto, BookVM>();
            CreateMap<BookVM, BookDto>();
            CreateMap<OrderDto, OrderVM>();
            CreateMap<OrderVM, OrderVM>();
            CreateMap<OrderItemDto, OrderItemVM>();
            CreateMap<OrderItemVM, OrderItemDto>();
            CreateMap<BookWithReviewsDto, BookWithReviewsVM>();
            CreateMap<BookReviewDto, BookReviewVM>();

        }
    }
}
