namespace AlquilaFacilPlatform.Contracts.Interfaces.REST.Resources;

public record ContractInstanceResource(
    int Id,
    int ContractTemplateId,
    string? TemplateName,
    int LocalId,
    int LandlordUserId,
    int TenantUserId,
    int ReservationId,
    DateTime StartDate,
    DateTime EndDate,
    string Status,
    string GeneratedContent,
    DateTime CreatedAt,
    DateTime? SignedAt
);
