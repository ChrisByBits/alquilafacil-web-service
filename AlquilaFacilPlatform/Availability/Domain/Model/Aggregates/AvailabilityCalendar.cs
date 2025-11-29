namespace AlquilaFacilPlatform.Availability.Domain.Model.Aggregates;

public class AvailabilityCalendar
{
    public int Id { get; set; }
    public int LocalId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsAvailable { get; set; }
    public string? Reason { get; set; } // e.g., "Maintenance", "Personal Use", "Reserved"
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }

    public AvailabilityCalendar()
    {
        CreatedAt = DateTime.UtcNow;
        IsAvailable = true;
    }

    public AvailabilityCalendar(int localId, DateTime startDate, DateTime endDate, bool isAvailable, int createdBy, string? reason = null) : this()
    {
        LocalId = localId;
        StartDate = startDate;
        EndDate = endDate;
        IsAvailable = isAvailable;
        CreatedBy = createdBy;
        Reason = reason;
    }

    public bool OverlapsWith(DateTime checkStart, DateTime checkEnd)
    {
        return StartDate < checkEnd && EndDate > checkStart;
    }

    public bool Contains(DateTime date)
    {
        return date >= StartDate && date < EndDate;
    }

    public void Update(DateTime startDate, DateTime endDate, bool isAvailable, string? reason = null)
    {
        StartDate = startDate;
        EndDate = endDate;
        IsAvailable = isAvailable;
        Reason = reason;
    }
}
