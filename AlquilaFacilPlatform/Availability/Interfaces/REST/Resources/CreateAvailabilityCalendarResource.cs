namespace AlquilaFacilPlatform.Availability.Interfaces.REST.Resources;

public record CreateAvailabilityCalendarResource(
    int LocalId,
    DateTime StartDate,
    DateTime EndDate,
    bool IsAvailable,
    int CreatedBy,
    string? Reason = null
);
