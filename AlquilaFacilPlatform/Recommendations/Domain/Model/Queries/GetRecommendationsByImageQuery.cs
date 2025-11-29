namespace AlquilaFacilPlatform.Recommendations.Domain.Model.Queries;

public record GetRecommendationsByImageQuery(string ImageUrl, int Limit = 10);
