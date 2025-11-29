namespace AlquilaFacilPlatform.Contracts.Domain.Model.Commands;

public record CreateContractTemplateCommand(string Title, string Content, int UserId);
