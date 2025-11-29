namespace AlquilaFacilPlatform.IAM.Interfaces.REST.Resources;

public record AuthenticatedUserResource(
    int Id,
    string Username,
    string AccessToken,
    string RefreshToken,
    string TokenType = "Bearer"
);