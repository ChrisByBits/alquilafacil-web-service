using System.Net.Mime;
using AlquilaFacilPlatform.ImageManagement.Application.Internal.OutboundServices;
using AlquilaFacilPlatform.ImageManagement.Domain.Model.Commands;
using AlquilaFacilPlatform.ImageManagement.Domain.Services;
using AlquilaFacilPlatform.Profiles.Domain.Model.Commands;
using AlquilaFacilPlatform.Profiles.Domain.Model.Queries;
using AlquilaFacilPlatform.Profiles.Domain.Services;
using AlquilaFacilPlatform.Profiles.Interfaces.REST.Resources;
using AlquilaFacilPlatform.Profiles.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Mvc;

namespace AlquilaFacilPlatform.Profiles.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
public class ProfilesController(
    IProfileCommandService profileCommandService,
    IProfileQueryService profileQueryService,
    IImageCommandService imageCommandService,
    IImageStorageService imageStorageService)
    : ControllerBase
{
    [HttpGet("user/{userId}")]
    public async Task<IActionResult> GetProfileByUserId(int userId)
    {
        var profile = await profileQueryService.Handle(new GetProfileByUserIdQuery(userId));
        if (profile == null) return NotFound();
        var profileResource = ProfileResourceFromEntityAssembler.ToResourceFromEntity(profile);
        return Ok(profileResource);
    }
    
    [HttpGet("subscription-status/{userId}")]
    public async Task<IActionResult> GetSubscriptionStatusByUserId(int userId)
    {
        var query = new GetSubscriptionStatusByUserIdQuery(userId);
        var subscriptionStatus = await profileQueryService.Handle(query);
        return Ok(subscriptionStatus);
    }

    [HttpPut("{userId}")]
    public async Task<ActionResult> UpdateProfile(int userId, [FromBody] UpdateProfileResource updateProfileResource)
    {
        var updateProfileCommand = UpdateProfileCommandFromResourceAssembler.ToCommandFromResource(userId, updateProfileResource);
        var result = await profileCommandService.Handle(updateProfileCommand);
        return Ok(ProfileResourceFromEntityAssembler.ToResourceFromEntity(result));
    }
    
    [HttpGet("bank-accounts/{userId}")]
    public async Task<IActionResult> GetProfileBankAccountsByUserId(int userId)
    {
        var query = new GetProfileBankAccountsByUserIdQuery(userId);
        var bankAccounts = await profileQueryService.Handle(query);
        return Ok(bankAccounts);
    }

    [HttpPut("{profileId}/avatar")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UploadProfileAvatar(int profileId, [FromForm] UploadAvatarRequest request)
    {
        try
        {
            // Validate image
            if (!imageStorageService.ValidateImage(request.File, out var errorMessage))
                return BadRequest(new { message = errorMessage });

            // Upload to storage
            var (url, storagePath) = await imageStorageService.UploadImageAsync(request.File, "profiles");

            // Get dimensions
            var (width, height) = await imageStorageService.GetImageDimensionsAsync(request.File);

            // Save metadata to database
            var uploadImageCommand = new UploadImageCommand(
                url,
                request.File.FileName,
                request.File.ContentType,
                request.File.Length,
                storagePath,
                "Profile",
                profileId,
                request.UserId,
                width,
                height
            );

            await imageCommandService.Handle(uploadImageCommand);

            // Update profile avatar URL
            var updateAvatarCommand = new UpdateProfileAvatarCommand(profileId, url);
            var profile = await profileCommandService.Handle(updateAvatarCommand);

            var profileResource = ProfileResourceFromEntityAssembler.ToResourceFromEntity(profile);
            return Ok(profileResource);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = ex.Message });
        }
    }
}