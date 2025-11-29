using AlquilaFacilPlatform.Chat.Domain.Model.Commands;
using AlquilaFacilPlatform.Chat.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.Chat.Interfaces.REST.Transform;

public static class SendMessageCommandFromResourceAssembler
{
    public static SendMessageCommand ToCommandFromResource(SendMessageResource resource)
    {
        return new SendMessageCommand(resource.ConversationId, resource.SenderId, resource.Content);
    }
}
