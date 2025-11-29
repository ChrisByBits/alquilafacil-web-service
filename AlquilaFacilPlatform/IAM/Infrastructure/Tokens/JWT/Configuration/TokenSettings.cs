namespace AlquilaFacilPlatform.IAM.Infrastructure.Tokens.JWT.Configuration;

public class TokenSettings
{
    /**
     * <summary>
     *     This class is used to store the token settings.
     *     It is used to configure the token settings in the appsettings.json file.
     * </summary>
     */
    public required string Secret { get; set; }

    /// <summary>
    /// Access token expiration time in minutes. Default: 15 minutes
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 15;

    /// <summary>
    /// Refresh token expiration time in days. Default: 7 days
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;
}