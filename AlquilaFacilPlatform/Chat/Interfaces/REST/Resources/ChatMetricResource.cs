namespace AlquilaFacilPlatform.Chat.Interfaces.REST.Resources;

public record ChatMetricResource(
    int Id,
    int UserId,
    int ConversationId,
    DateTime FirstMessageAt,
    DateTime? FirstResponseAt,
    double? FirstResponseTimeMinutes
);
