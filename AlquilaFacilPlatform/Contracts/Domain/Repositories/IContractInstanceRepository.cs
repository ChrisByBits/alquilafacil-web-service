using AlquilaFacilPlatform.Contracts.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.Contracts.Domain.Repositories;

public interface IContractInstanceRepository : IBaseRepository<ContractInstance>
{
    Task<ContractInstance?> FindByIdWithTemplateAsync(int id);
    Task<IEnumerable<ContractInstance>> FindByUserIdAsync(int userId);
    Task<ContractInstance?> FindByReservationIdAsync(int reservationId);
}
