using AlquilaFacilPlatform.Availability.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Availability.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.Availability.Interfaces.REST.Transform;

public static class BlockedDateResourceFromEntityAssembler
{
    public static BlockedDateResource ToResourceFromEntity(BlockedDate blockedDate)
    {
        return new BlockedDateResource(
            blockedDate.Id,
            blockedDate.LocalId,
            blockedDate.Date,
            blockedDate.Reason,
            blockedDate.IsRecurring,
            blockedDate.RecurringDayOfWeek,
            blockedDate.CreatedAt,
            blockedDate.CreatedBy
        );
    }
}
