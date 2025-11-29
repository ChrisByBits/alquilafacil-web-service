using AlquilaFacilPlatform.Contracts.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Contracts.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.Contracts.Interfaces.REST.Transform;

public static class ContractTemplateResourceFromEntityAssembler
{
    public static ContractTemplateResource ToResourceFromEntity(ContractTemplate template)
    {
        return new ContractTemplateResource(
            template.Id,
            template.Title,
            template.Content,
            template.UserId,
            template.CreatedAt,
            template.UpdatedAt
        );
    }
}
