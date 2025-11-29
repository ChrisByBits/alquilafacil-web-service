using System.Net.Mime;
using AlquilaFacilPlatform.Management.Domain.Model.Queries;
using AlquilaFacilPlatform.Management.Domain.Services;
using AlquilaFacilPlatform.Management.Interfaces.REST.Resources;
using AlquilaFacilPlatform.Management.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlquilaFacilPlatform.Management.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class LocalEdgeNodesController(ILocalEdgeNodeQueryService localEdgeNodeQueryService, 
                                    ILocalEdgeNodeCommandService localEdgeNodeCommandService): ControllerBase
{
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<IActionResult> CreateLocalEdgeNode([FromBody] CreateLocalEdgeNodeResource resource)
    {
        var command = CreateLocalEdgeNodeCommandFromResourceAssembler.ToCommandFromResource(resource);
        var localEdgeNode = await localEdgeNodeCommandService.Handle(command);
        if (localEdgeNode is null) return BadRequest();
        var localEdgeNodeResource = LocalEdgeNodeResourceFromEntityAssembler.ToResourceFromEntity(localEdgeNode);
        return StatusCode(201, localEdgeNodeResource);
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPut("{localId:int}")]
    public async Task<IActionResult> UpdateLocalEdgeNode(int localId, [FromBody] UpdateLocalEdgeNodeResource resource)
    {
        var command = UpdateLocalEdgeNodeCommandFromResourceAssembler.ToCommandFromResource(localId, resource);
        var localEdgeNode = await localEdgeNodeCommandService.Handle(command);
        if (localEdgeNode is null) return BadRequest();
        var localEdgeNodeResource = LocalEdgeNodeResourceFromEntityAssembler.ToResourceFromEntity(localEdgeNode);
        return Ok(localEdgeNodeResource);
    }
    
    [Authorize]
    [HttpGet("local-id/{localId}")]
    public async Task<IActionResult> GetLocalEdgeNodeByLocalId(int localId)
    {
        var query = new GetLocalEdgeNodeByLocalIdQuery(localId);
        var localEdgeNode = await localEdgeNodeQueryService.Handle(query);
        if (localEdgeNode is null) return NotFound();
        var localEdgeNodeResource = LocalEdgeNodeResourceFromEntityAssembler.ToResourceFromEntity(localEdgeNode);
        return Ok(localEdgeNodeResource);
    }
}