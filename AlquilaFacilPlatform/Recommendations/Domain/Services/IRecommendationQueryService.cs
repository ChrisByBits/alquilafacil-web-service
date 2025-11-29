using AlquilaFacilPlatform.Recommendations.Domain.Model.Queries;

namespace AlquilaFacilPlatform.Recommendations.Domain.Services;

public interface IRecommendationQueryService
{
    Task<IEnumerable<int>> Handle(GetRecommendationsByUserIdQuery query);
    Task<IEnumerable<int>> Handle(GetRecommendationsByLocalIdQuery query);
    Task<IEnumerable<int>> Handle(GetRecommendationsByImageQuery query);
}
