namespace AlquilaFacilPlatform.Chat.Interfaces.REST.Resources;

public record SendMessageResource(int ConversationId, int SenderId, string Content);
