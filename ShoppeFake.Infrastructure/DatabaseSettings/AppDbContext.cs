using Microsoft.EntityFrameworkCore;
using ShoppeFake.Domain.Entities;

namespace ShoppeFake.Infrastructure.DatabaseSettings
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        //DbSets 
        public DbSet<Account> Accounts { get; set; }
       

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            // Model configurations go here

        }
    }
}
//dotnet ef migrations add AddAccountTable -p DontMissPassword.Infrastructure -s DontMissPassword.API
//dotnet ef database update -p DontMissPassword.Infrastructure -s DontMissPassword.API