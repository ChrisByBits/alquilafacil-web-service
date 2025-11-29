namespace AlquilaFacilPlatform.Chat.Domain.Model.Commands;

public record SendMessageCommand(
    int ConversationId,
    int SenderId,
    string Content,
    string? AttachmentUrl = null,
    string? AttachmentType = null,
    string? AttachmentFileName = null,
    long? AttachmentFileSizeBytes = null
);
