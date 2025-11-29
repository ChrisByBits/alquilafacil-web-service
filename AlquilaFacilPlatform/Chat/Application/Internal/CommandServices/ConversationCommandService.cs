using AlquilaFacilPlatform.Chat.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Chat.Domain.Model.Commands;
using AlquilaFacilPlatform.Chat.Domain.Repositories;
using AlquilaFacilPlatform.Chat.Domain.Services;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.Chat.Application.Internal.CommandServices;

public class ConversationCommandService(
    IConversationRepository conversationRepository,
    IUnitOfWork unitOfWork) : IConversationCommandService
{
    public async Task<Conversation?> Handle(CreateConversationCommand command)
    {
        // Check if conversation already exists between users
        var existingConversation = await conversationRepository
            .FindBetweenUsersAsync(command.ParticipantOneId, command.ParticipantTwoId);

        if (existingConversation != null)
            return existingConversation;

        // Prevent self-conversation
        if (command.ParticipantOneId == command.ParticipantTwoId)
            return null;

        var conversation = new Conversation(command);

        await conversationRepository.AddAsync(conversation);
        await unitOfWork.CompleteAsync();

        return conversation;
    }
}
