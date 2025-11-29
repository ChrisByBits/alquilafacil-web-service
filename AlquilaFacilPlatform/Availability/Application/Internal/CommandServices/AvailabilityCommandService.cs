using AlquilaFacilPlatform.Availability.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Availability.Domain.Model.Commands;
using AlquilaFacilPlatform.Availability.Domain.Repositories;
using AlquilaFacilPlatform.Availability.Domain.Services;
using AlquilaFacilPlatform.Shared.Domain.Repositories;

namespace AlquilaFacilPlatform.Availability.Application.Internal.CommandServices;

public class AvailabilityCommandService(
    IAvailabilityCalendarRepository calendarRepository,
    IBlockedDateRepository blockedDateRepository,
    IAvailabilityRuleRepository ruleRepository,
    IUnitOfWork unitOfWork) : IAvailabilityCommandService
{
    public async Task<AvailabilityCalendar?> Handle(CreateAvailabilityCalendarCommand command)
    {
        // Check for conflicts
        var conflicts = await calendarRepository.FindConflictsAsync(
            command.LocalId,
            command.StartDate,
            command.EndDate);

        if (conflicts.Any())
        {
            // Return null or throw exception if there are conflicts
            return null;
        }

        var calendar = new AvailabilityCalendar(
            command.LocalId,
            command.StartDate,
            command.EndDate,
            command.IsAvailable,
            command.CreatedBy,
            command.Reason);

        await calendarRepository.AddAsync(calendar);
        await unitOfWork.CompleteAsync();

        return calendar;
    }

    public async Task<AvailabilityCalendar?> Handle(UpdateAvailabilityCalendarCommand command)
    {
        var calendar = await calendarRepository.FindByIdAsync(command.CalendarId);

        if (calendar == null)
            return null;

        calendar.Update(command.StartDate, command.EndDate, command.IsAvailable, command.Reason);
        await unitOfWork.CompleteAsync();

        return calendar;
    }

    public async Task<bool> Handle(DeleteAvailabilityCalendarCommand command)
    {
        var calendar = await calendarRepository.FindByIdAsync(command.CalendarId);

        if (calendar == null)
            return false;

        calendarRepository.Remove(calendar);
        await unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<BlockedDate?> Handle(CreateBlockedDateCommand command)
    {
        var blockedDate = new BlockedDate(
            command.LocalId,
            command.Date,
            command.Reason,
            command.CreatedBy,
            command.IsRecurring,
            command.RecurringDayOfWeek);

        await blockedDateRepository.AddAsync(blockedDate);
        await unitOfWork.CompleteAsync();

        return blockedDate;
    }

    public async Task<bool> Handle(DeleteBlockedDateCommand command)
    {
        var blockedDates = await blockedDateRepository.FindByLocalIdAsync(0); // Will need to add FindByIdAsync
        var blockedDate = blockedDates.FirstOrDefault(b => b.Id == command.BlockedDateId);

        if (blockedDate == null)
            return false;

        blockedDateRepository.Remove(blockedDate);
        await unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<AvailabilityRule?> Handle(CreateAvailabilityRuleCommand command)
    {
        var rule = new AvailabilityRule(
            command.LocalId,
            command.DayOfWeek,
            command.StartTime,
            command.EndTime,
            command.IsAvailable,
            command.CreatedBy);

        await ruleRepository.AddAsync(rule);
        await unitOfWork.CompleteAsync();

        return rule;
    }

    public async Task<bool> Handle(DeleteAvailabilityRuleCommand command)
    {
        var rules = await ruleRepository.FindByLocalIdAsync(0); // Will need to add FindByIdAsync
        var rule = rules.FirstOrDefault(r => r.Id == command.RuleId);

        if (rule == null)
            return false;

        ruleRepository.Remove(rule);
        await unitOfWork.CompleteAsync();

        return true;
    }
}
