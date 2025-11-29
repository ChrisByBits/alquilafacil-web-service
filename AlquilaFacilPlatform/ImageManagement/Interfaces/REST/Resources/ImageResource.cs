namespace AlquilaFacilPlatform.ImageManagement.Interfaces.REST.Resources;

public record ImageResource(
    int Id,
    string Url,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    int? Width,
    int? Height,
    string EntityType,
    int EntityId,
    DateTime UploadedAt,
    int UploadedBy
);
