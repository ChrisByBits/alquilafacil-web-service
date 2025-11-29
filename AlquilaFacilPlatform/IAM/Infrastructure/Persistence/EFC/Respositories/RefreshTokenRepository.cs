using AlquilaFacilPlatform.IAM.Domain.Model.Entities;
using AlquilaFacilPlatform.IAM.Domain.Repositories;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlquilaFacilPlatform.IAM.Infrastructure.Persistence.EFC.Respositories;

public class RefreshTokenRepository : BaseRepository<RefreshToken>, IRefreshTokenRepository
{
    public RefreshTokenRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<RefreshToken?> FindByTokenAsync(string token)
    {
        return await Context.Set<RefreshToken>()
            .FirstOrDefaultAsync(rt => rt.Token == token);
    }

    public async Task<IEnumerable<RefreshToken>> FindByUserIdAsync(int userId)
    {
        return await Context.Set<RefreshToken>()
            .Where(rt => rt.UserId == userId)
            .ToListAsync();
    }

    public async Task RevokeAllUserTokensAsync(int userId)
    {
        var tokens = await Context.Set<RefreshToken>()
            .Where(rt => rt.UserId == userId && !rt.IsRevoked)
            .ToListAsync();

        foreach (var token in tokens)
        {
            token.Revoke();
        }
    }
}
