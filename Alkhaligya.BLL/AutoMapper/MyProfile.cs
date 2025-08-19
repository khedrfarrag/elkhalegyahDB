using Alkhaligya.BLL.Dtos.Auth;
using Alkhaligya.BLL.Dtos.Cart;
using Alkhaligya.BLL.Dtos.CategoryDtos;
using Alkhaligya.BLL.Dtos.Contact;
using Alkhaligya.BLL.Dtos.Order;
using Alkhaligya.BLL.Dtos.ProductDtos;
using Alkhaligya.BLL.Dtos.ProductFeedbackDto;
using Alkhaligya.BLL.Dtos.SiteFeedbackDtos;
using Alkhaligya.DAL.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Alkhaligya.BLL.Dtos.Order.AddOrderDto;

namespace Alkhaligya.BLL.AutoMapper
{
    public class MyProfile : Profile
    {
        public MyProfile()
        {

            //CreateMap<Order , ReadOrderDto>().ReverseMap();
            CreateMap<Order, AddOrderDto>().ReverseMap();
            CreateMap<Order, UpdateOrderDto>().ReverseMap();
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<Order, ReadOrderDto>()

    .ForMember(dest => dest.TotalQuantity, opt =>
        opt.MapFrom(src => src.OrderItems.Sum(item => item.Quantity)))
    .ForMember(dest => dest.TotalPrice, opt =>
        opt.MapFrom(src => src.OrderItems.Sum(item => item.TotalPrice)))
    .ForMember(dest => dest.OrderItems, opt =>
        opt.MapFrom(src => src.OrderItems.Select(item => new OrderItemDto
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity,
            Price = item.Quantity == 0 ? 0 : item.TotalPrice / item.Quantity
        }).ToList()))
    .ReverseMap();







            // Category
            CreateMap<Category, CategoryReadDto>().ReverseMap();
            CreateMap<CategoryAddDto, Category>()
                .ForMember(dest => dest.SubCategories, opt => opt.MapFrom(src => src.SubCategories)).ReverseMap();
            CreateMap<CategoryUpdateDto, Category>().ReverseMap();

            //Subcategory
            CreateMap<SubCategory, SubCategoryReadDto>().ReverseMap();
            CreateMap<SubCategoryAddDto, SubCategory>().ReverseMap();
            CreateMap<SubCategoryUpdateDto, SubCategory>().ReverseMap();

            // Product
            CreateMap<Product, ProductReadDto>()
                .ForMember(dest => dest.ImageUrl, opt => opt.MapFrom(src => src.ImageUrl))
                .ForMember(dest => dest.StockStatues, opt => opt.MapFrom(src =>
                    src.StockQuantity > 0 ? "In Stock" : "Out of Stock"))
                .ForMember(dest => dest.SubCategoryId, opt => opt.MapFrom(src => src.SubCategory.Id))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Category.Id))
                .ForMember(dest => dest.DiscountedPrice,
                      opt => opt.MapFrom(src => CalculateDiscountedPrice(src)))
                .ForMember(dest => dest.ProductFeedbacks, opt => opt.MapFrom(src => src.ProductFeedbacks))
                .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate));

            CreateMap<ProductAddDto, Product>()
                .ForMember(dest => dest.SubCategoryId, opt => opt.MapFrom(src =>
                 src.SubCategoryId == 0 ? (int?)null : src.SubCategoryId))
                 .ReverseMap();

            CreateMap<ProductUpdateDto, Product>().ReverseMap();

            //product details
            //  CreateMap<ProductDetail, ProductDetailsReadDto>();
            //CreateMap<ProductDetailsAddDto, ProductDetail>();
            // CreateMap<ProductDetailsUpdateDto, ProductDetail>();

            // Product Feedback
            CreateMap<ProductFeedback, ProductFeedbackReadDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.User != null ? src.User.Id : src.UserId))
                .ForMember(dest => dest.FeedBackId, opt => opt.MapFrom(src => src.Id));


            CreateMap<ProductFeedbackAddDto, ProductFeedback>();
            CreateMap<ProductFeddbackUpdateDto, ProductFeedback>();






            //site feedback
            CreateMap<SiteFeedbackAddDto, SiteFeedback>();

            CreateMap<SiteFeedback, SiteFeedbackReadDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId));

            CreateMap<CartShop, ReadCartShopDto>().ReverseMap();
            CreateMap<CartItem, ReadCartItemDto>().ReverseMap();
            CreateMap<CartItem, UpdateCartItemDto>().ReverseMap();


            CreateMap<ContactMessage, ReadContactMessageDto>().ReverseMap();
            CreateMap<ContactMessage, CreateContactMessageDto>().ReverseMap();
            CreateMap<ContactMessage, UpdateContactMessageDto>().ReverseMap();


            CreateMap<ApplicationUser, UserReadDto>().ReverseMap();



        }

        private static decimal? CalculateDiscountedPrice(Product product)
        {
            if (product.DiscountPercentage.HasValue &&
                product.DiscountPercentage > 0 &&
                product.DiscountPercentage <= 100)
            {
                return product.Price * (1 - product.DiscountPercentage.Value / 100);
            }
            return null;
        }


    }
}
