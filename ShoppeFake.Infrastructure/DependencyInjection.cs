using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ShoppeFake.Application.Interfaces;
using ShoppeFake.Domain.Abstractions;
using ShoppeFake.Infrastructure.Implemention;
using ShoppeFake.Infrastructure.SeedData;

namespace ShoppeFake.Infrastructure
{
    public static class DependencyInjection
    {
        public static void AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Infrastructure service registrations go here
            services.AddLogging();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IJwtProvider, JwtProvider>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IRedisService, OtpCacheService>();
            services.AddScoped<ICloudinaryService, CloudinaryService>();
            services.AddScoped<IExcelService, ExcelService>();
            services.AddScoped<IChatApiClient, ChatApiClient>();
            services.AddScoped<IChatApiService, ChatApiService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<InitData>();

        }

    }
}
