namespace AlquilaFacilPlatform.Recommendations.Domain.Model.Queries;

public record GetRecommendationsByLocalIdQuery(int LocalId, int Limit = 10);
