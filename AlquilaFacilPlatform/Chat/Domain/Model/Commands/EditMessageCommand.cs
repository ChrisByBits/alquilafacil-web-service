namespace AlquilaFacilPlatform.Chat.Domain.Model.Commands;

public record EditMessageCommand(int MessageId, int SenderId, string NewContent);
