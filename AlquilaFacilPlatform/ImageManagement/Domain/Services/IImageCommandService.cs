using AlquilaFacilPlatform.ImageManagement.Domain.Model.Aggregates;
using AlquilaFacilPlatform.ImageManagement.Domain.Model.Commands;

namespace AlquilaFacilPlatform.ImageManagement.Domain.Services;

public interface IImageCommandService
{
    Task<Image?> Handle(UploadImageCommand command);
    Task<bool> Handle(DeleteImageCommand command);
}
