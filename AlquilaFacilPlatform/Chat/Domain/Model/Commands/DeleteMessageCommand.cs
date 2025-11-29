namespace AlquilaFacilPlatform.Chat.Domain.Model.Commands;

public record DeleteMessageCommand(int MessageId, int SenderId);
