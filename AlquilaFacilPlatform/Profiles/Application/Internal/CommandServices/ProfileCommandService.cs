using AlquilaFacilPlatform.Profiles.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Profiles.Domain.Model.Commands;
using AlquilaFacilPlatform.Profiles.Domain.Repositories;
using AlquilaFacilPlatform.Profiles.Domain.Services;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.Profiles.Application.Internal.CommandServices;

public class ProfileCommandService(IProfileRepository profileRepository, IUnitOfWork unitOfWork) : IProfileCommandService
{
    public async Task<Profile?> Handle(CreateProfileCommand command)
    {
        var profile = new Profile(command);
        await profileRepository.AddAsync(profile);
        await unitOfWork.CompleteAsync();
        return profile;
    }

    public async Task<Profile> Handle(UpdateProfileCommand command)
    {
        var profile = await profileRepository.FindByUserIdAsync(command.UserId);
        if (profile == null)
        {
            throw new Exception("Profile with User ID does not exist");
        }
        profile.Update(command);
        await unitOfWork.CompleteAsync();
        return profile;
    }

    public async Task<Profile> Handle(UpdateProfileAvatarCommand command)
    {
        var profile = await profileRepository.FindByIdAsync(command.ProfileId);
        if (profile == null)
        {
            throw new Exception("Profile does not exist");
        }
        profile.UpdateAvatar(command.AvatarUrl);
        await unitOfWork.CompleteAsync();
        return profile;
    }
}