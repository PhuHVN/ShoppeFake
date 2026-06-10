using Microsoft.Extensions.DependencyInjection;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Application.Services;

namespace ShoppeFake.Application
{
    public static class DependencyInjection
    {
        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Application service registrations go here
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IAttributeService, AttributeService>();
            services.AddScoped<IAttributeValueService, AttributeValueService>();
            services.AddScoped<IVariantService, VariantService>();
            services.AddScoped<IProductImageService, ImageService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<ICartItemService, CartItemService>();
            services.AddScoped<IUserService, UserService>();
        }


    }
}
