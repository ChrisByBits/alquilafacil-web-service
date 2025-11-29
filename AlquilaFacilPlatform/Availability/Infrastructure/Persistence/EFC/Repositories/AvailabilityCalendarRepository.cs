using AlquilaFacilPlatform.Availability.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Availability.Domain.Repositories;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlquilaFacilPlatform.Availability.Infrastructure.Persistence.EFC.Repositories;

public class AvailabilityCalendarRepository(AppDbContext context)
    : BaseRepository<AvailabilityCalendar>(context), IAvailabilityCalendarRepository
{
    public async Task<AvailabilityCalendar?> FindByIdAsync(int calendarId)
    {
        return await Context.Set<AvailabilityCalendar>()
            .FirstOrDefaultAsync(c => c.Id == calendarId);
    }

    public async Task<IEnumerable<AvailabilityCalendar>> FindByLocalIdAndDateRangeAsync(int localId, DateTime startDate, DateTime endDate)
    {
        return await Context.Set<AvailabilityCalendar>()
            .Where(c => c.LocalId == localId &&
                        c.StartDate < endDate &&
                        c.EndDate > startDate)
            .OrderBy(c => c.StartDate)
            .ToListAsync();
    }

    public async Task<IEnumerable<AvailabilityCalendar>> FindConflictsAsync(int localId, DateTime startDate, DateTime endDate)
    {
        return await Context.Set<AvailabilityCalendar>()
            .Where(c => c.LocalId == localId &&
                        !c.IsAvailable &&
                        c.StartDate < endDate &&
                        c.EndDate > startDate)
            .OrderBy(c => c.StartDate)
            .ToListAsync();
    }
}
