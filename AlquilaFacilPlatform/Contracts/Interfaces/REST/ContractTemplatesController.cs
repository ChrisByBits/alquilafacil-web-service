using System.Net.Mime;
using AlquilaFacilPlatform.Contracts.Domain.Model.Commands;
using AlquilaFacilPlatform.Contracts.Domain.Model.Queries;
using AlquilaFacilPlatform.Contracts.Domain.Services;
using AlquilaFacilPlatform.Contracts.Interfaces.REST.Resources;
using AlquilaFacilPlatform.Contracts.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlquilaFacilPlatform.Contracts.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
public class ContractTemplatesController(
    IContractTemplateCommandService contractTemplateCommandService,
    IContractTemplateQueryService contractTemplateQueryService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateContractTemplate([FromBody] CreateContractTemplateResource resource)
    {
        var command = CreateContractTemplateCommandFromResourceAssembler.ToCommandFromResource(resource);
        var template = await contractTemplateCommandService.Handle(command);

        if (template == null)
            return BadRequest(new { message = "Cannot create contract template" });

        var templateResource = ContractTemplateResourceFromEntityAssembler.ToResourceFromEntity(template);
        return CreatedAtAction(nameof(GetContractTemplateById), new { templateId = template.Id }, templateResource);
    }

    [HttpGet("{templateId:int}")]
    public async Task<IActionResult> GetContractTemplateById(int templateId)
    {
        var query = new GetContractTemplateByIdQuery(templateId);
        var template = await contractTemplateQueryService.Handle(query);

        if (template == null)
            return NotFound();

        var resource = ContractTemplateResourceFromEntityAssembler.ToResourceFromEntity(template);
        return Ok(resource);
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetContractTemplatesByUserId(int userId)
    {
        var query = new GetContractTemplatesByUserIdQuery(userId);
        var templates = await contractTemplateQueryService.Handle(query);

        var resources = templates.Select(ContractTemplateResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpPut("{templateId:int}")]
    public async Task<IActionResult> UpdateContractTemplate(int templateId, [FromBody] UpdateContractTemplateResource resource)
    {
        var command = new UpdateContractTemplateCommand(templateId, resource.Title, resource.Content);
        var template = await contractTemplateCommandService.Handle(command);

        if (template == null)
            return NotFound();

        var templateResource = ContractTemplateResourceFromEntityAssembler.ToResourceFromEntity(template);
        return Ok(templateResource);
    }

    [HttpDelete("{templateId:int}")]
    public async Task<IActionResult> DeleteContractTemplate(int templateId)
    {
        var command = new DeleteContractTemplateCommand(templateId);
        var result = await contractTemplateCommandService.Handle(command);

        if (!result)
            return NotFound();

        return NoContent();
    }
}
