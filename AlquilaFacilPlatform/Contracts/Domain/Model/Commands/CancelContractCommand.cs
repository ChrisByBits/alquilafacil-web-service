namespace AlquilaFacilPlatform.Contracts.Domain.Model.Commands;

public record CancelContractCommand(int ContractInstanceId, int UserId);
