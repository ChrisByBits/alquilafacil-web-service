using AlquilaFacilPlatform.Contracts.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.Contracts.Domain.Repositories;

public interface IContractTemplateRepository : IBaseRepository<ContractTemplate>
{
    Task<IEnumerable<ContractTemplate>> FindByUserIdAsync(int userId);
}
