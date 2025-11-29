namespace AlquilaFacilPlatform.Availability.Domain.Model.Commands;

public record UpdateAvailabilityCalendarCommand(
    int CalendarId,
    DateTime StartDate,
    DateTime EndDate,
    bool IsAvailable,
    string? Reason = null
);
