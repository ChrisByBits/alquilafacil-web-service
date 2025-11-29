namespace AlquilaFacilPlatform.IAM.Domain.Model.Entities;

public class RefreshToken
{
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsRevoked { get; set; }
    public DateTime? RevokedAt { get; set; }

    public RefreshToken()
    {
        CreatedAt = DateTime.UtcNow;
        IsRevoked = false;
    }

    public RefreshToken(string token, int userId, DateTime expiresAt) : this()
    {
        Token = token;
        UserId = userId;
        ExpiresAt = expiresAt;
    }

    public bool IsExpired => DateTime.UtcNow >= ExpiresAt;
    public bool IsActive => !IsRevoked && !IsExpired;

    public void Revoke()
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
    }
}
