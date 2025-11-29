using AlquilaFacilPlatform.Contracts.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Contracts.Domain.Model.Commands;

namespace AlquilaFacilPlatform.Contracts.Domain.Services;

public interface IContractInstanceCommandService
{
    Task<ContractInstance?> Handle(CreateContractInstanceCommand command);
    Task<ContractInstance?> Handle(SignContractCommand command);
    Task<ContractInstance?> Handle(CancelContractCommand command);
}
