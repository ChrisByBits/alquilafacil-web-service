using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Chat.Domain.Model.Commands;
using AlquilaFacilPlatform.Chat.Domain.Repositories;
using AlquilaFacilPlatform.Chat.Domain.Services;
using AlquilaFacilPlatform.Shared.Domain.Repositories;
using System.Text.Json;

namespace AlquilaFacilPlatform.Chat.Application.Internal.CommandServices;

public class MessageCommandService(
    IMessageRepository messageRepository,
    IConversationRepository conversationRepository,
    IUnitOfWork unitOfWork) : IMessageCommandService
{
    public async Task<Message?> Handle(SendMessageCommand command)
    {
        var conversation = await conversationRepository.FindByIdAsync(command.ConversationId);

        if (conversation == null)
            return null;

        // Verify sender is participant
        if (!conversation.HasParticipant(command.SenderId))
            return null;

        var message = new Message(command);

        await messageRepository.AddAsync(message);
        conversation.UpdateLastMessageTime();
        await unitOfWork.CompleteAsync();

        return message;
    }

    public async Task Handle(MarkMessagesAsReadCommand command)
    {
        var conversation = await conversationRepository.FindByIdAsync(command.ConversationId);

        if (conversation == null || !conversation.HasParticipant(command.UserId))
            return;

        await messageRepository.MarkAsReadAsync(command.ConversationId, command.UserId);
        await unitOfWork.CompleteAsync();
    }

    public async Task<Message?> Handle(EditMessageCommand command)
    {
        var message = await messageRepository.FindByIdAsync(command.MessageId);

        if (message == null)
            return null;

        // Verify sender owns the message
        if (message.SenderId != command.SenderId)
            return null;

        // Cannot edit deleted messages
        if (message.IsDeleted)
            return null;

        message.Edit(command.NewContent);
        await unitOfWork.CompleteAsync();

        return message;
    }

    public async Task<bool> Handle(DeleteMessageCommand command)
    {
        var message = await messageRepository.FindByIdAsync(command.MessageId);

        if (message == null)
            return false;

        // Verify sender owns the message
        if (message.SenderId != command.SenderId)
            return false;

        message.Delete();
        await unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<Message?> Handle(AddMessageReactionCommand command)
    {
        var message = await messageRepository.FindByIdAsync(command.MessageId);

        if (message == null || message.IsDeleted)
            return null;

        // Parse existing reactions or create new
        var reactions = string.IsNullOrEmpty(message.Reactions)
            ? new Dictionary<string, List<int>>()
            : JsonSerializer.Deserialize<Dictionary<string, List<int>>>(message.Reactions)
              ?? new Dictionary<string, List<int>>();

        // Add user to emoji reaction list
        if (!reactions.ContainsKey(command.Emoji))
            reactions[command.Emoji] = new List<int>();

        if (!reactions[command.Emoji].Contains(command.UserId))
            reactions[command.Emoji].Add(command.UserId);

        message.Reactions = JsonSerializer.Serialize(reactions);
        await unitOfWork.CompleteAsync();

        return message;
    }

    public async Task<Message?> Handle(RemoveMessageReactionCommand command)
    {
        var message = await messageRepository.FindByIdAsync(command.MessageId);

        if (message == null || string.IsNullOrEmpty(message.Reactions))
            return null;

        var reactions = JsonSerializer.Deserialize<Dictionary<string, List<int>>>(message.Reactions);

        if (reactions == null || !reactions.ContainsKey(command.Emoji))
            return message;

        reactions[command.Emoji].Remove(command.UserId);

        // Remove emoji key if no users left
        if (reactions[command.Emoji].Count == 0)
            reactions.Remove(command.Emoji);

        message.Reactions = reactions.Count > 0 ? JsonSerializer.Serialize(reactions) : null;
        await unitOfWork.CompleteAsync();

        return message;
    }
}
