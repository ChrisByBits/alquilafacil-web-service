using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Chat.Domain.Repositories;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlquilaFacilPlatform.Chat.Infrastructure.Persistence.EFC.Repositories;

public class MessageRepository(AppDbContext context)
    : BaseRepository<Message>(context), IMessageRepository
{
    public async Task<Message?> FindByIdAsync(int messageId)
    {
        return await Context.Set<Message>()
            .FirstOrDefaultAsync(m => m.Id == messageId);
    }

    public async Task<IEnumerable<Message>> FindByConversationIdAsync(int conversationId, int pageNumber, int pageSize)
    {
        return await Context.Set<Message>()
            .Where(m => m.ConversationId == conversationId)
            .OrderByDescending(m => m.SentAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<int> CountUnreadByUserIdAsync(int userId)
    {
        return await Context.Set<Message>()
            .Include(m => m.Conversation)
            .Where(m => m.Conversation != null &&
                        (m.Conversation.ParticipantOneId == userId || m.Conversation.ParticipantTwoId == userId) &&
                        m.SenderId != userId &&
                        !m.IsRead)
            .CountAsync();
    }

    public async Task MarkAsReadAsync(int conversationId, int userId)
    {
        var unreadMessages = await Context.Set<Message>()
            .Where(m => m.ConversationId == conversationId &&
                        m.SenderId != userId &&
                        !m.IsRead)
            .ToListAsync();

        foreach (var message in unreadMessages)
        {
            message.MarkAsRead();
        }
    }
}
