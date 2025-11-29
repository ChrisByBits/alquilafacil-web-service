using AlquilaFacilPlatform.Locals.Domain.Model.Commands;
using AlquilaFacilPlatform.Locals.Domain.Model.Entities;
using AlquilaFacilPlatform.Locals.Domain.Model.ValueObjects;

namespace AlquilaFacilPlatform.Locals.Domain.Model.Aggregates;

public partial class Local
{
    public Local()
    {
        Name = new LocalName();
        Description = new DescriptionMessage();
        Country = new Country();
        City = new City();
        District = new District();
        Street = new Street();
        PricePerHour = new PricePerHour();
        Capacity = 0;
        Features = string.Empty;
        UserId = 0;
    }
    
    
    public Local(
        string localName,
        string descriptionMessage,
        string country,
        string city,
        string district,
        string street,
        int price,
        int capacity,
        string features,
        int localCategoryId,
        int userId
        ) 
    { 
        Name = new LocalName(localName);
        Description = new DescriptionMessage(descriptionMessage);
        Country = new Country(country);
        City = new City(city);
        District = new District(district);
        Street = new Street(street);
        PricePerHour = new PricePerHour(price);
        Capacity = capacity;
        Features = features;
        LocalCategoryId = localCategoryId;
        UserId = userId;
    }

    public Local(CreateLocalCommand command)
    {
        Name = new LocalName(command.LocalName);
        Description = new DescriptionMessage(command.DescriptionMessage);
        Country = new Country(command.Country);
        City = new City(command.City);
        District = new District(command.District);
        Street = new Street(command.Street);
        PricePerHour = new PricePerHour(command.Price);
        LocalCategoryId = command.LocalCategoryId;
        Features = command.Features;
        Capacity = command.Capacity;
        UserId = command.UserId;
        Latitude = command.Latitude;
        Longitude = command.Longitude;
    }


    public void Update(UpdateLocalCommand command)
    {
        Name = new LocalName(command.LocalName);
        Description = new DescriptionMessage(command.DescriptionMessage);
        Country = new Country(command.Country);
        City = new City(command.City);
        District = new District(command.District);
        Street = new Street(command.Street);
        PricePerHour = new PricePerHour(command.Price);
        Capacity = command.Capacity;
        Features = command.Features;
        LocalCategoryId = command.LocalCategoryId;
        UserId = command.UserId;
        Latitude = command.Latitude;
        Longitude = command.Longitude;
    }

    public int Id { get; set; }
    public LocalName Name { get; private set; }
    public DescriptionMessage Description { get; private set; }
    public Country Country { get; private set; }
    public City City { get; private set; }
    public District District { get; private set; }
    public Street Street { get; private set; }
    public PricePerHour PricePerHour { get; private set; }
    public int Capacity { get; set; }
    public string Features { get; set; }
    public int LocalCategoryId { get; set; }
    public int UserId { get; set; }
    public ICollection<LocalPhoto> LocalPhotos { get; set; } = new List<LocalPhoto>();

    // Google Maps coordinates
    public double? Latitude { get; set; }
    public double? Longitude { get; set; }

    public string LocalName => Name.Value;
    public string DescriptionMessage => Description.Value;
    public string Address => $"{Street.Value}, {District.Value}, {City.Value}, {Country.Value}";
    public int Price => PricePerHour.Value;

    public void UpdateCoordinates(double? latitude, double? longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }
}