using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.Chat.Domain.Repositories;

public interface IConversationRepository : IBaseRepository<Conversation>
{
    Task<Conversation?> FindByIdWithMessagesAsync(int id);
    Task<IEnumerable<Conversation>> FindByUserIdAsync(int userId);
    Task<Conversation?> FindBetweenUsersAsync(int userOneId, int userTwoId);
}
