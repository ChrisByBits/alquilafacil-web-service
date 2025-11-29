using AlquilaFacilPlatform.Contracts.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Contracts.Domain.Model.Commands;

namespace AlquilaFacilPlatform.Contracts.Domain.Services;

public interface IContractTemplateCommandService
{
    Task<ContractTemplate?> Handle(CreateContractTemplateCommand command);
    Task<ContractTemplate?> Handle(UpdateContractTemplateCommand command);
    Task<bool> Handle(DeleteContractTemplateCommand command);
}
