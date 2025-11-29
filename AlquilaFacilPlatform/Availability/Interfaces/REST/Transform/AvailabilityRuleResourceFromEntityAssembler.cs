using AlquilaFacilPlatform.Availability.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Availability.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.Availability.Interfaces.REST.Transform;

public static class AvailabilityRuleResourceFromEntityAssembler
{
    public static AvailabilityRuleResource ToResourceFromEntity(AvailabilityRule rule)
    {
        return new AvailabilityRuleResource(
            rule.Id,
            rule.LocalId,
            rule.DayOfWeek,
            rule.StartTime.ToString(@"hh\:mm\:ss"),
            rule.EndTime.ToString(@"hh\:mm\:ss"),
            rule.IsAvailable,
            rule.CreatedAt,
            rule.CreatedBy
        );
    }
}
