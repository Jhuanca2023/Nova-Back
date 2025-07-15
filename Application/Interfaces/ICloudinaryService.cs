using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface ICloudinaryService
{
    Task<string> UploadImage(IFormFile file);
    Task DeleteImage(string publicId);
    string GetPublicIdFromUrl(string imageUrl);
}