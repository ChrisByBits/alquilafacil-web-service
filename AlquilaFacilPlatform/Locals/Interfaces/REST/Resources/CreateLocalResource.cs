namespace AlquilaFacilPlatform.Locals.Interfaces.REST.Resources;

public record CreateLocalResource(
    string LocalName,
    string DescriptionMessage,
    int Price,
    int Capacity,
    string Country,
    string City,
    string District,
    string Street,
    IEnumerable<string> PhotoUrls,
    string Features,
    int LocalCategoryId,
    int UserId,
    double? Latitude = null,
    double? Longitude = null
);