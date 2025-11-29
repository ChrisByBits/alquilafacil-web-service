namespace AlquilaFacilPlatform.Availability.Interfaces.REST.Resources;

public record CreateBlockedDateResource(
    int LocalId,
    DateTime Date,
    string Reason,
    int CreatedBy,
    bool IsRecurring = false,
    int? RecurringDayOfWeek = null
);
