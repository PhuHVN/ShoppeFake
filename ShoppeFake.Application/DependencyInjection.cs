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
            
            
        }


    }
}
