using AlquilaFacilPlatform.ImageManagement.Domain.Model.Aggregates;
using AlquilaFacilPlatform.ImageManagement.Domain.Model.Queries;
using AlquilaFacilPlatform.ImageManagement.Domain.Repositories;
using AlquilaFacilPlatform.ImageManagement.Domain.Services;

namespace AlquilaFacilPlatform.ImageManagement.Application.Internal.QueryServices;

public class ImageQueryService(IImageRepository imageRepository) : IImageQueryService
{
    public async Task<Image?> Handle(GetImageByIdQuery query)
    {
        return await imageRepository.FindByIdAsync(query.ImageId);
    }

    public async Task<IEnumerable<Image>> Handle(GetImagesByEntityQuery query)
    {
        return await imageRepository.FindByEntityTypeAndIdAsync(query.EntityType, query.EntityId);
    }
}
