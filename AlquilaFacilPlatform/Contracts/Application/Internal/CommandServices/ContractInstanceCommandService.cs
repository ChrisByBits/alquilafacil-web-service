using AlquilaFacilPlatform.Contracts.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Contracts.Domain.Model.Commands;
using AlquilaFacilPlatform.Contracts.Domain.Repositories;
using AlquilaFacilPlatform.Contracts.Domain.Services;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.Contracts.Application.Internal.CommandServices;

public class ContractInstanceCommandService(
    IContractInstanceRepository contractInstanceRepository,
    IContractTemplateRepository contractTemplateRepository,
    IUnitOfWork unitOfWork) : IContractInstanceCommandService
{
    public async Task<ContractInstance?> Handle(CreateContractInstanceCommand command)
    {
        var template = await contractTemplateRepository.FindByIdAsync(command.ContractTemplateId);
        if (template == null)
            return null;

        var generatedContent = template.GenerateContract(command.Placeholders);
        var instance = new ContractInstance(command, generatedContent);

        await contractInstanceRepository.AddAsync(instance);
        await unitOfWork.CompleteAsync();

        return instance;
    }

    public async Task<ContractInstance?> Handle(SignContractCommand command)
    {
        var instance = await contractInstanceRepository.FindByIdAsync(command.ContractInstanceId);
        if (instance == null)
            return null;

        // Determine if user is landlord or tenant and sign accordingly
        if (instance.LandlordUserId == command.UserId)
        {
            instance.SignByLandlord(command.Signature);
        }
        else if (instance.TenantUserId == command.UserId)
        {
            instance.SignByTenant(command.Signature);
        }
        else
        {
            return null; // User is not a party to this contract
        }

        await unitOfWork.CompleteAsync();

        return instance;
    }

    public async Task<ContractInstance?> Handle(CancelContractCommand command)
    {
        var instance = await contractInstanceRepository.FindByIdAsync(command.ContractInstanceId);
        if (instance == null)
            return null;

        // Verify the user is a party to the contract
        if (instance.LandlordUserId != command.UserId && instance.TenantUserId != command.UserId)
            return null;

        instance.Cancel();
        await unitOfWork.CompleteAsync();

        return instance;
    }
}
