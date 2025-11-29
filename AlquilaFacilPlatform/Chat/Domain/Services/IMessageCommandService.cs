using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Chat.Domain.Model.Commands;

namespace AlquilaFacilPlatform.Chat.Domain.Services;

public interface IMessageCommandService
{
    Task<Message?> Handle(SendMessageCommand command);
    Task Handle(MarkMessagesAsReadCommand command);
    Task<Message?> Handle(EditMessageCommand command);
    Task<bool> Handle(DeleteMessageCommand command);
    Task<Message?> Handle(AddMessageReactionCommand command);
    Task<Message?> Handle(RemoveMessageReactionCommand command);
}
