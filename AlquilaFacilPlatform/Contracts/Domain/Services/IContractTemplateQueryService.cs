using AlquilaFacilPlatform.Contracts.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Contracts.Domain.Model.Queries;

namespace AlquilaFacilPlatform.Contracts.Domain.Services;

public interface IContractTemplateQueryService
{
    Task<ContractTemplate?> Handle(GetContractTemplateByIdQuery query);
    Task<IEnumerable<ContractTemplate>> Handle(GetContractTemplatesByUserIdQuery query);
}
