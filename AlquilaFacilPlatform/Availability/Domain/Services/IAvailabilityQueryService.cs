using AlquilaFacilPlatform.Availability.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Availability.Domain.Model.Queries;

namespace AlquilaFacilPlatform.Availability.Domain.Services;

public interface IAvailabilityQueryService
{
    Task<IEnumerable<AvailabilityCalendar>> Handle(GetAvailabilityCalendarByLocalIdQuery query);
    Task<IEnumerable<BlockedDate>> Handle(GetBlockedDatesByLocalIdQuery query);
    Task<IEnumerable<AvailabilityRule>> Handle(GetAvailabilityRulesByLocalIdQuery query);
    Task<bool> Handle(CheckAvailabilityQuery query);
}
