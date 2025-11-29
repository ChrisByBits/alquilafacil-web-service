using AlquilaFacilPlatform.ImageManagement.Domain.Model.Aggregates;
using AlquilaFacilPlatform.ImageManagement.Domain.Model.Queries;

namespace AlquilaFacilPlatform.ImageManagement.Domain.Services;

public interface IImageQueryService
{
    Task<Image?> Handle(GetImageByIdQuery query);
    Task<IEnumerable<Image>> Handle(GetImagesByEntityQuery query);
}
