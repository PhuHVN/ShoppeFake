namespace ShoppeFake.Application.Interfaces
{
    public interface IExcelService
    {
        Task<byte[]> ExportProductsToExcel();
    }
}
