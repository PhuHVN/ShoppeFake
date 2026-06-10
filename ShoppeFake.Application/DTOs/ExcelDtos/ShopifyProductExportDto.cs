using System.ComponentModel.DataAnnotations;

namespace ShoppeFake.Application.DTOs.ExcelDtos
{
    public class ShopifyProductExportDto
    {
        [Display(Name = "Handle")]
        public string Handle { get; set; } = string.Empty;

        [Display(Name = "Title")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Body (HTML)")]
        public string BodyHtml { get; set; } = string.Empty;

        [Display(Name = "Vendor")]
        public string Vendor { get; set; } = string.Empty;

        [Display(Name = "Product Category")]
        public string ProductCategory { get; set; } = string.Empty;

        [Display(Name = "Type")]
        public string Type { get; set; } = string.Empty;

        [Display(Name = "Tags")]
        public string Tags { get; set; } = string.Empty;

        [Display(Name = "Option1 Name")]
        public string Option1Name { get; set; } = string.Empty;

        [Display(Name = "Option1 Value")]
        public string Option1Value { get; set; } = string.Empty;

        [Display(Name = "Option2 Name")]
        public string Option2Name { get; set; } = string.Empty;

        [Display(Name = "Option2 Value")]
        public string Option2Value { get; set; } = string.Empty;

        [Display(Name = "Option3 Name")]
        public string Option3Name { get; set; } = string.Empty;

        [Display(Name = "Option3 Value")]
        public string Option3Value { get; set; } = string.Empty;

        [Display(Name = "Variant SKU")]
        public string VariantSku { get; set; } = string.Empty;

        [Display(Name = "Variant Grams")]
        public int? VariantGrams { get; set; }

        [Display(Name = "Variant Inventory Qty")]
        public int VariantInventoryQty { get; set; }

        [Display(Name = "Variant Price")]
        public decimal VariantPrice { get; set; }

        [Display(Name = "Variant Image")]
        public string VariantImage { get; set; } = string.Empty;

        [Display(Name = "Status")]
        public string Status { get; set; } = string.Empty;
    }
}
