using AlquilaFacilPlatform.Locals.Domain.Repositories;

namespace AlquilaFacilPlatform.Recommendations.Application.Internal.OutboundServices;

/// <summary>
/// Mock implementation of CNN recommendation service.
/// Returns random local IDs for demonstration purposes.
/// Replace this with actual CNN service integration when ready.
/// </summary>
public class MockCnnRecommendationService(ILocalRepository localRepository) : ICnnRecommendationService
{
    public async Task<IEnumerable<int>> GetRecommendationsForUserAsync(int userId, int limit)
    {
        // Mock implementation: return random locals
        // In production, this would call the CNN microservice
        var allLocals = await localRepository.ListAsync();
        var localIds = allLocals
            .OrderBy(_ => Guid.NewGuid())
            .Take(limit)
            .Select(l => l.Id)
            .ToList();

        return localIds;
    }

    public async Task<IEnumerable<int>> GetSimilarLocalsAsync(int localId, int limit)
    {
        // Mock implementation: return locals from same category
        // In production, this would use CNN image similarity
        var local = await localRepository.FindByIdAsync(localId);
        if (local == null)
            return Enumerable.Empty<int>();

        var allLocals = await localRepository.ListAsync();
        var similarLocals = allLocals
            .Where(l => l.Id != localId && l.LocalCategoryId == local.LocalCategoryId)
            .OrderBy(_ => Guid.NewGuid())
            .Take(limit)
            .Select(l => l.Id)
            .ToList();

        // If not enough from same category, add others
        if (similarLocals.Count < limit)
        {
            var additional = allLocals
                .Where(l => l.Id != localId && !similarLocals.Contains(l.Id))
                .OrderBy(_ => Guid.NewGuid())
                .Take(limit - similarLocals.Count)
                .Select(l => l.Id);

            similarLocals.AddRange(additional);
        }

        return similarLocals;
    }

    public async Task<IEnumerable<int>> GetRecommendationsByImageAsync(string imageUrl, int limit)
    {
        // Mock implementation: return random locals
        // In production, this would analyze the image and find similar spaces
        var allLocals = await localRepository.ListAsync();
        var localIds = allLocals
            .OrderBy(_ => Guid.NewGuid())
            .Take(limit)
            .Select(l => l.Id)
            .ToList();

        return localIds;
    }
}
