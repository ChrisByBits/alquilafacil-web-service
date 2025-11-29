namespace AlquilaFacilPlatform.Availability.Domain.Model.Aggregates;

public class AvailabilityRule
{
    public int Id { get; set; }
    public int LocalId { get; set; }
    public int DayOfWeek { get; set; } // 0 = Sunday, 6 = Saturday
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public bool IsAvailable { get; set; }
    public DateTime CreatedAt { get; set; }
    public int CreatedBy { get; set; }

    public AvailabilityRule()
    {
        CreatedAt = DateTime.UtcNow;
        IsAvailable = true;
    }

    public AvailabilityRule(int localId, int dayOfWeek, TimeSpan startTime, TimeSpan endTime, bool isAvailable, int createdBy) : this()
    {
        LocalId = localId;
        DayOfWeek = dayOfWeek;
        StartTime = startTime;
        EndTime = endTime;
        IsAvailable = isAvailable;
        CreatedBy = createdBy;
    }

    public bool AppliesToDateTime(DateTime dateTime)
    {
        return (int)dateTime.DayOfWeek == DayOfWeek;
    }

    public bool IsTimeAvailable(TimeSpan time)
    {
        if (!IsAvailable)
            return false;

        return time >= StartTime && time < EndTime;
    }
}
