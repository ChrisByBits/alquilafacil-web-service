using AlquilaFacilPlatform.Notifications.Domain.Models.Aggregates;
using AlquilaFacilPlatform.Notifications.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.Notifications.Interfaces.REST.Transforms;

public static class NotificationResourceFromEntityAssembler
{
    public static NotificationResource ToResourceFromEntity(Notification notification)
    {
        return new NotificationResource
        (
            notification.Id,
            notification.Title,
            notification.Description,
            notification.UserId,
            notification.IsRead,
            notification.CreatedAt
        );
    }
}