using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Chat.Domain.Model.Commands;

namespace AlquilaFacilPlatform.Chat.Domain.Services;

public interface IConversationCommandService
{
    Task<Conversation?> Handle(CreateConversationCommand command);
}
