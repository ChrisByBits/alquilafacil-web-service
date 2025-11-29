using AlquilaFacilPlatform.Chat.Domain.Model.Commands;

namespace AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;

public class Message
{
    public int Id { get; set; }
    public int ConversationId { get; set; }
    public int SenderId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime SentAt { get; set; }
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }

    // File/Image attachment support
    public string? AttachmentUrl { get; set; }
    public string? AttachmentType { get; set; } // "image", "file"
    public string? AttachmentFileName { get; set; }
    public long? AttachmentFileSizeBytes { get; set; }

    // Message editing and deletion
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }

    // Reactions support
    public string? Reactions { get; set; } // JSON string: {"👍": [1,2,3], "❤️": [4,5]}

    public Conversation? Conversation { get; set; }

    public Message()
    {
        SentAt = DateTime.UtcNow;
        IsRead = false;
    }

    public Message(int conversationId, int senderId, string content) : this()
    {
        ConversationId = conversationId;
        SenderId = senderId;
        Content = content;
    }

    public Message(SendMessageCommand command) : this()
    {
        ConversationId = command.ConversationId;
        SenderId = command.SenderId;
        Content = command.Content;
        AttachmentUrl = command.AttachmentUrl;
        AttachmentType = command.AttachmentType;
        AttachmentFileName = command.AttachmentFileName;
        AttachmentFileSizeBytes = command.AttachmentFileSizeBytes;
    }

    public void MarkAsRead()
    {
        if (!IsRead)
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }
    }

    public void Edit(string newContent)
    {
        if (!IsDeleted)
        {
            Content = newContent;
            IsEdited = true;
            EditedAt = DateTime.UtcNow;
        }
    }

    public void Delete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        Content = string.Empty; // Clear content for privacy
    }

    public void AddAttachment(string url, string type, string fileName, long fileSizeBytes)
    {
        AttachmentUrl = url;
        AttachmentType = type;
        AttachmentFileName = fileName;
        AttachmentFileSizeBytes = fileSizeBytes;
    }
}
