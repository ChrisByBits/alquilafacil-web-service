namespace AlquilaFacilPlatform.Availability.Interfaces.REST.Resources;

public record CreateAvailabilityRuleResource(
    int LocalId,
    int DayOfWeek,
    string StartTime, // Format: "HH:mm:ss"
    string EndTime,   // Format: "HH:mm:ss"
    bool IsAvailable,
    int CreatedBy
);
