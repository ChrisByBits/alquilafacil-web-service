using AlquilaFacilPlatform.ImageManagement.Application.Internal.OutboundServices;
using Microsoft.AspNetCore.Http;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace AlquilaFacilPlatform.ImageManagement.Infrastructure.Persistence.LocalStorage.Services;

public class LocalImageStorageService : IImageStorageService
{
    private readonly IWebHostEnvironment _environment;
    private readonly IConfiguration _configuration;
    private readonly long _maxFileSizeBytes;
    private readonly string[] _allowedExtensions;
    private readonly string[] _allowedContentTypes;

    public LocalImageStorageService(IWebHostEnvironment environment, IConfiguration configuration)
    {
        _environment = environment;
        _configuration = configuration;
        _maxFileSizeBytes = configuration.GetValue<long>("ImageStorage:MaxFileSizeMB", 5) * 1024 * 1024;
        _allowedExtensions = configuration.GetSection("ImageStorage:AllowedExtensions").Get<string[]>()
            ?? new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        _allowedContentTypes = configuration.GetSection("ImageStorage:AllowedContentTypes").Get<string[]>()
            ?? new[] { "image/jpeg", "image/png", "image/gif", "image/webp" };
    }

    public async Task<(string url, string storagePath)> UploadImageAsync(IFormFile file, string folder)
    {
        if (!ValidateImage(file, out var errorMessage))
            throw new InvalidOperationException(errorMessage);

        // Create uploads directory if it doesn't exist
        var uploadsPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, "uploads", folder);
        Directory.CreateDirectory(uploadsPath);

        // Generate unique filename
        var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
        var uniqueFileName = $"{Guid.NewGuid()}{fileExtension}";
        var filePath = Path.Combine(uploadsPath, uniqueFileName);

        // Save file
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        // Generate URL
        var relativePath = Path.Combine("uploads", folder, uniqueFileName).Replace("\\", "/");
        var baseUrl = _configuration.GetValue<string>("ImageStorage:BaseUrl") ?? "";
        var url = $"{baseUrl}/{relativePath}";

        return (url, relativePath);
    }

    public Task<bool> DeleteImageAsync(string storagePath)
    {
        try
        {
            var fullPath = Path.Combine(_environment.WebRootPath ?? _environment.ContentRootPath, storagePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task<(int? width, int? height)> GetImageDimensionsAsync(IFormFile file)
    {
        try
        {
            using var image = await Image.LoadAsync(file.OpenReadStream());
            return (image.Width, image.Height);
        }
        catch
        {
            return (null, null);
        }
    }

    public bool ValidateImage(IFormFile file, out string errorMessage)
    {
        errorMessage = string.Empty;

        if (file == null || file.Length == 0)
        {
            errorMessage = "File is empty";
            return false;
        }

        if (file.Length > _maxFileSizeBytes)
        {
            errorMessage = $"File size exceeds maximum allowed size of {_maxFileSizeBytes / 1024 / 1024} MB";
            return false;
        }

        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!_allowedExtensions.Contains(extension))
        {
            errorMessage = $"File extension {extension} is not allowed. Allowed extensions: {string.Join(", ", _allowedExtensions)}";
            return false;
        }

        if (!_allowedContentTypes.Contains(file.ContentType.ToLowerInvariant()))
        {
            errorMessage = $"Content type {file.ContentType} is not allowed";
            return false;
        }

        return true;
    }
}
