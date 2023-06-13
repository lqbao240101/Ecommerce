using AutoMapper;
using Ecommerce.Data.Entities;
using Ecommerce.ViewModels.Addresses;
using Ecommerce.ViewModels.CartItem;
using Ecommerce.ViewModels.Comments;
using Ecommerce.ViewModels.Products;
using Ecommerce.ViewModels.ViewModels.MapperModel;

namespace Ecommerce.Services.Mapper
{
    public class AutoMapping : Profile
    {
        public AutoMapping()
        {
            CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<decimal?, decimal>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<double?, double>().ConvertUsing((src, dest) => src ?? dest);

            CreateMap<ProductAddModel, Product>();
            CreateMap<ProductUpdateModel, Product>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            
            CreateMap<AddressAddModel, Address>();
            CreateMap<AddressUpdateModel, Address>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CommentAddModel, Comment>();
            CreateMap<CommentUpdateModel, Comment>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<CartItemAddModel, CartItem>();
            CreateMap<CartItemUpdateModel, Comment>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<Order, OrderPost>().ReverseMap();
            CreateMap<Order, OrderView>().ReverseMap();
            CreateMap<ApplicationUser, UserCommentView>();
            CreateMap<Comment, CommentView>();
            CreateMap<Review, ReviewView>();
            CreateMap<Product, ProductGetAllView>();
            CreateMap<ApplicationUser, GetAllUsersByAdminView>();
            CreateMap<ApplicationUser, GetUserByIdView>();
            CreateMap<Product, ProductOrderView>();
            CreateMap<OrderDetail, OrderDetailView>();
        }
    }
}
