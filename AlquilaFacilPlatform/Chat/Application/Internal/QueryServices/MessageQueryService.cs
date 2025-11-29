using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Chat.Domain.Model.Queries;
using AlquilaFacilPlatform.Chat.Domain.Repositories;
using AlquilaFacilPlatform.Chat.Domain.Services;

namespace AlquilaFacilPlatform.Chat.Application.Internal.QueryServices;

public class MessageQueryService(IMessageRepository messageRepository)
    : IMessageQueryService
{
    public async Task<IEnumerable<Message>> Handle(GetMessagesByConversationIdQuery query)
    {
        return await messageRepository.FindByConversationIdAsync(
            query.ConversationId, query.PageNumber, query.PageSize);
    }

    public async Task<int> Handle(GetUnreadMessageCountQuery query)
    {
        return await messageRepository.CountUnreadByUserIdAsync(query.UserId);
    }
}
