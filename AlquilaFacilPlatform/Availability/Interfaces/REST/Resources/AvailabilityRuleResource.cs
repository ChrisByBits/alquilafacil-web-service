namespace AlquilaFacilPlatform.Availability.Interfaces.REST.Resources;

public record AvailabilityRuleResource(
    int Id,
    int LocalId,
    int DayOfWeek,
    string StartTime,
    string EndTime,
    bool IsAvailable,
    DateTime CreatedAt,
    int CreatedBy
);
