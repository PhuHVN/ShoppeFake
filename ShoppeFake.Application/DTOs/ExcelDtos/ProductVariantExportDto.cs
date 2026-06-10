using System.ComponentModel.DataAnnotations;

namespace ShoppeFake.Application.DTOs.ExcelDtos
{
    public class ProductVariantExportDto
    {
        [Display(Name = "Product Variant ID")]
        public int ProductId { get; set; }
        [Display(Name = "Variant ID")]
        public int VariantId { get; set; }

        [Display(Name = "Product Name")]
        public string ProductName { get; set; } = default!;
        [Display(Name = "Product Description")]
        public string? ProductDescription { get; set; }

        [Display(Name = "Category Name")]
        public string? CategoryName { get; set; }
        [Display(Name = "Brand Name")]
        public string? BrandName { get; set; }

        [Display(Name = "Variant Name")]
        public string VariantName { get; set; } = default!;
        [Display(Name = "SKU")]
        public string Sku { get; set; } = default!;

        [Display(Name = "Price")]
        public decimal Price { get; set; }
        [Display(Name = "Stock Quantity")]
        public int StockQuantity { get; set; }
        [Display(Name = "Weight (grams)")]
        public int? WeightGrams { get; set; }

        [Display(Name = "Status")]
        public string Status { get; set; } = default!;

        [Display(Name = "Image URLs")]
        public string ImageUrls { get; set; } = default!;

        [Display(Name = "Attributes")]
        public string Attributes { get; set; } = default!;

        [Display(Name = "Created At")]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Updated At")]
        public DateTime? UpdatedAt { get; set; }

    }
}
