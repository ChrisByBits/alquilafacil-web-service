using AlquilaFacilPlatform.IAM.Domain.Model.Entities;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.IAM.Domain.Repositories;

public interface IRefreshTokenRepository : IBaseRepository<RefreshToken>
{
    Task<RefreshToken?> FindByTokenAsync(string token);
    Task<IEnumerable<RefreshToken>> FindByUserIdAsync(int userId);
    Task RevokeAllUserTokensAsync(int userId);
}
