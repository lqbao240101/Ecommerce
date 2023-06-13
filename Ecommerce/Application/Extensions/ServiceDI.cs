using AutoMapper;
using Ecommerce.Data.GraphQL;
using Ecommerce.Services.Core.Authentication;
using Ecommerce.Services.Core.Catalog;
using Ecommerce.Services.Core.IService;
using Ecommerce.Services.Mapper;

namespace Ecommerce.Application.Extensions
{
    public static class ServiceDI
    {
        public static void AddDependencies(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ISMSService, SMSService>();
            services.AddTransient<IUserService, UserService>();
            services.AddScoped<IMailService, GmailService>();
            services.AddScoped<ICartItemService, CartItemService>();
            services.AddScoped<IAddressService, AddressService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IDriveService, GGDriveService>();
            services.AddScoped<IProductImageService, ProductImageService>();
            services.AddScoped<IReviewService, ReviewService>();
            services.AddScoped<ICommentService, CommentService>();
            services.AddTransient<ITokenService, TokenService>();
            //services.AddScoped<IChatRoomService, ChatRoomService>();
            //services.AddScoped<IMessageService, MessageService>();
            services.AddGraphQLServer().AddQueryType<Query>().AddProjections().AddFiltering().AddSorting();
        }

        public static void AddMappingProfile(this IServiceCollection services)
        {
            var profiles = new MapperConfiguration(
                _ => { _.AddProfile(new AutoMapping()); }
            );

            services.AddSingleton(profiles.CreateMapper());
        }
    }
}