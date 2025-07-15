using Application.Interfaces;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace Application.Services;

public class CloudinaryService : ICloudinaryService
{
    private readonly Cloudinary _cloudinary;

    public CloudinaryService()
    {
        var cloudinaryUrl = Environment.GetEnvironmentVariable("CLOUDINARY_URL");
        _cloudinary = new Cloudinary(cloudinaryUrl);
    }

    public async Task<string> UploadImage(IFormFile file)
    {
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription(file.FileName, file.OpenReadStream()),
            PublicId = Guid.NewGuid().ToString(),
            // Transformation = new Transformation().Height(800).Width(600)
        };
        var result = await _cloudinary.UploadAsync(uploadParams);
        return result.SecureUrl.AbsoluteUri;
    }

    public async Task DeleteImage(string publicId)
    {
        await _cloudinary.DestroyAsync(new DeletionParams(publicId));
    }

    public string GetPublicIdFromUrl(string imageUrl)
    {
        var pathSegments = imageUrl.Split("/");
        var fileNameWithExtension = pathSegments[^1];
        var publicId = fileNameWithExtension.Split(".")[0];
        return publicId;
    }
}