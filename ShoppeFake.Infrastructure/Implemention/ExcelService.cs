using ClosedXML.Excel;
using ShoppeFake.Application.DTOs.ExcelDtos;
using ShoppeFake.Application.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
namespace ShoppeFake.Infrastructure.Implemention
{
    public class ExcelService : IExcelService
    {
        private readonly IVariantService _variantService;

        public ExcelService(IVariantService variantService)
        {

            _variantService = variantService;
        }
        public async Task<byte[]> ExportProductsToExcel()
        {
            var result = await _variantService.GetAllToExportAsync();
            if (result.Count <= 0)
            {
                throw new KeyNotFoundException("Error: No data available for export.");
            }

            var shopifyRows = result.Select(MapToShopifyProductExport).ToList();
            return ExportToExcel(shopifyRows, "Shopify Products");
        }

        private static ShopifyProductExportDto MapToShopifyProductExport(ProductVariantExportDto source)
        {
            var options = ParseOptions(source.Attributes);

            return new ShopifyProductExportDto
            {
                Handle = BuildHandle(source.ProductName, source.ProductId),
                Title = source.ProductName,
                BodyHtml = source.ProductDescription ?? string.Empty,
                Vendor = source.BrandName ?? string.Empty,
                ProductCategory = source.CategoryName ?? string.Empty,
                Type = source.CategoryName ?? string.Empty,
                Tags = BuildTags(options),
                Option1Name = GetOptionName(options, 0),
                Option1Value = GetOptionValue(options, 0),
                Option2Name = GetOptionName(options, 1),
                Option2Value = GetOptionValue(options, 1),
                Option3Name = GetOptionName(options, 2),
                Option3Value = GetOptionValue(options, 2),
                VariantSku = source.Sku,
                VariantGrams = source.WeightGrams,
                VariantInventoryQty = source.StockQuantity,
                VariantPrice = source.Price,
                VariantImage = GetFirstImageUrl(source.ImageUrls),
                Status = source.Status.Equals("Active", StringComparison.OrdinalIgnoreCase)
                    ? "active"
                    : "draft"
            };
        }

        private static List<(string Name, string Value)> ParseOptions(string? attributes)
        {
            if (string.IsNullOrWhiteSpace(attributes))
                return new List<(string Name, string Value)>();

            return attributes
                .Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(attribute =>
                {
                    var separatorIndex = attribute.IndexOf(':');
                    if (separatorIndex < 0)
                        return (Name: attribute.Trim(), Value: string.Empty);

                    return (
                        Name: attribute[..separatorIndex].Trim(),
                        Value: attribute[(separatorIndex + 1)..].Trim()
                    );
                })
                .Where(option => !string.IsNullOrWhiteSpace(option.Name))
                .ToList();
        }

        private static string BuildTags(IEnumerable<(string Name, string Value)> options)
        {
            var tags = options
                .Where(option => !string.IsNullOrWhiteSpace(option.Value))
                .Select(option => $"{option.Name}: {option.Value}");

            return string.Join(", ", tags.Distinct(StringComparer.OrdinalIgnoreCase));
        }

        private static string GetOptionName(IReadOnlyList<(string Name, string Value)> options, int index)
        {
            return index < options.Count ? options[index].Name : string.Empty;
        }

        private static string GetOptionValue(IReadOnlyList<(string Name, string Value)> options, int index)
        {
            return index < options.Count ? options[index].Value : string.Empty;
        }

        private static string GetFirstImageUrl(string? imageUrls)
        {
            if (string.IsNullOrWhiteSpace(imageUrls))
                return string.Empty;

            return imageUrls
                .Split(new[] { '|', ',' }, StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .FirstOrDefault() ?? string.Empty;
        }

        private static string BuildHandle(string productName, int productId)
        {
            var normalized = productName.Normalize(NormalizationForm.FormD);
            var builder = new StringBuilder();

            foreach (var character in normalized)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(character) != UnicodeCategory.NonSpacingMark)
                    builder.Append(char.ToLowerInvariant(character));
            }

            var slug = Regex.Replace(builder.ToString(), @"[^a-z0-9]+", "-").Trim('-');
            return string.IsNullOrWhiteSpace(slug)
                ? $"product-{productId}"
                : $"{slug}-{productId}";
        }

        public static byte[] ExportToExcel<T>(IEnumerable<T> data, string sheetName)
        {
            var items = data.ToList();

            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add(sheetName);

            var columns = typeof(T)
                .GetProperties()
                .Select(p =>
                {
                    var display = p.GetCustomAttribute<DisplayAttribute>();

                    return new
                    {
                        Property = p,
                        Header = display?.Name ?? p.Name,
                        Type = Nullable.GetUnderlyingType(p.PropertyType) ?? p.PropertyType
                    };
                })
                .ToList();

            // Header
            for (int col = 0; col < columns.Count; col++)
            {
                ws.Cell(1, col + 1).Value = columns[col].Header;
                ws.Cell(1, col + 1).Style.Font.Bold = true;
            }

            // Data
            for (int row = 0; row < items.Count; row++)
            {
                for (int col = 0; col < columns.Count; col++)
                {
                    var column = columns[col];
                    var value = column.Property.GetValue(items[row]);
                    var cell = ws.Cell(row + 2, col + 1);

                    if (value == null)
                    {
                        cell.Value = "";
                        continue;
                    }

                    if (column.Type == typeof(DateTime))
                    {
                        cell.Value = (DateTime)value;
                        cell.Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
                    }
                    else if (column.Type == typeof(decimal))
                    {
                        cell.Value = (decimal)value;
                        cell.Style.NumberFormat.Format = "#,##0";
                    }
                    else if (column.Type == typeof(double))
                    {
                        cell.Value = (double)value;
                    }
                    else if (column.Type == typeof(float))
                    {
                        cell.Value = Convert.ToDouble(value);
                    }
                    else if (column.Type == typeof(int))
                    {
                        cell.Value = (int)value;
                    }
                    else if (column.Type == typeof(long))
                    {
                        cell.Value = (long)value;
                    }
                    else if (column.Type == typeof(bool))
                    {
                        cell.Value = (bool)value;
                    }
                    else
                    {
                        cell.Value = value.ToString();
                    }
                }
            }

            ws.Columns().AdjustToContents(1, Math.Min(items.Count + 1, 200));

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }
    }
}
