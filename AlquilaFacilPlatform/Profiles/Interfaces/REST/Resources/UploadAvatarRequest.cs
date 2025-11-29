namespace AlquilaFacilPlatform.Profiles.Interfaces.REST.Resources;

public class UploadAvatarRequest
{
    public IFormFile File { get; set; } = null!;
    public int UserId { get; set; }
}
