namespace AlquilaFacilPlatform.Chat.Interfaces.REST.Resources;

public record MessageResource(
    int Id,
    int ConversationId,
    int SenderId,
    string Content,
    DateTime SentAt,
    bool IsRead,
    DateTime? ReadAt,
    string? AttachmentUrl,
    string? AttachmentType,
    string? AttachmentFileName,
    long? AttachmentFileSizeBytes,
    bool IsEdited,
    DateTime? EditedAt,
    bool IsDeleted,
    DateTime? DeletedAt,
    string? Reactions
);
