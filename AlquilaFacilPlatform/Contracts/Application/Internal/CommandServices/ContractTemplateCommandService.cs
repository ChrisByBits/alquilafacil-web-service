using AlquilaFacilPlatform.Contracts.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Contracts.Domain.Model.Commands;
using AlquilaFacilPlatform.Contracts.Domain.Repositories;
using AlquilaFacilPlatform.Contracts.Domain.Services;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.Contracts.Application.Internal.CommandServices;

public class ContractTemplateCommandService(
    IContractTemplateRepository contractTemplateRepository,
    IUnitOfWork unitOfWork) : IContractTemplateCommandService
{
    public async Task<ContractTemplate?> Handle(CreateContractTemplateCommand command)
    {
        var template = new ContractTemplate(command);
        await contractTemplateRepository.AddAsync(template);
        await unitOfWork.CompleteAsync();
        return template;
    }

    public async Task<ContractTemplate?> Handle(UpdateContractTemplateCommand command)
    {
        var template = await contractTemplateRepository.FindByIdAsync(command.Id);
        if (template == null)
            return null;

        template.Update(command);
        await unitOfWork.CompleteAsync();
        return template;
    }

    public async Task<bool> Handle(DeleteContractTemplateCommand command)
    {
        var template = await contractTemplateRepository.FindByIdAsync(command.Id);
        if (template == null)
            return false;

        contractTemplateRepository.Remove(template);
        await unitOfWork.CompleteAsync();
        return true;
    }
}
