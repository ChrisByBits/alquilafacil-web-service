using System.Net.Mime;
using AlquilaFacilPlatform.Contracts.Application.Internal.Services;
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
public class ContractInstancesController(
    IContractInstanceCommandService contractInstanceCommandService,
    IContractInstanceQueryService contractInstanceQueryService,
    IContractPdfService contractPdfService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateContractInstance([FromBody] CreateContractInstanceResource resource)
    {
        var command = CreateContractInstanceCommandFromResourceAssembler.ToCommandFromResource(resource);
        var instance = await contractInstanceCommandService.Handle(command);

        if (instance == null)
            return BadRequest(new { message = "Cannot create contract instance" });

        var instanceResource = ContractInstanceResourceFromEntityAssembler.ToResourceFromEntity(instance);
        return CreatedAtAction(nameof(GetContractInstanceById), new { instanceId = instance.Id }, instanceResource);
    }

    [HttpGet("{instanceId:int}")]
    public async Task<IActionResult> GetContractInstanceById(int instanceId)
    {
        var query = new GetContractInstanceByIdQuery(instanceId);
        var instance = await contractInstanceQueryService.Handle(query);

        if (instance == null)
            return NotFound();

        var resource = ContractInstanceResourceFromEntityAssembler.ToResourceFromEntity(instance);
        return Ok(resource);
    }

    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetContractInstancesByUserId(int userId)
    {
        var query = new GetContractInstancesByUserIdQuery(userId);
        var instances = await contractInstanceQueryService.Handle(query);

        var resources = instances.Select(ContractInstanceResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpGet("reservation/{reservationId:int}")]
    public async Task<IActionResult> GetContractInstanceByReservationId(int reservationId)
    {
        var query = new GetContractInstancesByReservationIdQuery(reservationId);
        var instance = await contractInstanceQueryService.Handle(query);

        if (instance == null)
            return NotFound();

        var resource = ContractInstanceResourceFromEntityAssembler.ToResourceFromEntity(instance);
        return Ok(resource);
    }

    [HttpPost("{instanceId:int}/sign")]
    public async Task<IActionResult> SignContract(int instanceId, [FromBody] SignContractResource resource)
    {
        var command = new SignContractCommand(instanceId, resource.UserId, resource.Signature);
        var instance = await contractInstanceCommandService.Handle(command);

        if (instance == null)
            return BadRequest(new { message = "Cannot sign contract" });

        var instanceResource = ContractInstanceResourceFromEntityAssembler.ToResourceFromEntity(instance);
        return Ok(instanceResource);
    }

    [HttpPost("{instanceId:int}/cancel")]
    public async Task<IActionResult> CancelContract(int instanceId, [FromQuery] int userId)
    {
        var command = new CancelContractCommand(instanceId, userId);
        var instance = await contractInstanceCommandService.Handle(command);

        if (instance == null)
            return BadRequest(new { message = "Cannot cancel contract" });

        var resource = ContractInstanceResourceFromEntityAssembler.ToResourceFromEntity(instance);
        return Ok(resource);
    }

    [HttpGet("{instanceId:int}/pdf")]
    public async Task<IActionResult> DownloadContractPdf(int instanceId)
    {
        var query = new GetContractInstanceByIdQuery(instanceId);
        var instance = await contractInstanceQueryService.Handle(query);

        if (instance == null)
            return NotFound();

        var pdfBytes = contractPdfService.GeneratePdf(instance);

        return File(pdfBytes, "application/pdf", $"contrato-{instanceId}.pdf");
    }
}
