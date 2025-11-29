using AlquilaFacilPlatform.ImageManagement.Domain.Model.Aggregates;
using AlquilaFacilPlatform.ImageManagement.Domain.Repositories;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlquilaFacilPlatform.ImageManagement.Infrastructure.Persistence.EFC.Repositories;

public class ImageRepository(AppDbContext context) : BaseRepository<Image>(context), IImageRepository
{
    public async Task<IEnumerable<Image>> FindByEntityTypeAndIdAsync(string entityType, int entityId)
    {
        return await Context.Set<Image>()
            .Where(i => i.EntityType == entityType && i.EntityId == entityId && !i.IsDeleted)
            .ToListAsync();
    }

    public async Task<Image?> FindByIdAsync(int id)
    {
        return await Context.Set<Image>()
            .Where(i => i.Id == id && !i.IsDeleted)
            .FirstOrDefaultAsync();
    }
}
