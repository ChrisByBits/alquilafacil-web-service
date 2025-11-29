using AlquilaFacilPlatform.Locals.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.Locals.Domain.Repositories;

public interface ILocalRepository : IBaseRepository<Local>
{
    Task<IEnumerable<Local>> GetLocalsAsync();
    Task<Local?> GetLocalByIdAsync(int localId);
    Task<HashSet<string>> GetAllDistrictsAsync();
    Task<IEnumerable<Local>> GetLocalsByCategoryIdAndCapacityRange(int categoryId, int minCapacity, int maxCapacity);
    Task<IEnumerable<Local>> GetLocalsByUserIdAsync(int userId);
    Task<bool> IsOwnerAsync(int userId, int localId);
    Task<int?> GetLocalOwnerIdByLocalId(int localId);
    Task<Local?> FindByIdAsync(int id);
    Task<IEnumerable<Local>> FindByLocalCategoryIdAsync(int categoryId);
    Task<IEnumerable<Local>> GetAllAsync();
}