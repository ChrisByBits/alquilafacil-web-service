using AlquilaFacilPlatform.Contracts.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Contracts.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.Contracts.Interfaces.REST.Transform;

public static class ContractInstanceResourceFromEntityAssembler
{
    public static ContractInstanceResource ToResourceFromEntity(ContractInstance instance)
    {
        return new ContractInstanceResource(
            instance.Id,
            instance.ContractTemplateId,
            instance.Template?.Title,
            instance.LocalId,
            instance.LandlordUserId,
            instance.TenantUserId,
            instance.ReservationId,
            instance.StartDate,
            instance.EndDate,
            instance.Status.ToString(),
            instance.GeneratedContent,
            instance.CreatedAt,
            instance.SignedAt
        );
    }
}
