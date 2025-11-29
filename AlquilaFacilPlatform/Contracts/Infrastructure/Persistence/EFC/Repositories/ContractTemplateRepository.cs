using AlquilaFacilPlatform.Contracts.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Contracts.Domain.Repositories;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Configuration;
using AlquilaFacilPlatform.Shared.Infrastructure.Persistence.EFC.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AlquilaFacilPlatform.Contracts.Infrastructure.Persistence.EFC.Repositories;

public class ContractTemplateRepository(AppDbContext context)
    : BaseRepository<ContractTemplate>(context), IContractTemplateRepository
{
    public async Task<IEnumerable<ContractTemplate>> FindByUserIdAsync(int userId)
    {
        return await Context.Set<ContractTemplate>()
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}
