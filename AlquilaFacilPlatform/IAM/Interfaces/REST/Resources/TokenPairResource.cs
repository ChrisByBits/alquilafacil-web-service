namespace AlquilaFacilPlatform.IAM.Interfaces.REST.Resources;

public record TokenPairResource(
    string AccessToken,
    string RefreshToken,
    int ExpiresIn,
    string TokenType = "Bearer"
);
