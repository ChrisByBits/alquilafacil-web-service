using AlquilaFacilPlatform.IAM.Domain.Model.Aggregates;

namespace AlquilaFacilPlatform.IAM.Application.Internal.OutboundServices;

public interface ITokenService
{
    /// <summary>
    /// Generates an access token for the user (15 minutes expiration)
    /// </summary>
    string GenerateToken(User user);

    /// <summary>
    /// Generates a refresh token string (7 days expiration)
    /// </summary>
    string GenerateRefreshToken();

    /// <summary>
    /// Gets the refresh token expiration date
    /// </summary>
    DateTime GetRefreshTokenExpiration();

    /// <summary>
    /// Validates a JWT token and returns the user ID if valid
    /// </summary>
    int? ValidateTokenAndGetUserId(string token);
}