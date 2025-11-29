using AlquilaFacilPlatform.ImageManagement.Domain.Model.Aggregates;
using AlquilaFacilPlatform.ImageManagement.Domain.Model.Commands;
using AlquilaFacilPlatform.ImageManagement.Domain.Repositories;
using AlquilaFacilPlatform.ImageManagement.Domain.Services;
using AlquilaFacilPlatform.ImageManagement.Application.Internal.OutboundServices;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.ImageManagement.Application.Internal.CommandServices;

public class ImageCommandService(
    IImageRepository imageRepository,
    IImageStorageService imageStorageService,
    IUnitOfWork unitOfWork) : IImageCommandService
{
    public async Task<Image?> Handle(UploadImageCommand command)
    {
        var image = new Image(
            command.Url,
            command.FileName,
            command.ContentType,
            command.FileSizeBytes,
            command.StoragePath,
            command.EntityType,
            command.EntityId,
            command.UploadedBy,
            command.Width,
            command.Height
        );

        await imageRepository.AddAsync(image);
        await unitOfWork.CompleteAsync();

        return image;
    }

    public async Task<bool> Handle(DeleteImageCommand command)
    {
        var image = await imageRepository.FindByIdAsync(command.ImageId);
        if (image == null) return false;

        // Delete from storage
        await imageStorageService.DeleteImageAsync(image.StoragePath);

        // Mark as deleted in database
        image.MarkAsDeleted();
        await unitOfWork.CompleteAsync();

        return true;
    }
}
