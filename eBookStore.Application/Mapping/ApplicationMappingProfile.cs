using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using eBookStore.Application.DTOs;
using eBookStore.Domain.Entities;

namespace eBookStore.Infrastructure.Mapping
{
    public class ApplicationMappingProfile : Profile
    {
        public ApplicationMappingProfile()
        {
            CreateMap<Book, BookDto>();
            CreateMap<BookDto, Book>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());

            CreateMap<Book, BookWithDescriptionDto>();
            CreateMap<BookWithDescriptionDto, Book>();
            CreateMap<Order, OrderDto>();
            CreateMap<OrderDto, Order>();
            CreateMap<OrderItem, OrderItemDto>();

            CreateMap<OrderItemDto, OrderItem>()
                .ForMember(dest => dest.Book, opt => opt.Ignore());

            CreateMap<Cart, CartDto>();
            CreateMap<CartDto, Cart>();
            CreateMap<CartItem, CartItemDto>();
            CreateMap<CartItemDto, CartItem>();
            CreateMap<BookReview, BookReviewDto>();
            CreateMap<Book, BookWithReviewsDto>();
            CreateMap<AddressDto, Address>();
            CreateMap<Address, AddressDto>();

        }
    }
}
