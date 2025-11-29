using AlquilaFacilPlatform.Availability.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Availability.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.Availability.Interfaces.REST.Transform;

public static class AvailabilityCalendarResourceFromEntityAssembler
{
    public static AvailabilityCalendarResource ToResourceFromEntity(AvailabilityCalendar calendar)
    {
        return new AvailabilityCalendarResource(
            calendar.Id,
            calendar.LocalId,
            calendar.StartDate,
            calendar.EndDate,
            calendar.IsAvailable,
            calendar.Reason,
            calendar.CreatedAt,
            calendar.CreatedBy
        );
    }
}
