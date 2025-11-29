using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.Chat.Domain.Repositories;

public interface IMessageRepository : IBaseRepository<Message>
{
    Task<Message?> FindByIdAsync(int messageId);
    Task<IEnumerable<Message>> FindByConversationIdAsync(int conversationId, int pageNumber, int pageSize);
    Task<int> CountUnreadByUserIdAsync(int userId);
    Task MarkAsReadAsync(int conversationId, int userId);
}
