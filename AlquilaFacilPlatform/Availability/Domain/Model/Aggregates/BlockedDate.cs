namespace AlquilaFacilPlatform.Availability.Domain.Model.Aggregates;

public class BlockedDate
{
    public int Id { get; set; }
    public int LocalId { get; set; }
    public DateTime Date { get; set; }
    public string Reason { get; set; } = string.Empty;
    public bool IsRecurring { get; set; } // For recurring blockouts (e.g., every Sunday)
    public int? RecurringDayOfWeek { get; set; } // 0 = Sunday, 6 = Saturday
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }

    public BlockedDate()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public BlockedDate(int localId, DateTime date, string reason, int createdBy, bool isRecurring = false, int? recurringDayOfWeek = null) : this()
    {
        LocalId = localId;
        Date = date;
        Reason = reason;
        CreatedBy = createdBy;
        IsRecurring = isRecurring;
        RecurringDayOfWeek = recurringDayOfWeek;
    }

    public bool IsDateBlocked(DateTime checkDate)
    {
        if (IsRecurring && RecurringDayOfWeek.HasValue)
        {
            return (int)checkDate.DayOfWeek == RecurringDayOfWeek.Value;
        }

        return checkDate.Date == Date.Date;
    }
}
