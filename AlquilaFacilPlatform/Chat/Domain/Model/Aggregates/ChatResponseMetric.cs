namespace AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;

public class ChatResponseMetric
{
    public int Id { get; set; }
    public int UserId { get; set; } // The user being measured (typically landlord)
    public int ConversationId { get; set; }
    public DateTime FirstMessageAt { get; set; } // When the conversation started or inquiry was sent
    public DateTime? FirstResponseAt { get; set; } // When the user first responded
    public TimeSpan? FirstResponseTime { get; set; } // Calculated time to first response
    public DateTime LastCalculatedAt { get; set; } // Last time metrics were updated

    public Conversation? Conversation { get; set; }

    public ChatResponseMetric()
    {
        LastCalculatedAt = DateTime.UtcNow;
    }

    public ChatResponseMetric(int userId, int conversationId, DateTime firstMessageAt) : this()
    {
        UserId = userId;
        ConversationId = conversationId;
        FirstMessageAt = firstMessageAt;
    }

    public void SetFirstResponse(DateTime responseTime)
    {
        if (!FirstResponseAt.HasValue)
        {
            FirstResponseAt = responseTime;
            FirstResponseTime = responseTime - FirstMessageAt;
            LastCalculatedAt = DateTime.UtcNow;
        }
    }

    public void RecalculateMetrics()
    {
        if (FirstResponseAt.HasValue)
        {
            FirstResponseTime = FirstResponseAt.Value - FirstMessageAt;
        }
        LastCalculatedAt = DateTime.UtcNow;
    }
}
