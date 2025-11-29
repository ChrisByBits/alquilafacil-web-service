using AlquilaFacilPlatform.Contracts.Domain.Model.Commands;
using AlquilaFacilPlatform.Contracts.Interfaces.REST.Resources;

namespace AlquilaFacilPlatform.Contracts.Interfaces.REST.Transform;

public static class CreateContractTemplateCommandFromResourceAssembler
{
    public static CreateContractTemplateCommand ToCommandFromResource(CreateContractTemplateResource resource)
    {
        return new CreateContractTemplateCommand(resource.Title, resource.Content, resource.UserId);
    }
}
