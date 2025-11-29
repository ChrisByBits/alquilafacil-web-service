namespace AlquilaFacilPlatform.Availability.Domain.Model.Queries;

public record GetAvailabilityCalendarByLocalIdQuery(int LocalId, DateTime StartDate, DateTime EndDate);
