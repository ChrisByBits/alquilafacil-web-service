namespace AlquilaFacilPlatform.Chat.Interfaces.REST.Resources;

public record ConversationResource(
    int Id,
    int ParticipantOneId,
    int ParticipantTwoId,
    DateTime CreatedAt,
    DateTime? LastMessageAt
);
