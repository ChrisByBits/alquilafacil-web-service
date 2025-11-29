using AlquilaFacilPlatform.Chat.Domain.Model.Commands;

namespace AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;

public class Conversation
{
    public int Id { get; set; }
    public int ParticipantOneId { get; set; }
    public int ParticipantTwoId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? LastMessageAt { get; set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();

    public Conversation()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public Conversation(int participantOneId, int participantTwoId) : this()
    {
        ParticipantOneId = participantOneId;
        ParticipantTwoId = participantTwoId;
    }

    public Conversation(CreateConversationCommand command) : this()
    {
        ParticipantOneId = command.ParticipantOneId;
        ParticipantTwoId = command.ParticipantTwoId;
    }

    public bool HasParticipant(int userId)
    {
        return ParticipantOneId == userId || ParticipantTwoId == userId;
    }

    public int GetOtherParticipantId(int userId)
    {
        return ParticipantOneId == userId ? ParticipantTwoId : ParticipantOneId;
    }

    public void UpdateLastMessageTime()
    {
        LastMessageAt = DateTime.UtcNow;
    }
}
