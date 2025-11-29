using System.Net.Mime;
using AlquilaFacilPlatform.Recommendations.Domain.Model.Queries;
using AlquilaFacilPlatform.Recommendations.Domain.Services;
using AlquilaFacilPlatform.Recommendations.Interfaces.REST.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlquilaFacilPlatform.Recommendations.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
public class RecommendationsController(IRecommendationQueryService recommendationQueryService) : ControllerBase
{
    /// <summary>
    /// Gets personalized recommendations for a user based on their preferences and history.
    /// Uses CNN-based analysis to suggest relevant spaces.
    /// </summary>
    [HttpGet("user/{userId:int}")]
    public async Task<IActionResult> GetRecommendationsForUser(int userId, [FromQuery] int limit = 10)
    {
        var query = new GetRecommendationsByUserIdQuery(userId, limit);
        var recommendedIds = await recommendationQueryService.Handle(query);

        return Ok(new RecommendationResponseResource(recommendedIds));
    }

    /// <summary>
    /// Gets similar spaces based on a reference local.
    /// Uses CNN image similarity to find visually similar spaces.
    /// </summary>
    [HttpGet("local/{localId:int}/similar")]
    public async Task<IActionResult> GetSimilarLocals(int localId, [FromQuery] int limit = 10)
    {
        var query = new GetRecommendationsByLocalIdQuery(localId, limit);
        var recommendedIds = await recommendationQueryService.Handle(query);

        return Ok(new RecommendationResponseResource(recommendedIds));
    }

    /// <summary>
    /// Gets recommendations based on an uploaded image.
    /// Uses CNN to extract features and find matching spaces.
    /// </summary>
    [HttpPost("by-image")]
    public async Task<IActionResult> GetRecommendationsByImage([FromBody] RecommendationRequestResource resource)
    {
        if (string.IsNullOrEmpty(resource.ImageUrl))
            return BadRequest(new { message = "ImageUrl is required" });

        var query = new GetRecommendationsByImageQuery(resource.ImageUrl, resource.Limit);
        var recommendedIds = await recommendationQueryService.Handle(query);

        return Ok(new RecommendationResponseResource(recommendedIds));
    }
}
