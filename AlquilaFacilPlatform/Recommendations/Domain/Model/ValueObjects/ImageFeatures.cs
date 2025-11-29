namespace AlquilaFacilPlatform.Recommendations.Domain.Model.ValueObjects;

public class ImageFeatures
{
    public List<LabelFeature> Labels { get; set; } = new();
    public List<ColorFeature> DominantColors { get; set; } = new();
    public List<ObjectFeature> Objects { get; set; } = new();
}

public class LabelFeature
{
    public string Description { get; set; } = string.Empty;
    public float Score { get; set; }
}

public class ColorFeature
{
    public float Red { get; set; }
    public float Green { get; set; }
    public float Blue { get; set; }
    public float PixelFraction { get; set; }
    public float Score { get; set; }
}

public class ObjectFeature
{
    public string Name { get; set; } = string.Empty;
    public float Score { get; set; }
}
