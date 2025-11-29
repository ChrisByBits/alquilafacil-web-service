using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Chat.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.Chat.Interfaces.REST.Transform;

public static class ConversationResourceFromEntityAssembler
{
    public static ConversationResource ToResourceFromEntity(Conversation conversation)
    {
        return new ConversationResource(
            conversation.Id,
            conversation.ParticipantOneId,
            conversation.ParticipantTwoId,
            conversation.CreatedAt,
            conversation.LastMessageAt
        );
    }
}
