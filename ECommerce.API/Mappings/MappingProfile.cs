using AutoMapper;
using ECommerce.API.DTOs;
using ECommerce.API.Models;
using InventoryAPI.Models;

namespace ECommerce.API.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductResponseDto>()
                .ForMember(
                    dest => dest.CategoryName,
                    opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : null))
                .ForMember(
                    dest => dest.AverageRating,
                    opt => opt.MapFrom(src => src.Reviews.Any() ? src.Reviews.Average(r => r.Rating) : 0))
                .ForMember(
                    dest => dest.ReviewCount,
                    opt => opt.MapFrom(src => src.Reviews.Count));

            CreateMap<ProductCreateDto, Product>();
            CreateMap<ProductUpdateDto, Product>();

            CreateMap<Category, CategoryResponseDto>();
            CreateMap<CategoryCreateDto, Category>();
            CreateMap<CategoryUpdateDto, Category>();

            CreateMap<OrderCreateDto, Order>();
            CreateMap<OrderItemCreateDto, OrderItem>();

            CreateMap<OrderItem, OrderItemResponseDto>()
                .ForMember(
                    dest => dest.ProductName,
                    opt => opt.MapFrom(src => src.Product.Name))
                .ForMember(
                    dest => dest.ProductImageUrl,
                    opt => opt.MapFrom(src => src.Product.ImageUrl))
                .ForMember(
                    dest => dest.Subtotal,
                    opt => opt.MapFrom(src => src.Price * src.Quantity));

            CreateMap<Order, OrderResponseDto>()
                .ForMember(
                    dest => dest.Items,
                    opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(
                    dest => dest.CustomerName,
                    opt => opt.MapFrom(src => src.User != null ? src.User.FullName : "Guest"));

            CreateMap<ReviewCreateDto, Review>();
            CreateMap<Review, ReviewResponseDto>();
        }
    }
}
