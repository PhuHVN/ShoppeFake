using Microsoft.Extensions.Configuration;
using ShoppeFake.Infrastructure.DatabaseSettings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Infrastructure.SeedData
{
    public class InitData
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public InitData(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }
        public async Task SeedAsync()
        {
            if (!_context.Accounts.Any())
            {
                var adminEmail = _configuration["Admin:Email"];
                var adminPassword = _configuration["Admin:Password"];
                             
                var adminAccount = new Domain.Entities.Account
                {
                    
                    Email = adminEmail,
                    Password = BCrypt.Net.BCrypt.HashPassword(adminPassword),
                    FullName = "Administrator",
                    Role = Domain.Enums.RoleEnum.Admin,
                    Status = Domain.Enums.StatusEnum.Active,
                    CreatedAt = DateTime.UtcNow
                };
                await _context.Accounts.AddAsync(adminAccount);
                await _context.SaveChangesAsync();
            }
        }
    }
}
