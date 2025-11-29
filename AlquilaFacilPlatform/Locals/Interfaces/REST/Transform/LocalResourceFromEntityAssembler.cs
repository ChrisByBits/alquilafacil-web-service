using AlquilaFacilPlatform.Locals.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Locals.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.Locals.Interfaces.REST.Transform;

public static class LocalResourceFromEntityAssembler
{
    public static LocalResource ToResourceFromEntity(Local local)
    {
        return new LocalResource(
            local.Id,
            local.LocalName,
            local.DescriptionMessage,
            local.Address,
            local.Price,
            local.Capacity,
            local.LocalPhotos.Select(photo => photo.Url),
            local.Features,
            local.LocalCategoryId,
            local.UserId,
            local.Latitude,
            local.Longitude
        );
    }
}