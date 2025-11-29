namespace AlquilaFacilPlatform.Availability.Domain.Model.Queries;

public record CheckAvailabilityQuery(int LocalId, DateTime StartDate, DateTime EndDate);
