namespace AlquilaFacilPlatform.Contracts.Interfaces.REST.Resources;

public record ContractTemplateResource(
    int Id,
    string Title,
    string Content,
    int UserId,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
