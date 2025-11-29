using AlquilaFacilPlatform.Contracts.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Contracts.Domain.Model.Queries;
using AlquilaFacilPlatform.Contracts.Domain.Repositories;
using AlquilaFacilPlatform.Contracts.Domain.Services;

namespace AlquilaFacilPlatform.Contracts.Application.Internal.QueryServices;

public class ContractTemplateQueryService(IContractTemplateRepository contractTemplateRepository)
    : IContractTemplateQueryService
{
    public async Task<ContractTemplate?> Handle(GetContractTemplateByIdQuery query)
    {
        return await contractTemplateRepository.FindByIdAsync(query.Id);
    }

    public async Task<IEnumerable<ContractTemplate>> Handle(GetContractTemplatesByUserIdQuery query)
    {
        return await contractTemplateRepository.FindByUserIdAsync(query.UserId);
    }
}
