using AlquilaFacilPlatform.Availability.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Availability.Domain.Repositories;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlquilaFacilPlatform.Availability.Infrastructure.Persistence.EFC.Repositories;

public class BlockedDateRepository(AppDbContext context)
    : BaseRepository<BlockedDate>(context), IBlockedDateRepository
{
    public async Task<IEnumerable<BlockedDate>> FindByLocalIdAsync(int localId)
    {
        return await Context.Set<BlockedDate>()
            .Where(b => b.LocalId == localId)
            .OrderBy(b => b.Date)
            .ToListAsync();
    }

    public async Task<IEnumerable<BlockedDate>> FindByLocalIdAndDateRangeAsync(int localId, DateTime startDate, DateTime endDate)
    {
        return await Context.Set<BlockedDate>()
            .Where(b => b.LocalId == localId &&
                        ((b.Date >= startDate && b.Date < endDate) || b.IsRecurring))
            .OrderBy(b => b.Date)
            .ToListAsync();
    }
}
