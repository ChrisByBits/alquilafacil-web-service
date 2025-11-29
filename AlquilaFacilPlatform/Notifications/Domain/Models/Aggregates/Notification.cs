using AlquilaFacilPlatform.Notifications.Domain.Models.Commands;

namespace AlquilaFacilPlatform.Notifications.Domain.Models.Aggregates;

public partial class Notification
{
    public int Id { get; set; }
    public string Title { get; }
    public string Description { get; }
    public int UserId { get; }
    public bool IsRead { get; private set; }
    public DateTime CreatedAt { get; }
}

public partial class Notification
{
    public Notification()
    {
        Title = string.Empty;
        Description = string.Empty;
        UserId = 0;
        IsRead = false;
        CreatedAt = DateTime.UtcNow;
    }

    public Notification(CreateNotificationCommand command)
    {
        Title = command.Title;
        Description = command.Description;
        UserId = command.UserId;
        IsRead = false;
        CreatedAt = DateTime.UtcNow;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}