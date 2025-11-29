using AlquilaFacilPlatform.Profiles.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Profiles.Domain.Model.Commands;

namespace AlquilaFacilPlatform.Profiles.Domain.Services;

public interface IProfileCommandService
{
    public Task<Profile?> Handle(CreateProfileCommand command);
    public Task<Profile> Handle(UpdateProfileCommand command);
    public Task<Profile> Handle(UpdateProfileAvatarCommand command);
}