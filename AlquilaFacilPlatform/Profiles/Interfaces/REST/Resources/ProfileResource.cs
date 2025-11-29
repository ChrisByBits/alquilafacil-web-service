using AlquilaFacilPlatform.IAM.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.Profiles.Interfaces.REST.Resources;

public record ProfileResource(
    int Id,
    string FullName,
    string Phone,
    string DocumentNumber,
    string DateOfBirth,
    string AvatarUrl
);