using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Chat.Domain.Repositories;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlquilaFacilPlatform.Chat.Infrastructure.Persistence.EFC.Repositories;

public class ConversationRepository(AppDbContext context)
    : BaseRepository<Conversation>(context), IConversationRepository
{
    public async Task<Conversation?> FindByIdWithMessagesAsync(int id)
    {
        return await Context.Set<Conversation>()
            .Include(c => c.Messages.OrderByDescending(m => m.SentAt).Take(50))
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Conversation>> FindByUserIdAsync(int userId)
    {
        return await Context.Set<Conversation>()
            .Where(c => c.ParticipantOneId == userId || c.ParticipantTwoId == userId)
            .OrderByDescending(c => c.LastMessageAt ?? c.CreatedAt)
            .ToListAsync();
    }

    public async Task<Conversation?> FindBetweenUsersAsync(int userOneId, int userTwoId)
    {
        return await Context.Set<Conversation>()
            .FirstOrDefaultAsync(c =>
                (c.ParticipantOneId == userOneId && c.ParticipantTwoId == userTwoId) ||
                (c.ParticipantOneId == userTwoId && c.ParticipantTwoId == userOneId));
    }
}
