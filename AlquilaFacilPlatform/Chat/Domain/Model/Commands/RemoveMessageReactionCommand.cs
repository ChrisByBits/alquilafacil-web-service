namespace AlquilaFacilPlatform.Chat.Domain.Model.Commands;

public record RemoveMessageReactionCommand(int MessageId, int UserId, string Emoji);
