using AlquilaFacilPlatform.Availability.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Availability.Domain.Repositories;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlquilaFacilPlatform.Availability.Infrastructure.Persistence.EFC.Repositories;

public class AvailabilityRuleRepository(AppDbContext context)
    : BaseRepository<AvailabilityRule>(context), IAvailabilityRuleRepository
{
    public async Task<IEnumerable<AvailabilityRule>> FindByLocalIdAsync(int localId)
    {
        return await Context.Set<AvailabilityRule>()
            .Where(r => r.LocalId == localId)
            .OrderBy(r => r.DayOfWeek)
            .ThenBy(r => r.StartTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<AvailabilityRule>> FindByLocalIdAndDayOfWeekAsync(int localId, int dayOfWeek)
    {
        return await Context.Set<AvailabilityRule>()
            .Where(r => r.LocalId == localId && r.DayOfWeek == dayOfWeek)
            .OrderBy(r => r.StartTime)
            .ToListAsync();
    }
}
