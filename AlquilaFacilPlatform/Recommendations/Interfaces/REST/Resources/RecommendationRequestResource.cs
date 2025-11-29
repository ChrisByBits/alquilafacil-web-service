namespace AlquilaFacilPlatform.Recommendations.Interfaces.REST.Resources;

public record RecommendationRequestResource(string? ImageUrl = null, int Limit = 10);
