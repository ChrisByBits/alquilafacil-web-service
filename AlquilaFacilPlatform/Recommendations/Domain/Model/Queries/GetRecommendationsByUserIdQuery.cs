namespace AlquilaFacilPlatform.Recommendations.Domain.Model.Queries;

public record GetRecommendationsByUserIdQuery(int UserId, int Limit = 10);
