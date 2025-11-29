using AlquilaFacilPlatform.Locals.Domain.Model.Commands;
using AlquilaFacilPlatform.Locals.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.Locals.Interfaces.REST.Transform;

public static class CreateLocalCommandFromResourceAssembler
{
    public static CreateLocalCommand ToCommandFromResource(CreateLocalResource resource)
    {
        return new CreateLocalCommand(
            resource.LocalName,
            resource.DescriptionMessage,
            resource.Country,
            resource.City,
            resource.District,
            resource.Street,
            resource.Price,
            resource.Capacity,
            resource.PhotoUrls,
            resource.Features,
            resource.LocalCategoryId,
            resource.UserId,
            resource.Latitude,
            resource.Longitude
        );
    }
}