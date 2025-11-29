using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using AlquilaFacilPlatform.IAM.Application.Internal.OutboundServices;
using AlquilaFacilPlatform.IAM.Domain.Model.Aggregates;
using AlquilaFacilPlatform.IAM.Domain.Model.ValueObjects;
using AlquilaFacilPlatform.IAM.Infrastructure.Tokens.JWT.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AlquilaFacilPlatform.IAM.Infrastructure.Tokens.JWT.Services;

public class TokenService(IOptions<TokenSettings> tokenSettings) : ITokenService
{
    private readonly TokenSettings _tokenSettings = tokenSettings.Value;

    /// <summary>
    /// Generate access token (15 minutes expiration by default)
    /// </summary>
    public string GenerateToken(User user)
    {
        var secret = _tokenSettings.Secret;
        var key = Encoding.ASCII.GetBytes(secret);
        var roleName = Enum.GetName(typeof(EUserRoles), user.RoleId);
        if (roleName == null)
        {
            throw new ArgumentException("Invalid role ID");
        }
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Sid, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, roleName)
            }),
            Expires = DateTime.UtcNow.AddMinutes(_tokenSettings.AccessTokenExpirationMinutes),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature
            )
        };

        var tokenHandler = new JsonWebTokenHandler();
        return tokenHandler.CreateToken(tokenDescriptor);
    }

    /// <summary>
    /// Generate a random refresh token string
    /// </summary>
    public string GenerateRefreshToken()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }

    /// <summary>
    /// Gets the refresh token expiration date based on settings
    /// </summary>
    public DateTime GetRefreshTokenExpiration()
    {
        return DateTime.UtcNow.AddDays(_tokenSettings.RefreshTokenExpirationDays);
    }

    /// <summary>
    /// Validates a JWT token and returns the user ID if valid
    /// </summary>
    public int? ValidateTokenAndGetUserId(string token)
    {
        try
        {
            var secret = _tokenSettings.Secret;
            var key = Encoding.ASCII.GetBytes(secret);
            var tokenHandler = new JsonWebTokenHandler();

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false // We don't validate lifetime for refresh token flow
            };

            var result = tokenHandler.ValidateTokenAsync(token, validationParameters).Result;

            if (result.IsValid && result.Claims.TryGetValue(ClaimTypes.Sid, out var sidClaim))
            {
                if (int.TryParse(sidClaim?.ToString(), out var userId))
                {
                    return userId;
                }
            }

            return null;
        }
        catch
        {
            return null;
        }
    }
}