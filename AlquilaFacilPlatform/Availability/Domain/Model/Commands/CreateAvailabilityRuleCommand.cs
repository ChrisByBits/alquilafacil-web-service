namespace AlquilaFacilPlatform.Availability.Domain.Model.Commands;

public record CreateAvailabilityRuleCommand(
    int LocalId,
    int DayOfWeek,
    TimeSpan StartTime,
    TimeSpan EndTime,
    bool IsAvailable,
    int CreatedBy
);
