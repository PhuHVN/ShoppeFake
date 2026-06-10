using Microsoft.AspNetCore.Http;

namespace ShoppeFake.Application.Interfaces
{
    public interface ICloudinaryService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}
