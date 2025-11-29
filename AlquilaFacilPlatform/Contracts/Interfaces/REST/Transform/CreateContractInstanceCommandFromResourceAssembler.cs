using AlquilaFacilPlatform.Contracts.Domain.Model.Commands;
using AlquilaFacilPlatform.Contracts.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.Contracts.Interfaces.REST.Transform;

public static class CreateContractInstanceCommandFromResourceAssembler
{
    public static CreateContractInstanceCommand ToCommandFromResource(CreateContractInstanceResource resource)
    {
        return new CreateContractInstanceCommand(
            resource.ContractTemplateId,
            resource.LocalId,
            resource.LandlordUserId,
            resource.TenantUserId,
            resource.ReservationId,
            resource.StartDate,
            resource.EndDate,
            resource.Placeholders
        );
    }
}
