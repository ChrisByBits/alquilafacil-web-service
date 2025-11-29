using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Chat.Domain.Model.Queries;
using AlquilaFacilPlatform.Chat.Domain.Repositories;
using AlquilaFacilPlatform.Chat.Domain.Services;

namespace AlquilaFacilPlatform.Chat.Application.Internal.QueryServices;

public class ConversationQueryService(IConversationRepository conversationRepository)
    : IConversationQueryService
{
    public async Task<Conversation?> Handle(GetConversationByIdQuery query)
    {
        return await conversationRepository.FindByIdWithMessagesAsync(query.ConversationId);
    }

    public async Task<IEnumerable<Conversation>> Handle(GetConversationsByUserIdQuery query)
    {
        return await conversationRepository.FindByUserIdAsync(query.UserId);
    }

    public async Task<Conversation?> Handle(GetConversationBetweenUsersQuery query)
    {
        return await conversationRepository.FindBetweenUsersAsync(query.UserOneId, query.UserTwoId);
    }
}
