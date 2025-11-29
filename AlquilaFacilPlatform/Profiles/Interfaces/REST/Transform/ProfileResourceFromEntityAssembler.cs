using AlquilaFacilPlatform.IAM.Interfaces.REST.Transform;
using AlquilaFacilPlatform.Profiles.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Profiles.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.Profiles.Interfaces.REST.Transform;

public class ProfileResourceFromEntityAssembler
{
    public static ProfileResource ToResourceFromEntity(Profile entity)
    {
        return new ProfileResource(
            entity.Id,
            entity.FullName,
            entity.PhoneNumber,
            entity.NumberDocument,
            entity.BirthDate,
            entity.AvatarUrl
            );
    }
}