using AlquilaFacilPlatform.Recommendations.Application.Internal.OutboundServices;
using AlquilaFacilPlatform.Recommendations.Domain.Model.Queries;
using AlquilaFacilPlatform.Recommendations.Domain.Services;

namespace AlquilaFacilPlatform.Recommendations.Application.Internal.QueryServices;

public class RecommendationQueryService(ICnnRecommendationService cnnRecommendationService)
    : IRecommendationQueryService
{
    public async Task<IEnumerable<int>> Handle(GetRecommendationsByUserIdQuery query)
    {
        return await cnnRecommendationService.GetRecommendationsForUserAsync(query.UserId, query.Limit);
    }

    public async Task<IEnumerable<int>> Handle(GetRecommendationsByLocalIdQuery query)
    {
        return await cnnRecommendationService.GetSimilarLocalsAsync(query.LocalId, query.Limit);
    }

    public async Task<IEnumerable<int>> Handle(GetRecommendationsByImageQuery query)
    {
        return await cnnRecommendationService.GetRecommendationsByImageAsync(query.ImageUrl, query.Limit);
    }
}
