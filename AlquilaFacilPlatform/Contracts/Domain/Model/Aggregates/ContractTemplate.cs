using AlquilaFacilPlatform.Contracts.Domain.Model.Commands;

namespace AlquilaFacilPlatform.Contracts.Domain.Model.Aggregates;

public class ContractTemplate
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public int UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ContractTemplate()
    {
        CreatedAt = DateTime.UtcNow;
    }

    public ContractTemplate(string title, string content, int userId) : this()
    {
        Title = title;
        Content = content;
        UserId = userId;
    }

    public ContractTemplate(CreateContractTemplateCommand command) : this()
    {
        Title = command.Title;
        Content = command.Content;
        UserId = command.UserId;
    }

    public void Update(UpdateContractTemplateCommand command)
    {
        Title = command.Title;
        Content = command.Content;
        UpdatedAt = DateTime.UtcNow;
    }

    public string GenerateContract(Dictionary<string, string> placeholders)
    {
        var generatedContent = Content;
        foreach (var placeholder in placeholders)
        {
            generatedContent = generatedContent.Replace($"{{{{{placeholder.Key}}}}}", placeholder.Value);
        }
        return generatedContent;
    }
}
