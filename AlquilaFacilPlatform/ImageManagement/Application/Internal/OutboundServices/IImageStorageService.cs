using Microsoft.AspNetCore.Http;

namespace AlquilaFacilPlatform.ImageManagement.Application.Internal.OutboundServices;

public interface IImageStorageService
{
    Task<(string url, string storagePath)> UploadImageAsync(IFormFile file, string folder);
    Task<bool> DeleteImageAsync(string storagePath);
    Task<(int? width, int? height)> GetImageDimensionsAsync(IFormFile file);
    bool ValidateImage(IFormFile file, out string errorMessage);
}
