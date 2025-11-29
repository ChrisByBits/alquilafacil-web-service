using AlquilaFacilPlatform.Contracts.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Contracts.Domain.Model.Queries;

namespace AlquilaFacilPlatform.Contracts.Domain.Services;

public interface IContractInstanceQueryService
{
    Task<ContractInstance?> Handle(GetContractInstanceByIdQuery query);
    Task<IEnumerable<ContractInstance>> Handle(GetContractInstancesByUserIdQuery query);
    Task<ContractInstance?> Handle(GetContractInstancesByReservationIdQuery query);
}
