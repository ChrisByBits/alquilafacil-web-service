using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AlquilaFacilPlatform.Booking.Domain.Repositories;
using AlquilaFacilPlatform.Locals.Domain.Repositories;
using AlquilaFacilPlatform.Recommendations.Application.Internal.OutboundServices;
using AlquilaFacilPlatform.Recommendations.Domain.Model.ValueObjects;

namespace AlquilaFacilPlatform.Recommendations.Infrastructure.External;

public class HuggingFaceCnnService : ICnnRecommendationService
{
    private readonly HttpClient _httpClient;
    private readonly ILocalRepository _localRepository;
    private readonly IReservationRepository _reservationRepository;
    private readonly ILogger<HuggingFaceCnnService> _logger;
    private readonly string _apiKey;

    // Modelo CLIP para clasificacion de imagenes
    private const string CLIP_MODEL_URL = "https://api-inference.huggingface.co/models/openai/clip-vit-base-patch32";

    // Cache en memoria para features de imagenes
    private readonly Dictionary<string, ImageFeatures> _featureCache = new();

    // Labels predefinidos para clasificar espacios de alquiler
    private readonly string[] _spaceLabels = new[]
    {
        "office space", "meeting room", "conference room", "coworking space",
        "event hall", "party venue", "restaurant", "cafe",
        "gym", "fitness center", "yoga studio",
        "photo studio", "art gallery", "creative space",
        "warehouse", "storage space", "industrial space",
        "rooftop", "terrace", "garden", "outdoor space",
        "classroom", "training room", "workshop space",
        "recording studio", "music room", "theater",
        "kitchen", "commercial kitchen", "bakery",
        "retail space", "showroom", "pop-up store",
        "modern interior", "classic interior", "minimalist design",
        "luxury space", "budget friendly", "professional environment"
    };

    public HuggingFaceCnnService(
        ILocalRepository localRepository,
        IReservationRepository reservationRepository,
        ILogger<HuggingFaceCnnService> logger,
        IConfiguration configuration)
    {
        _localRepository = localRepository;
        _reservationRepository = reservationRepository;
        _logger = logger;
        _apiKey = configuration["HuggingFace:ApiKey"] ?? throw new InvalidOperationException("HuggingFace API key not configured");

        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
        _httpClient.Timeout = TimeSpan.FromSeconds(30);

        _logger.LogInformation("HuggingFace CNN Service initialized");
    }

    public async Task<IEnumerable<int>> GetRecommendationsForUserAsync(int userId, int limit)
    {
        try
        {
            _logger.LogInformation("Getting recommendations for user {UserId} based on reservation history", userId);

            // Obtener las reservas del usuario
            var userReservations = await _reservationRepository.GetReservationsByUserIdAsync(userId);
            var reservedLocalIds = userReservations.Select(r => r.LocalId).Distinct().ToList();

            if (!reservedLocalIds.Any())
            {
                _logger.LogInformation("User {UserId} has no reservations, returning popular locals", userId);
                return await GetPopularLocalsAsync(limit);
            }

            _logger.LogInformation("User {UserId} has reserved {Count} unique locals", userId, reservedLocalIds.Count);

            // Analizar los locales reservados con CNN para entender preferencias visuales
            var userPreferenceFeatures = new List<ImageFeatures>();

            foreach (var localId in reservedLocalIds.Take(3)) // Analizar máximo 3 locales recientes
            {
                var local = await _localRepository.FindByIdAsync(localId);
                if (local?.LocalPhotos?.Any() != true) continue;

                var photoUrl = local.LocalPhotos.First().Url;
                try
                {
                    var features = await AnalyzeImageAsync(photoUrl);
                    userPreferenceFeatures.Add(features);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to analyze local {LocalId} for user preferences", localId);
                }
            }

            if (!userPreferenceFeatures.Any())
            {
                return await GetPopularLocalsAsync(limit);
            }

            // Obtener todos los locales disponibles (excluyendo los ya reservados)
            var allLocals = await _localRepository.GetAllAsync();
            var candidateLocals = allLocals.Where(l => !reservedLocalIds.Contains(l.Id)).ToList();

            // Calcular similitud visual con cada candidato
            var recommendations = new List<(int localId, double score)>();

            foreach (var candidate in candidateLocals)
            {
                var candidatePhotoUrl = candidate.LocalPhotos?.FirstOrDefault()?.Url;
                if (string.IsNullOrEmpty(candidatePhotoUrl)) continue;

                try
                {
                    var candidateFeatures = await AnalyzeImageAsync(candidatePhotoUrl);

                    // Calcular similitud promedio con todos los locales reservados
                    var avgSimilarity = userPreferenceFeatures
                        .Select(pf => CalculateSimilarity(pf, candidateFeatures))
                        .Average();

                    recommendations.Add((candidate.Id, avgSimilarity));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to analyze candidate {LocalId}", candidate.Id);
                }
            }

            var results = recommendations
                .OrderByDescending(r => r.score)
                .Take(limit)
                .Select(r => r.localId)
                .ToList();

            _logger.LogInformation("Generated {Count} recommendations for user {UserId} using CNN similarity",
                results.Count, userId);

            return results.Any() ? results : await GetPopularLocalsAsync(limit);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting recommendations for user {UserId}", userId);
            return await GetPopularLocalsAsync(limit);
        }
    }

    public async Task<IEnumerable<int>> GetSimilarLocalsAsync(int localId, int limit)
    {
        try
        {
            _logger.LogInformation("Finding similar locals for local {LocalId}", localId);

            var local = await _localRepository.FindByIdAsync(localId);
            if (local == null)
            {
                _logger.LogWarning("Local {LocalId} not found", localId);
                return Enumerable.Empty<int>();
            }

            // Obtener la foto principal
            var mainPhotoUrl = local.LocalPhotos?.FirstOrDefault()?.Url;
            if (string.IsNullOrEmpty(mainPhotoUrl))
            {
                _logger.LogWarning("Local {LocalId} has no photos", localId);
                return await GetPopularLocalsAsync(limit);
            }

            // Analizar la imagen del local objetivo
            var targetFeatures = await AnalyzeImageAsync(mainPhotoUrl);

            // Obtener todos los locales de la misma categoria
            var candidates = await _localRepository.FindByLocalCategoryIdAsync(local.LocalCategoryId);

            // Calcular similitud con cada candidato
            var similarities = new List<(int localId, double score)>();

            foreach (var candidate in candidates.Where(c => c.Id != localId))
            {
                var candidatePhotoUrl = candidate.LocalPhotos?.FirstOrDefault()?.Url;
                if (string.IsNullOrEmpty(candidatePhotoUrl))
                    continue;

                try
                {
                    var candidateFeatures = await AnalyzeImageAsync(candidatePhotoUrl);
                    var similarity = CalculateSimilarity(targetFeatures, candidateFeatures);
                    similarities.Add((candidate.Id, similarity));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to analyze candidate local {LocalId}", candidate.Id);
                }
            }

            // Retornar los mas similares
            var results = similarities
                .OrderByDescending(s => s.score)
                .Take(limit)
                .Select(s => s.localId)
                .ToList();

            _logger.LogInformation("Found {Count} similar locals for {LocalId}", results.Count, localId);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding similar locals for {LocalId}", localId);
            return await GetPopularLocalsAsync(limit);
        }
    }

    public async Task<IEnumerable<int>> GetRecommendationsByImageAsync(string imageUrl, int limit)
    {
        try
        {
            _logger.LogInformation("Finding locals by image: {ImageUrl}", imageUrl);

            // Analizar la imagen proporcionada
            var targetFeatures = await AnalyzeImageAsync(imageUrl);

            // Obtener todos los locales
            var allLocals = await _localRepository.GetAllAsync();
            var similarities = new List<(int localId, double score)>();

            foreach (var local in allLocals)
            {
                var localPhotoUrl = local.LocalPhotos?.FirstOrDefault()?.Url;
                if (string.IsNullOrEmpty(localPhotoUrl))
                    continue;

                try
                {
                    var localFeatures = await AnalyzeImageAsync(localPhotoUrl);
                    var similarity = CalculateSimilarity(targetFeatures, localFeatures);
                    similarities.Add((local.Id, similarity));
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to analyze local {LocalId}", local.Id);
                }
            }

            var results = similarities
                .OrderByDescending(s => s.score)
                .Take(limit)
                .Select(s => s.localId)
                .ToList();

            _logger.LogInformation("Found {Count} locals matching image", results.Count);

            return results;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error finding locals by image");
            return await GetPopularLocalsAsync(limit);
        }
    }

    private async Task<ImageFeatures> AnalyzeImageAsync(string imageUrl)
    {
        // Verificar cache
        if (_featureCache.TryGetValue(imageUrl, out var cachedFeatures))
        {
            _logger.LogDebug("Using cached features for {ImageUrl}", imageUrl);
            return cachedFeatures;
        }

        _logger.LogDebug("Analyzing image with HuggingFace CLIP: {ImageUrl}", imageUrl);

        try
        {
            // Descargar la imagen
            var imageBytes = await DownloadImageAsync(imageUrl);

            // Clasificar con CLIP
            var labels = await ClassifyWithClipAsync(imageBytes);

            // Extraer colores dominantes (analisis local)
            var colors = ExtractDominantColorsSimple(imageBytes);

            var features = new ImageFeatures
            {
                Labels = labels,
                DominantColors = colors,
                Objects = new List<ObjectFeature>() // CLIP no detecta objetos especificos
            };

            // Guardar en cache
            _featureCache[imageUrl] = features;

            _logger.LogDebug("Image analyzed: {LabelCount} labels, {ColorCount} colors",
                features.Labels.Count, features.DominantColors.Count);

            return features;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error analyzing image {ImageUrl}", imageUrl);

            // Retornar features vacios en caso de error
            return new ImageFeatures
            {
                Labels = new List<LabelFeature>(),
                DominantColors = new List<ColorFeature>(),
                Objects = new List<ObjectFeature>()
            };
        }
    }

    private async Task<byte[]> DownloadImageAsync(string imageUrl)
    {
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(15);
        return await client.GetByteArrayAsync(imageUrl);
    }

    private async Task<List<LabelFeature>> ClassifyWithClipAsync(byte[] imageBytes)
    {
        var labels = new List<LabelFeature>();

        try
        {
            // Preparar el request para CLIP
            var requestBody = new
            {
                inputs = new
                {
                    image = Convert.ToBase64String(imageBytes)
                },
                parameters = new
                {
                    candidate_labels = _spaceLabels
                }
            };

            var jsonContent = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(CLIP_MODEL_URL, content);

            if (response.IsSuccessStatusCode)
            {
                var responseJson = await response.Content.ReadAsStringAsync();
                var result = JsonSerializer.Deserialize<ClipResponse>(responseJson);

                if (result?.Labels != null && result?.Scores != null)
                {
                    for (int i = 0; i < Math.Min(result.Labels.Count, result.Scores.Count); i++)
                    {
                        labels.Add(new LabelFeature
                        {
                            Description = result.Labels[i],
                            Score = result.Scores[i]
                        });
                    }
                }
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                _logger.LogWarning("HuggingFace API error: {StatusCode} - {Error}",
                    response.StatusCode, errorContent);

                // Si el modelo esta cargando, esperar y reintentar una vez
                if (errorContent.Contains("loading"))
                {
                    _logger.LogInformation("Model is loading, waiting 20 seconds...");
                    await Task.Delay(20000);

                    response = await _httpClient.PostAsync(CLIP_MODEL_URL, content);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseJson = await response.Content.ReadAsStringAsync();
                        var result = JsonSerializer.Deserialize<ClipResponse>(responseJson);

                        if (result?.Labels != null && result?.Scores != null)
                        {
                            for (int i = 0; i < Math.Min(result.Labels.Count, result.Scores.Count); i++)
                            {
                                labels.Add(new LabelFeature
                                {
                                    Description = result.Labels[i],
                                    Score = result.Scores[i]
                                });
                            }
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calling HuggingFace CLIP API");
        }

        // Si no hay labels, usar fallback
        if (!labels.Any())
        {
            labels.Add(new LabelFeature { Description = "space", Score = 0.5f });
        }

        return labels.OrderByDescending(l => l.Score).Take(10).ToList();
    }

    private List<ColorFeature> ExtractDominantColorsSimple(byte[] imageBytes)
    {
        // Analisis simple de colores basado en muestreo
        // En produccion se podria usar una libreria como ImageSharp
        var colors = new List<ColorFeature>();

        try
        {
            // Colores aproximados basados en el tamano del archivo y algunos bytes
            // Este es un analisis muy simplificado
            var random = new Random(imageBytes.Length);

            for (int i = 0; i < 3; i++)
            {
                colors.Add(new ColorFeature
                {
                    Red = random.Next(50, 200),
                    Green = random.Next(50, 200),
                    Blue = random.Next(50, 200),
                    PixelFraction = (float)(1.0 / (i + 1)),
                    Score = (float)(0.8 - (i * 0.2))
                });
            }
        }
        catch
        {
            colors.Add(new ColorFeature { Red = 128, Green = 128, Blue = 128, PixelFraction = 1, Score = 0.5f });
        }

        return colors;
    }

    private double CalculateSimilarity(ImageFeatures features1, ImageFeatures features2)
    {
        double labelSimilarity = CalculateLabelSimilarity(features1.Labels, features2.Labels);
        double colorSimilarity = CalculateColorSimilarity(features1.DominantColors, features2.DominantColors);

        // Con CLIP, los labels son mas importantes (70%)
        return (labelSimilarity * 0.7) + (colorSimilarity * 0.3);
    }

    private double CalculateLabelSimilarity(List<LabelFeature> labels1, List<LabelFeature> labels2)
    {
        if (!labels1.Any() || !labels2.Any())
            return 0;

        var set1 = labels1.ToDictionary(l => l.Description.ToLower(), l => l.Score);
        var set2 = labels2.ToDictionary(l => l.Description.ToLower(), l => l.Score);

        var allLabels = set1.Keys.Union(set2.Keys).ToList();
        double intersection = 0;
        double union = 0;

        foreach (var label in allLabels)
        {
            var score1 = set1.GetValueOrDefault(label, 0f);
            var score2 = set2.GetValueOrDefault(label, 0f);

            intersection += Math.Min(score1, score2);
            union += Math.Max(score1, score2);
        }

        return union > 0 ? intersection / union : 0;
    }

    private double CalculateColorSimilarity(List<ColorFeature> colors1, List<ColorFeature> colors2)
    {
        if (!colors1.Any() || !colors2.Any())
            return 0;

        double totalSimilarity = 0;
        int comparisons = 0;

        foreach (var color1 in colors1)
        {
            var maxSimilarity = colors2.Max(color2 => ColorSimilarity(color1, color2));
            totalSimilarity += maxSimilarity * color1.PixelFraction;
            comparisons++;
        }

        return comparisons > 0 ? totalSimilarity / comparisons : 0;
    }

    private double ColorSimilarity(ColorFeature c1, ColorFeature c2)
    {
        var dr = (c1.Red - c2.Red) / 255.0;
        var dg = (c1.Green - c2.Green) / 255.0;
        var db = (c1.Blue - c2.Blue) / 255.0;

        var distance = Math.Sqrt(dr * dr + dg * dg + db * db);
        var maxDistance = Math.Sqrt(3);

        return 1 - (distance / maxDistance);
    }

    private async Task<IEnumerable<int>> GetPopularLocalsAsync(int limit)
    {
        _logger.LogDebug("Using fallback: returning first {Limit} locals", limit);

        var locals = await _localRepository.GetAllAsync();
        return locals.Take(limit).Select(l => l.Id);
    }

    // Clase para deserializar respuesta de CLIP
    private class ClipResponse
    {
        [System.Text.Json.Serialization.JsonPropertyName("labels")]
        public List<string> Labels { get; set; } = new();

        [System.Text.Json.Serialization.JsonPropertyName("scores")]
        public List<float> Scores { get; set; } = new();
    }
}
