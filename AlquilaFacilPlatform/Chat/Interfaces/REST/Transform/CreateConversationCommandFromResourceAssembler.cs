using AlquilaFacilPlatform.Chat.Domain.Model.Commands;
using AlquilaFacilPlatform.Chat.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.Chat.Interfaces.REST.Transform;

public static class CreateConversationCommandFromResourceAssembler
{
    public static CreateConversationCommand ToCommandFromResource(CreateConversationResource resource)
    {
        return new CreateConversationCommand(resource.ParticipantOneId, resource.ParticipantTwoId);
    }
}
