namespace AlquilaFacilPlatform.Chat.Domain.Model.Commands;

public record CreateConversationCommand(int ParticipantOneId, int ParticipantTwoId);
