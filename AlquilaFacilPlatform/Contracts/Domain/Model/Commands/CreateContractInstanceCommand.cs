namespace AlquilaFacilPlatform.Contracts.Domain.Model.Commands;

public record CreateContractInstanceCommand(
    int ContractTemplateId,
    int LocalId,
    int LandlordUserId,
    int TenantUserId,
    int ReservationId,
    DateTime StartDate,
    DateTime EndDate,
    Dictionary<string, string> Placeholders
);
