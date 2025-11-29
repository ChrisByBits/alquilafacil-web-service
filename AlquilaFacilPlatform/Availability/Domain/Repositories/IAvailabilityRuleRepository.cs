using AlquilaFacilPlatform.Availability.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.Availability.Domain.Repositories;

public interface IAvailabilityRuleRepository : IBaseRepository<AvailabilityRule>
{
    Task<IEnumerable<AvailabilityRule>> FindByLocalIdAsync(int localId);
    Task<IEnumerable<AvailabilityRule>> FindByLocalIdAndDayOfWeekAsync(int localId, int dayOfWeek);
}
