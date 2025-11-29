namespace AlquilaFacilPlatform.Availability.Interfaces.REST.Resources;

public record BlockedDateResource(
    int Id,
    int LocalId,
    DateTime Date,
    string Reason,
    bool IsRecurring,
    int? RecurringDayOfWeek,
    DateTime CreatedAt,
    int CreatedBy
);
