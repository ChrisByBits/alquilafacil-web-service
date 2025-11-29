using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Chat.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.Chat.Interfaces.REST.Transform;

public static class ChatMetricResourceFromEntityAssembler
{
    public static ChatMetricResource ToResourceFromEntity(ChatResponseMetric metric)
    {
        return new ChatMetricResource(
            metric.Id,
            metric.UserId,
            metric.ConversationId,
            metric.FirstMessageAt,
            metric.FirstResponseAt,
            metric.FirstResponseTime?.TotalMinutes
        );
    }
}
