using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Chat.Domain.Repositories;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlquilaFacilPlatform.Chat.Infrastructure.Persistence.EFC.Repositories;

public class ChatResponseMetricRepository(AppDbContext context)
    : BaseRepository<ChatResponseMetric>(context), IChatResponseMetricRepository
{
    public async Task<ChatResponseMetric?> FindByConversationIdAsync(int conversationId)
    {
        return await Context.Set<ChatResponseMetric>()
            .FirstOrDefaultAsync(m => m.ConversationId == conversationId);
    }

    public async Task<IEnumerable<ChatResponseMetric>> FindByUserIdAsync(int userId)
    {
        return await Context.Set<ChatResponseMetric>()
            .Where(m => m.UserId == userId)
            .OrderByDescending(m => m.FirstMessageAt)
            .ToListAsync();
    }

    public async Task<double> GetAverageResponseTimeByUserIdAsync(int userId)
    {
        var metrics = await Context.Set<ChatResponseMetric>()
            .Where(m => m.UserId == userId && m.FirstResponseTime != null)
            .ToListAsync();

        if (!metrics.Any())
            return 0;

        return metrics.Average(m => m.FirstResponseTime!.Value.TotalMinutes);
    }
}
