using AlquilaFacilPlatform.Contracts.Domain.Model.Commands;
using AlquilaFacilPlatform.Contracts.Domain.Model.ValueObjects;

namespace AlquilaFacilPlatform.Contracts.Domain.Model.Aggregates;

public class ContractInstance
{
    public int Id { get; set; }
    public int ContractTemplateId { get; set; }
    public int LocalId { get; set; }
    public int LandlordUserId { get; set; }
    public int TenantUserId { get; set; }
    public int ReservationId { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public EContractStatus Status { get; set; }
    public string GeneratedContent { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? SignedAt { get; set; }
    public string? LandlordSignature { get; set; }
    public string? TenantSignature { get; set; }
    public DateTime? LandlordSignedAt { get; set; }
    public DateTime? TenantSignedAt { get; set; }
    public int Version { get; set; }

    public ContractTemplate? Template { get; set; }

    public ContractInstance()
    {
        CreatedAt = DateTime.UtcNow;
        Status = EContractStatus.Draft;
        Version = 1;
    }

    public ContractInstance(CreateContractInstanceCommand command, string generatedContent) : this()
    {
        ContractTemplateId = command.ContractTemplateId;
        LocalId = command.LocalId;
        LandlordUserId = command.LandlordUserId;
        TenantUserId = command.TenantUserId;
        ReservationId = command.ReservationId;
        StartDate = command.StartDate;
        EndDate = command.EndDate;
        GeneratedContent = generatedContent;
    }

    public void Sign()
    {
        if (Status == EContractStatus.Pending)
        {
            Status = EContractStatus.Signed;
            SignedAt = DateTime.UtcNow;
        }
    }

    public void SendForSignature()
    {
        if (Status == EContractStatus.Draft)
        {
            Status = EContractStatus.Pending;
        }
    }

    public void Cancel()
    {
        if (Status != EContractStatus.Signed)
        {
            Status = EContractStatus.Cancelled;
        }
    }

    public void SignByLandlord(string signature)
    {
        LandlordSignature = signature;
        LandlordSignedAt = DateTime.UtcNow;

        if (TenantSignature != null)
        {
            Status = EContractStatus.Signed;
            SignedAt = DateTime.UtcNow;
        }
        else
        {
            Status = EContractStatus.Pending;
        }
    }

    public void SignByTenant(string signature)
    {
        TenantSignature = signature;
        TenantSignedAt = DateTime.UtcNow;

        if (LandlordSignature != null)
        {
            Status = EContractStatus.Signed;
            SignedAt = DateTime.UtcNow;
        }
        else
        {
            Status = EContractStatus.Pending;
        }
    }
}
