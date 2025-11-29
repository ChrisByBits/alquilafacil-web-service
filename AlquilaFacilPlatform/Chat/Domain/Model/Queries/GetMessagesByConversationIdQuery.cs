namespace AlquilaFacilPlatform.Chat.Domain.Model.Queries;

public record GetMessagesByConversationIdQuery(int ConversationId, int PageNumber = 1, int PageSize = 50);
