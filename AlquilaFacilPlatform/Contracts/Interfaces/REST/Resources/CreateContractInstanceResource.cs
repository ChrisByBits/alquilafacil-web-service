namespace AlquilaFacilPlatform.Contracts.Interfaces.REST.Resources;

public record CreateContractInstanceResource(
    int ContractTemplateId,
    int LocalId,
    int LandlordUserId,
    int TenantUserId,
    int ReservationId,
    DateTime StartDate,
    DateTime EndDate,
    Dictionary<string, string> Placeholders
);
