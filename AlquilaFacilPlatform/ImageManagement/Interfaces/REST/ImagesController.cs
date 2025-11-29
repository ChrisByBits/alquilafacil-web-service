using AlquilaFacilPlatform.ImageManagement.Application.Internal.OutboundServices;
using AlquilaFacilPlatform.ImageManagement.Domain.Model.Commands;
using AlquilaFacilPlatform.ImageManagement.Domain.Model.Queries;
using AlquilaFacilPlatform.ImageManagement.Domain.Services;
using AlquilaFacilPlatform.ImageManagement.Interfaces.REST.Resources;
using AlquilaFacilPlatform.ImageManagement.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mime;

namespace AlquilaFacilPlatform.ImageManagement.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class ImagesController(
    IImageCommandService imageCommandService,
    IImageQueryService imageQueryService,
    IImageStorageService imageStorageService) : ControllerBase
{
    [HttpPost("upload")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadImage([FromForm] UploadImageRequest request)
    {
        try
        {
            // Validate image
            if (!imageStorageService.ValidateImage(request.File, out var errorMessage))
                return BadRequest(new { message = errorMessage });

            // Upload to storage
            var (url, storagePath) = await imageStorageService.UploadImageAsync(
                request.File,
                request.Folder ?? request.EntityType.ToLower()
            );

            // Get dimensions
            var (width, height) = await imageStorageService.GetImageDimensionsAsync(request.File);

            // Save metadata to database
            var command = new UploadImageCommand(
                url,
                request.File.FileName,
                request.File.ContentType,
                request.File.Length,
                storagePath,
                request.EntityType,
                request.EntityId,
                request.UploadedBy,
                width,
                height
            );

            var image = await imageCommandService.Handle(command);
            if (image == null)
                return BadRequest(new { message = "Failed to save image metadata" });

            var resource = ImageResourceFromEntityAssembler.ToResourceFromEntity(image);
            return CreatedAtAction(nameof(GetImageById), new { imageId = image.Id }, resource);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }

    [HttpGet("{imageId:int}")]
    public async Task<IActionResult> GetImageById(int imageId)
    {
        var query = new GetImageByIdQuery(imageId);
        var image = await imageQueryService.Handle(query);

        if (image == null)
            return NotFound(new { message = "Image not found" });

        var resource = ImageResourceFromEntityAssembler.ToResourceFromEntity(image);
        return Ok(resource);
    }

    [HttpGet("entity/{entityType}/{entityId:int}")]
    public async Task<IActionResult> GetImagesByEntity(string entityType, int entityId)
    {
        var query = new GetImagesByEntityQuery(entityType, entityId);
        var images = await imageQueryService.Handle(query);

        var resources = images.Select(ImageResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpDelete("{imageId:int}")]
    public async Task<IActionResult> DeleteImage(int imageId)
    {
        var command = new DeleteImageCommand(imageId);
        var result = await imageCommandService.Handle(command);

        if (!result)
            return NotFound(new { message = "Image not found or already deleted" });

        return Ok(new { message = "Image deleted successfully" });
    }
}
