namespace AlquilaFacilPlatform.Availability.Domain.Model.Commands;

public record CreateAvailabilityCalendarCommand(
    int LocalId,
    DateTime StartDate,
    DateTime EndDate,
    bool IsAvailable,
    int CreatedBy,
    string? Reason = null
);
