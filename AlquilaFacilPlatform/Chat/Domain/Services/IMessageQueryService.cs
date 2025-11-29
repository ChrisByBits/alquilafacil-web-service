using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Chat.Domain.Model.Queries;

namespace AlquilaFacilPlatform.Chat.Domain.Services;

public interface IMessageQueryService
{
    Task<IEnumerable<Message>> Handle(GetMessagesByConversationIdQuery query);
    Task<int> Handle(GetUnreadMessageCountQuery query);
}
