namespace AlquilaFacilPlatform.Recommendations.Application.Internal.OutboundServices;

/// <summary>
/// Interface for CNN-based recommendation service.
/// This interface is designed to be implemented by an external CNN service
/// for intelligent space recommendations based on image similarity and user preferences.
/// </summary>
public interface ICnnRecommendationService
{
    /// <summary>
    /// Gets recommended local IDs based on user's historical preferences and behavior.
    /// Uses CNN to analyze patterns in previously viewed/booked spaces.
    /// </summary>
    /// <param name="userId">The user ID to get recommendations for</param>
    /// <param name="limit">Maximum number of recommendations to return</param>
    /// <returns>List of local IDs sorted by relevance score</returns>
    Task<IEnumerable<int>> GetRecommendationsForUserAsync(int userId, int limit);

    /// <summary>
    /// Gets similar locals based on image feature extraction using CNN.
    /// Compares visual features (style, layout, amenities) to find similar spaces.
    /// </summary>
    /// <param name="localId">The local ID to find similar spaces for</param>
    /// <param name="limit">Maximum number of recommendations to return</param>
    /// <returns>List of similar local IDs sorted by similarity score</returns>
    Task<IEnumerable<int>> GetSimilarLocalsAsync(int localId, int limit);

    /// <summary>
    /// Gets recommendations based on an uploaded image.
    /// Uses CNN to extract features from the image and find matching spaces.
    /// </summary>
    /// <param name="imageUrl">URL of the reference image</param>
    /// <param name="limit">Maximum number of recommendations to return</param>
    /// <returns>List of local IDs that match the image features</returns>
    Task<IEnumerable<int>> GetRecommendationsByImageAsync(string imageUrl, int limit);
}
