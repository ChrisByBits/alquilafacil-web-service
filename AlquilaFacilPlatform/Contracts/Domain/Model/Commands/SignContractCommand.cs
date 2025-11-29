namespace AlquilaFacilPlatform.Contracts.Domain.Model.Commands;

public record SignContractCommand(int ContractInstanceId, int UserId, string Signature);
