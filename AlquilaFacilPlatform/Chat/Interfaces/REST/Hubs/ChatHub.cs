using AlquilaFacilPlatform.Chat.Domain.Model.Commands;
using AlquilaFacilPlatform.Chat.Domain.Services;
using AlquilaFacilPlatform.Chat.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace AlquilaFacilPlatform.Chat.Interfaces.REST.Hubs;

[Authorize]
public class ChatHub(
    IMessageCommandService messageCommandService,
    IConversationQueryService conversationQueryService) : Hub
{
    public override async Task OnConnectedAsync()
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
        await base.OnConnectedAsync();
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var userId = Context.UserIdentifier;
        if (!string.IsNullOrEmpty(userId))
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user_{userId}");
        }
        await base.OnDisconnectedAsync(exception);
    }

    public async Task JoinConversation(int conversationId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
    }

    public async Task LeaveConversation(int conversationId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"conversation_{conversationId}");
    }

    public async Task SendMessage(int conversationId, int senderId, string content)
    {
        var command = new SendMessageCommand(conversationId, senderId, content);
        var message = await messageCommandService.Handle(command);

        if (message != null)
        {
            var messageResource = MessageResourceFromEntityAssembler.ToResourceFromEntity(message);

            // Send to all users in the conversation
            await Clients.Group($"conversation_{conversationId}").SendAsync("ReceiveMessage", messageResource);

            // Notify both participants about the new message
            var conversation = await conversationQueryService.Handle(
                new Domain.Model.Queries.GetConversationByIdQuery(conversationId));

            if (conversation != null)
            {
                var recipientId = conversation.GetOtherParticipantId(senderId);
                await Clients.Group($"user_{recipientId}").SendAsync("NewMessageNotification", new
                {
                    conversationId,
                    senderId,
                    preview = content.Length > 50 ? content[..50] + "..." : content
                });
            }
        }
    }

    public async Task MarkAsRead(int conversationId, int userId)
    {
        var command = new MarkMessagesAsReadCommand(conversationId, userId);
        await messageCommandService.Handle(command);

        await Clients.Group($"conversation_{conversationId}").SendAsync("MessagesRead", new
        {
            conversationId,
            readByUserId = userId
        });
    }

    public async Task StartTyping(int conversationId, int userId)
    {
        await Clients.OthersInGroup($"conversation_{conversationId}").SendAsync("UserTyping", new
        {
            conversationId,
            userId
        });
    }

    public async Task StopTyping(int conversationId, int userId)
    {
        await Clients.OthersInGroup($"conversation_{conversationId}").SendAsync("UserStoppedTyping", new
        {
            conversationId,
            userId
        });
    }

    public async Task EditMessage(int messageId, int senderId, string newContent)
    {
        var command = new EditMessageCommand(messageId, senderId, newContent);
        var message = await messageCommandService.Handle(command);

        if (message != null)
        {
            var messageResource = MessageResourceFromEntityAssembler.ToResourceFromEntity(message);
            await Clients.Group($"conversation_{message.ConversationId}").SendAsync("MessageEdited", messageResource);
        }
    }

    public async Task DeleteMessage(int messageId, int senderId)
    {
        var message = await messageCommandService.Handle(new EditMessageCommand(messageId, senderId, ""));
        if (message != null)
        {
            var success = await messageCommandService.Handle(new DeleteMessageCommand(messageId, senderId));

            if (success)
            {
                await Clients.Group($"conversation_{message.ConversationId}").SendAsync("MessageDeleted", new
                {
                    messageId,
                    conversationId = message.ConversationId
                });
            }
        }
    }

    public async Task AddReaction(int messageId, int userId, string emoji)
    {
        var command = new AddMessageReactionCommand(messageId, userId, emoji);
        var message = await messageCommandService.Handle(command);

        if (message != null)
        {
            var messageResource = MessageResourceFromEntityAssembler.ToResourceFromEntity(message);
            await Clients.Group($"conversation_{message.ConversationId}").SendAsync("ReactionAdded", messageResource);
        }
    }

    public async Task RemoveReaction(int messageId, int userId, string emoji)
    {
        var command = new RemoveMessageReactionCommand(messageId, userId, emoji);
        var message = await messageCommandService.Handle(command);

        if (message != null)
        {
            var messageResource = MessageResourceFromEntityAssembler.ToResourceFromEntity(message);
            await Clients.Group($"conversation_{message.ConversationId}").SendAsync("ReactionRemoved", messageResource);
        }
    }
}
