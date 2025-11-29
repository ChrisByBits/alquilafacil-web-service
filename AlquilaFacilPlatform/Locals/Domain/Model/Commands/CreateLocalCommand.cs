namespace AlquilaFacilPlatform.Locals.Domain.Model.Commands;

public record CreateLocalCommand(
    string LocalName,
    string DescriptionMessage,
    string Country,
    string City,
    string District,
    string Street,
    int Price,
    int Capacity,
    IEnumerable<string> PhotoUrls,
    string Features,
    int LocalCategoryId,
    int UserId,
    double? Latitude = null,
    double? Longitude = null
);