using AlquilaFacilPlatform.Availability.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Availability.Domain.Model.Commands;

namespace AlquilaFacilPlatform.Availability.Domain.Services;

public interface IAvailabilityCommandService
{
    Task<AvailabilityCalendar?> Handle(CreateAvailabilityCalendarCommand command);
    Task<AvailabilityCalendar?> Handle(UpdateAvailabilityCalendarCommand command);
    Task<bool> Handle(DeleteAvailabilityCalendarCommand command);
    Task<BlockedDate?> Handle(CreateBlockedDateCommand command);
    Task<bool> Handle(DeleteBlockedDateCommand command);
    Task<AvailabilityRule?> Handle(CreateAvailabilityRuleCommand command);
    Task<bool> Handle(DeleteAvailabilityRuleCommand command);
}
