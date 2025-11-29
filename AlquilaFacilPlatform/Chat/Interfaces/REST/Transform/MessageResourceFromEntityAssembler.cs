using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Chat.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.Chat.Interfaces.REST.Transform;

public static class MessageResourceFromEntityAssembler
{
    public static MessageResource ToResourceFromEntity(Message message)
    {
        return new MessageResource(
            message.Id,
            message.ConversationId,
            message.SenderId,
            message.Content,
            message.SentAt,
            message.IsRead,
            message.ReadAt,
            message.AttachmentUrl,
            message.AttachmentType,
            message.AttachmentFileName,
            message.AttachmentFileSizeBytes,
            message.IsEdited,
            message.EditedAt,
            message.IsDeleted,
            message.DeletedAt,
            message.Reactions
        );
    }
}
