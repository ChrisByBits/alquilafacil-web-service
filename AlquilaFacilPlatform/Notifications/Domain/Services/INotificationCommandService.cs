using AlquilaFacilPlatform.Notifications.Domain.Models.Aggregates;
using AlquilaFacilPlatform.Notifications.Domain.Models.Commands;

namespace AlquilaFacilPlatform.Notifications.Domain.Services;

public interface INotificationCommandService
{
    Task<Notification> Handle(CreateNotificationCommand command);
    Task<Notification> Handle(DeleteNotificationCommand command);
    Task<Notification?> Handle(MarkNotificationAsReadCommand command);
}