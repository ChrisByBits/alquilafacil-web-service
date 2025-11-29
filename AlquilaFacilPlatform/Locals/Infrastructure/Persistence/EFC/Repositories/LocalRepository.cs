using AlquilaFacilPlatform.Locals.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Locals.Domain.Repositories;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlquilaFacilPlatform.Locals.Infrastructure.Persistence.EFC.Repositories;

public class LocalRepository(AppDbContext context) : BaseRepository<Local>(context), ILocalRepository
{

    public async Task<IEnumerable<Local>> GetLocalsAsync()
    {
        return await Context.Set<Local>()
            .Include(x => x.LocalPhotos)
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }
    
    public async Task<HashSet<string>> GetAllDistrictsAsync()
    {
        var districtsInfo = await Context.Set<Local>()
            .Select(x => $"{x.District.Value}, {x.City.Value}, {x.Country.Value}")
            .Distinct()
            .ToListAsync();

        return [..districtsInfo];
    }
    
    public async Task<Local?> GetLocalByIdAsync(int localId)
    {
        return await Context.Set<Local>()
            .Include(x => x.LocalPhotos)
            .FirstOrDefaultAsync(x => x.Id == localId);
    }

    public async Task<IEnumerable<Local>> GetLocalsByCategoryIdAndCapacityRange(int categoryId, int minCapacity, int maxCapacity)
    {
        return await Context.Set<Local>()
            .Where(x => x.LocalCategoryId == categoryId && x.Capacity >= minCapacity && x.Capacity <= maxCapacity)
            .Include(x => x.LocalPhotos)
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<Local>> GetLocalsByUserIdAsync(int userId)
    {
        return await Context.Set<Local>().Where(x => x.UserId == userId)
            .Include(x => x.LocalPhotos)
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<bool> IsOwnerAsync(int userId, int localId)
    {
        return await Context.Set<Local>().AnyAsync(x => x.UserId == userId && x.Id == localId);
    }
    
    public async Task<int?> GetLocalOwnerIdByLocalId(int localId)
    {
        return await Context.Set<Local>().Where(x => x.Id == localId).Select(x => x.UserId).FirstOrDefaultAsync();
    }

    public async Task<Local?> FindByIdAsync(int id)
    {
        return await Context.Set<Local>()
            .Include(x => x.LocalPhotos)
            .FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<IEnumerable<Local>> FindByLocalCategoryIdAsync(int categoryId)
    {
        return await Context.Set<Local>()
            .Where(x => x.LocalCategoryId == categoryId)
            .Include(x => x.LocalPhotos)
            .ToListAsync();
    }

    public async Task<IEnumerable<Local>> GetAllAsync()
    {
        return await Context.Set<Local>()
            .Include(x => x.LocalPhotos)
            .ToListAsync();
    }
}
