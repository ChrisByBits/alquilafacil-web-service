using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Chat.Domain.Model.Queries;

namespace AlquilaFacilPlatform.Chat.Domain.Services;

public interface IChatMetricsQueryService
{
    Task<IEnumerable<ChatResponseMetric>> Handle(GetChatMetricsByUserIdQuery query);
    Task<double> Handle(GetAverageResponseTimeQuery query);
}
