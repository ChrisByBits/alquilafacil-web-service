using AlquilaFacilPlatform.Availability.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.Availability.Domain.Repositories;

public interface IBlockedDateRepository : IBaseRepository<BlockedDate>
{
    Task<IEnumerable<BlockedDate>> FindByLocalIdAsync(int localId);
    Task<IEnumerable<BlockedDate>> FindByLocalIdAndDateRangeAsync(int localId, DateTime startDate, DateTime endDate);
}
