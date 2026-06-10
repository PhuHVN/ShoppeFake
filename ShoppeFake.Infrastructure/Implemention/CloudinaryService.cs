using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using ShoppeFake.Application.Interfaces;

namespace ShoppeFake.Infrastructure.Implemention
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly ICloudinary _cloudinary;

        public CloudinaryService(ICloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is null or empty", nameof(file));
            }

            if (file.Length > 5 * 1024 * 1024)
            {
                throw new ArgumentException("File size exceeds the limit of 5MB", nameof(file));
            }

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "products"
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result == null || result.Error != null)
            {
                throw new ArgumentException("Error occurred during Cloudinary upload: " + (result?.Error?.Message ?? "Unknown error"));
            }
            return result.SecureUrl.ToString();
        }
    }
}
