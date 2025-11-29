using AlquilaFacilPlatform.Locals.Domain.Model.Commands;

namespace AlquilaFacilPlatform.Locals.Domain.Services;

public interface ISeedLocalsCommandService
{
    Task Handle(SeedLocalsCommand command);
}
