namespace AlquilaFacilPlatform.Availability.Interfaces.REST.Resources;

public record AvailabilityCalendarResource(
    int Id,
    int LocalId,
    DateTime StartDate,
    DateTime EndDate,
    bool IsAvailable,
    string? Reason,
    DateTime CreatedAt,
    int CreatedBy
);
