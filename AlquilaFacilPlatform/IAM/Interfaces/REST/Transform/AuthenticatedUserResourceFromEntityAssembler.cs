using AlquilaFacilPlatform.IAM.Domain.Model.Aggregates;
using AlquilaFacilPlatform.IAM.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.IAM.Interfaces.REST.Transform;

public static class AuthenticatedUserResourceFromEntityAssembler
{
    public static AuthenticatedUserResource ToResourceFromEntity(User user, string accessToken, string refreshToken)
    {
        return new AuthenticatedUserResource(user.Id, user.Username, accessToken, refreshToken);
    }
}