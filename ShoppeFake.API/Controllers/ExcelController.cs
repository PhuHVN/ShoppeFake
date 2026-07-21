using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShoppeFake.Application.Interfaces;
using Swashbuckle.AspNetCore.Annotations;

namespace ShoppeFake.API.Controllers
{
    [Route("api/v1/excel")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class ExcelController : ControllerBase
    {
        private readonly IExcelService _excelService;

        public ExcelController(IExcelService excelService)
        {
            _excelService = excelService;
        }

        [HttpGet("export")]
        [SwaggerOperation(summary: "Admin - Export products to Excel", description: "Exports the list of products to an Excel file.")]
        public async Task<IActionResult> ExportToExcel()
        {
            var fileContent = await _excelService.ExportProductsToExcel();
            return File(fileContent,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    $"products_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
                );
        }

    }
}
