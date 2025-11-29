using AlquilaFacilPlatform.ImageManagement.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.ImageManagement.Domain.Repositories;

public interface IImageRepository : IBaseRepository<Image>
{
    Task<IEnumerable<Image>> FindByEntityTypeAndIdAsync(string entityType, int entityId);
    Task<Image?> FindByIdAsync(int id);
}
