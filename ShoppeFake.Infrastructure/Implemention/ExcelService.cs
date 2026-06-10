using ClosedXML.Excel;
using ShoppeFake.Application.Interfaces;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
namespace ShoppeFake.Infrastructure.Implemention
{
    public class ExcelService : IExcelService
    {
        private readonly IProductService _productService;
        private readonly IVariantService _variantService;
        
        public ExcelService(IProductService productService, IVariantService variantService)
        {
            _productService = productService;
            _variantService = variantService;
        }
        public async Task<byte[]> ExportProductsToExcel()
        {
            var result = await _variantService.GetAllToExportAsync();
            if(result.Count <= 0)
            {
                throw new Exception("Error: No data available for export.");
            }

            return ExportToExcel(result, "Products");
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

            ws.Columns().AdjustToContents();

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);

            return stream.ToArray();
        }
    }
}
