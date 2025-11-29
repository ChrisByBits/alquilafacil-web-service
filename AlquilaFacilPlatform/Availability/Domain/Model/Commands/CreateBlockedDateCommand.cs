namespace AlquilaFacilPlatform.Availability.Domain.Model.Commands;

public record CreateBlockedDateCommand(
    int LocalId,
    DateTime Date,
    string Reason,
    int CreatedBy,
    bool IsRecurring = false,
    int? RecurringDayOfWeek = null
);
