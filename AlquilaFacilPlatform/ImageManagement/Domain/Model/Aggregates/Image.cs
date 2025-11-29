namespace AlquilaFacilPlatform.ImageManagement.Domain.Model.Aggregates;

public class Image
{
    public int Id { get; private set; }
    public string Url { get; private set; }
    public string FileName { get; private set; }
    public string ContentType { get; private set; }
    public long FileSizeBytes { get; private set; }
    public int? Width { get; private set; }
    public int? Height { get; private set; }
    public string StoragePath { get; private set; }
    public string EntityType { get; private set; } // "Local", "Profile", "Chat"
    public int EntityId { get; private set; }
    public DateTime UploadedAt { get; private set; }
    public int UploadedBy { get; private set; }
    public bool IsDeleted { get; private set; }

    public Image()
    {
        UploadedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public Image(string url, string fileName, string contentType, long fileSizeBytes,
        string storagePath, string entityType, int entityId, int uploadedBy,
        int? width = null, int? height = null)
    {
        Url = url;
        FileName = fileName;
        ContentType = contentType;
        FileSizeBytes = fileSizeBytes;
        Width = width;
        Height = height;
        StoragePath = storagePath;
        EntityType = entityType;
        EntityId = entityId;
        UploadedBy = uploadedBy;
        UploadedAt = DateTime.UtcNow;
        IsDeleted = false;
    }

    public void MarkAsDeleted()
    {
        IsDeleted = true;
    }
}
