using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.Chat.Domain.Repositories;

public interface IChatResponseMetricRepository : IBaseRepository<ChatResponseMetric>
{
    Task<ChatResponseMetric?> FindByConversationIdAsync(int conversationId);
    Task<IEnumerable<ChatResponseMetric>> FindByUserIdAsync(int userId);
    Task<double> GetAverageResponseTimeByUserIdAsync(int userId);
}
