using AlquilaFacilPlatform.Contracts.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Contracts.Domain.Model.Queries;
using AlquilaFacilPlatform.Contracts.Domain.Repositories;
using AlquilaFacilPlatform.Contracts.Domain.Services;

namespace AlquilaFacilPlatform.Contracts.Application.Internal.QueryServices;

public class ContractInstanceQueryService(IContractInstanceRepository contractInstanceRepository)
    : IContractInstanceQueryService
{
    public async Task<ContractInstance?> Handle(GetContractInstanceByIdQuery query)
    {
        return await contractInstanceRepository.FindByIdWithTemplateAsync(query.Id);
    }

    public async Task<IEnumerable<ContractInstance>> Handle(GetContractInstancesByUserIdQuery query)
    {
        return await contractInstanceRepository.FindByUserIdAsync(query.UserId);
    }

    public async Task<ContractInstance?> Handle(GetContractInstancesByReservationIdQuery query)
    {
        return await contractInstanceRepository.FindByReservationIdAsync(query.ReservationId);
    }
}
