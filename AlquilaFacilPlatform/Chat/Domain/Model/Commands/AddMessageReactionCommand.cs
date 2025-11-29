namespace AlquilaFacilPlatform.Chat.Domain.Model.Commands;

public record AddMessageReactionCommand(int MessageId, int UserId, string Emoji);
