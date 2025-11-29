namespace AlquilaFacilPlatform.ImageManagement.Interfaces.REST.Resources;

public class UploadImageRequest
{
    public IFormFile File { get; set; } = null!;
    public string EntityType { get; set; } = null!;
    public int EntityId { get; set; }
    public int UploadedBy { get; set; }
    public string? Folder { get; set; }
}
