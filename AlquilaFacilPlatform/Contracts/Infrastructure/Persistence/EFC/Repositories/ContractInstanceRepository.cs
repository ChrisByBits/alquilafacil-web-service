using AlquilaFacilPlatform.Contracts.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Contracts.Domain.Repositories;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlquilaFacilPlatform.Contracts.Infrastructure.Persistence.EFC.Repositories;

public class ContractInstanceRepository(AppDbContext context)
    : BaseRepository<ContractInstance>(context), IContractInstanceRepository
{
    public async Task<ContractInstance?> FindByIdWithTemplateAsync(int id)
    {
        return await Context.Set<ContractInstance>()
            .Include(c => c.Template)
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<ContractInstance>> FindByUserIdAsync(int userId)
    {
        return await Context.Set<ContractInstance>()
            .Include(c => c.Template)
            .Where(c => c.LandlordUserId == userId || c.TenantUserId == userId)
            .OrderByDescending(c => c.CreatedAt)
            .ToListAsync();
    }

    public async Task<ContractInstance?> FindByReservationIdAsync(int reservationId)
    {
        return await Context.Set<ContractInstance>()
            .Include(c => c.Template)
            .FirstOrDefaultAsync(c => c.ReservationId == reservationId);
    }
}
