using AlquilaFacilPlatform.ImageManagement.Domain.Model.Aggregates;
using AlquilaFacilPlatform.ImageManagement.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.ImageManagement.Interfaces.REST.Transform;

public static class ImageResourceFromEntityAssembler
{
    public static ImageResource ToResourceFromEntity(Image entity)
    {
        return new ImageResource(
            entity.Id,
            entity.Url,
            entity.FileName,
            entity.ContentType,
            entity.FileSizeBytes,
            entity.Width,
            entity.Height,
            entity.EntityType,
            entity.EntityId,
            entity.UploadedAt,
            entity.UploadedBy
        );
    }
}
