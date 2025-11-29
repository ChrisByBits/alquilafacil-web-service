namespace AlquilaFacilPlatform.ImageManagement.Domain.Model.Commands;

public record UploadImageCommand(
    string Url,
    string FileName,
    string ContentType,
    long FileSizeBytes,
    string StoragePath,
    string EntityType,
    int EntityId,
    int UploadedBy,
    int? Width = null,
    int? Height = null
);
