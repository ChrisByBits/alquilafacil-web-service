using AlquilaFacilPlatform.Availability.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.Availability.Domain.Repositories;

public interface IAvailabilityCalendarRepository : IBaseRepository<AvailabilityCalendar>
{
    Task<AvailabilityCalendar?> FindByIdAsync(int calendarId);
    Task<IEnumerable<AvailabilityCalendar>> FindByLocalIdAndDateRangeAsync(int localId, DateTime startDate, DateTime endDate);
    Task<IEnumerable<AvailabilityCalendar>> FindConflictsAsync(int localId, DateTime startDate, DateTime endDate);
}
