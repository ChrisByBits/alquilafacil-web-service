using System.Net.Mime;
using AlquilaFacilPlatform.Chat.Domain.Model.Commands;
using AlquilaFacilPlatform.Chat.Domain.Model.Queries;
using AlquilaFacilPlatform.Chat.Domain.Services;
using AlquilaFacilPlatform.Chat.Interfaces.REST.Resources;
using AlquilaFacilPlatform.Chat.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlquilaFacilPlatform.Chat.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
public class ConversationsController(
    IConversationCommandService conversationCommandService,
    IConversationQueryService conversationQueryService,
    IMessageCommandService messageCommandService,
    IMessageQueryService messageQueryService,
    IChatMetricsQueryService chatMetricsQueryService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateConversation([FromBody] CreateConversationResource resource)
    {
        var command = CreateConversationCommandFromResourceAssembler.ToCommandFromResource(resource);
        var conversation = await conversationCommandService.Handle(command);

        if (conversation == null)
            return BadRequest(new { message = "Cannot create conversation" });

        var conversationResource = ConversationResourceFromEntityAssembler.ToResourceFromEntity(conversation);
        return CreatedAtAction(nameof(GetConversationById), new { conversationId = conversation.Id }, conversationResource);
    }

    [HttpGet("{conversationId:int}")]
    public async Task<IActionResult> GetConversationById(int conversationId)
    {
        var query = new GetConversationByIdQuery(conversationId);
        var conversation = await conversationQueryService.Handle(query);

        if (conversation == null)
            return NotFound();

        var resource = ConversationResourceFromEntityAssembler.ToResourceFromEntity(conversation);
        return Ok(resource);
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetConversationsByUserId(int userId)
    {
        var query = new GetConversationsByUserIdQuery(userId);
        var conversations = await conversationQueryService.Handle(query);

        var resources = conversations.Select(ConversationResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("between/{userOneId:int}/{userTwoId:int}")]
    public async Task<IActionResult> GetConversationBetweenUsers(int userOneId, int userTwoId)
    {
        var query = new GetConversationBetweenUsersQuery(userOneId, userTwoId);
        var conversation = await conversationQueryService.Handle(query);

        if (conversation == null)
            return NotFound();

        var resource = ConversationResourceFromEntityAssembler.ToResourceFromEntity(conversation);
        return Ok(resource);
    }

    [HttpGet("{conversationId:int}/messages")]
    public async Task<IActionResult> GetMessagesByConversationId(
        int conversationId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 50)
    {
        var query = new GetMessagesByConversationIdQuery(conversationId, pageNumber, pageSize);
        var messages = await messageQueryService.Handle(query);

        var resources = messages.Select(MessageResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpPost("messages")]
    public async Task<IActionResult> SendMessage([FromBody] SendMessageResource resource)
    {
        var command = SendMessageCommandFromResourceAssembler.ToCommandFromResource(resource);
        var message = await messageCommandService.Handle(command);

        if (message == null)
            return BadRequest(new { message = "Cannot send message" });

        var messageResource = MessageResourceFromEntityAssembler.ToResourceFromEntity(message);
        return Ok(messageResource);
    }

    [HttpPost("{conversationId:int}/read")]
    public async Task<IActionResult> MarkMessagesAsRead(int conversationId, [FromQuery] int userId)
    {
        var command = new MarkMessagesAsReadCommand(conversationId, userId);
        await messageCommandService.Handle(command);

        return Ok(new { message = "Messages marked as read" });
    }

    [HttpGet("unread/{userId:int}")]
    public async Task<IActionResult> GetUnreadMessageCount(int userId)
    {
        var query = new GetUnreadMessageCountQuery(userId);
        var count = await messageQueryService.Handle(query);

        return Ok(new { unreadCount = count });
    }

    [HttpPut("messages/{messageId:int}")]
    public async Task<IActionResult> EditMessage(int messageId, [FromBody] EditMessageResource resource)
    {
        var command = new EditMessageCommand(messageId, resource.SenderId, resource.NewContent);
        var message = await messageCommandService.Handle(command);

        if (message == null)
            return BadRequest(new { message = "Cannot edit message. Message not found or you don't have permission." });

        var messageResource = MessageResourceFromEntityAssembler.ToResourceFromEntity(message);
        return Ok(messageResource);
    }

    [HttpDelete("messages/{messageId:int}")]
    public async Task<IActionResult> DeleteMessage(int messageId, [FromBody] DeleteMessageResource resource)
    {
        var command = new DeleteMessageCommand(messageId, resource.SenderId);
        var success = await messageCommandService.Handle(command);

        if (!success)
            return BadRequest(new { message = "Cannot delete message. Message not found or you don't have permission." });

        return Ok(new { message = "Message deleted successfully" });
    }

    [HttpPost("messages/{messageId:int}/reactions")]
    public async Task<IActionResult> AddReaction(int messageId, [FromBody] MessageReactionResource resource)
    {
        var command = new AddMessageReactionCommand(messageId, resource.UserId, resource.Emoji);
        var message = await messageCommandService.Handle(command);

        if (message == null)
            return BadRequest(new { message = "Cannot add reaction. Message not found or deleted." });

        var messageResource = MessageResourceFromEntityAssembler.ToResourceFromEntity(message);
        return Ok(messageResource);
    }

    [HttpDelete("messages/{messageId:int}/reactions")]
    public async Task<IActionResult> RemoveReaction(int messageId, [FromBody] MessageReactionResource resource)
    {
        var command = new RemoveMessageReactionCommand(messageId, resource.UserId, resource.Emoji);
        var message = await messageCommandService.Handle(command);

        if (message == null)
            return BadRequest(new { message = "Cannot remove reaction. Message not found." });

        var messageResource = MessageResourceFromEntityAssembler.ToResourceFromEntity(message);
        return Ok(messageResource);
    }

    [HttpGet("metrics/user/{userId:int}")]
    public async Task<IActionResult> GetChatMetricsByUserId(int userId)
    {
        var query = new GetChatMetricsByUserIdQuery(userId);
        var metrics = await chatMetricsQueryService.Handle(query);

        var resources = metrics.Select(ChatMetricResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("metrics/user/{userId:int}/average-response-time")]
    public async Task<IActionResult> GetAverageResponseTime(int userId)
    {
        var query = new GetAverageResponseTimeQuery(userId);
        var averageMinutes = await chatMetricsQueryService.Handle(query);

        return Ok(new { userId, averageResponseTimeMinutes = averageMinutes });
    }
}
