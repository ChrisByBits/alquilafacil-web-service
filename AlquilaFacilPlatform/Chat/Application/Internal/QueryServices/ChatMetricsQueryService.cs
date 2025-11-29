using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Chat.Domain.Model.Queries;
using AlquilaFacilPlatform.Chat.Domain.Repositories;
using AlquilaFacilPlatform.Chat.Domain.Services;

namespace AlquilaFacilPlatform.Chat.Application.Internal.QueryServices;

public class ChatMetricsQueryService(IChatResponseMetricRepository chatResponseMetricRepository) : IChatMetricsQueryService
{
    public async Task<IEnumerable<ChatResponseMetric>> Handle(GetChatMetricsByUserIdQuery query)
    {
        return await chatResponseMetricRepository.FindByUserIdAsync(query.UserId);
    }

    public async Task<double> Handle(GetAverageResponseTimeQuery query)
    {
        return await chatResponseMetricRepository.GetAverageResponseTimeByUserIdAsync(query.UserId);
    }
}
