using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppeFake.Application.Interfaces
{
    public interface IExcelService
    {
        Task<byte[]> ExportProductsToExcel();
    }
}
