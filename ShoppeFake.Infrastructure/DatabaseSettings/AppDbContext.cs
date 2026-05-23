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
        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<VariantAttributeValue> VariantAttributeValues { get; set; }
        public DbSet<Domain.Entities.Attribute> Attributes { get; set; }
        public DbSet<AttributeValue> AttributeValues { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            // Model configurations go here
            modelBuilder.Entity<Product>()
                .HasIndex(p => p.Slug)
                .IsUnique();
            modelBuilder.Entity<Category>()
                .HasIndex(c => c.Name)
                .IsUnique();
            modelBuilder.Entity<AttributeValue>()
                .HasIndex(c => new { c.AttributeId,c.Slug })
                .IsUnique();
            modelBuilder.Entity<VariantAttributeValue>()
                .HasIndex(v => new { v.AttributeId, v.ProductVariantId })
                .IsUnique();
        }
    }
}
//dotnet ef migrations add AddAccountTable -p ShoppeFake.Infrastructure -s ShoppeFake.API
//dotnet ef database update -p ShoppeFake.Infrastructure -s ShoppeFake.API