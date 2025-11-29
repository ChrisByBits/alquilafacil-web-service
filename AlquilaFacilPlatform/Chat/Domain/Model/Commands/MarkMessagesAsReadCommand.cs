namespace AlquilaFacilPlatform.Chat.Domain.Model.Commands;

public record MarkMessagesAsReadCommand(int ConversationId, int UserId);
