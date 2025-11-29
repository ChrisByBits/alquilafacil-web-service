namespace AlquilaFacilPlatform.Notifications.Interfaces.REST.Resources;

public record NotificationResource(int Id, string Title, string Message, int UserId, bool IsRead, DateTime CreatedAt);