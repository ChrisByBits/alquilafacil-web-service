using AlquilaFacilPlatform.Notifications.Domain.Models.Aggregates;
using AlquilaFacilPlatform.Notifications.Domain.Models.Commands;
using AlquilaFacilPlatform.Notifications.Domain.Repositories;
using AlquilaFacilPlatform.Notifications.Domain.Services;
using AlquilaFacilPlatform.Notifications.Interfaces.REST.Hubs;
using AlquilaFacilPlatform.Shared.Domain.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace AlquilaFacilPlatform.Notifications.Application.Internal.CommandServices;

public class NotificationCommandService(
    IUnitOfWork unitOfWork,
    INotificationRepository notificationRepository,
    IHubContext<NotificationHub> notificationHub) : INotificationCommandService
{
    public async Task<Notification> Handle(CreateNotificationCommand command)
    {
        var notification = new Notification(command);
        await notificationRepository.AddAsync(notification);
        await unitOfWork.CompleteAsync();

        // Send real-time notification via SignalR
        await notificationHub.Clients
            .Group($"notifications_{command.UserId}")
            .SendAsync("ReceiveNotification", new
            {
                id = notification.Id,
                title = notification.Title,
                message = notification.Description,
                userId = notification.UserId,
                createdAt = notification.CreatedAt,
                isRead = false
            });

        return notification;
    }

    public async Task<Notification> Handle(DeleteNotificationCommand command)
    {
        var notification = await notificationRepository.FindByIdAsync(command.Id);
        if (notification == null)
        {
            throw new Exception("Notification not found");
        }

        var userId = notification.UserId;
        notificationRepository.Remove(notification);
        await unitOfWork.CompleteAsync();

        // Notify client that notification was deleted
        await notificationHub.Clients
            .Group($"notifications_{userId}")
            .SendAsync("NotificationDeleted", command.Id);

        return notification;
    }

    public async Task<Notification?> Handle(MarkNotificationAsReadCommand command)
    {
        var notification = await notificationRepository.FindByIdAsync(command.Id);
        if (notification == null)
        {
            return null;
        }

        notification.MarkAsRead();
        await unitOfWork.CompleteAsync();

        // Notify client that notification was marked as read
        await notificationHub.Clients
            .Group($"notifications_{notification.UserId}")
            .SendAsync("NotificationMarkedAsRead", command.Id);

        return notification;
    }
}