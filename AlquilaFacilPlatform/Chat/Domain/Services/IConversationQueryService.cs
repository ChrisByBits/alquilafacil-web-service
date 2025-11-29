using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Chat.Domain.Model.Queries;

namespace AlquilaFacilPlatform.Chat.Domain.Services;

public interface IConversationQueryService
{
    Task<Conversation?> Handle(GetConversationByIdQuery query);
    Task<IEnumerable<Conversation>> Handle(GetConversationsByUserIdQuery query);
    Task<Conversation?> Handle(GetConversationBetweenUsersQuery query);
}
