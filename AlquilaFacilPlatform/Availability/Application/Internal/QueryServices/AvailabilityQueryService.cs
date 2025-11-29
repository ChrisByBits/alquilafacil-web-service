using AlquilaFacilPlatform.Availability.Domain.Model.Aggregates;
using AlquilaFacilPlatform.Availability.Domain.Model.Queries;
using AlquilaFacilPlatform.Availability.Domain.Repositories;
using AlquilaFacilPlatform.Availability.Domain.Services;

namespace AlquilaFacilPlatform.Availability.Application.Internal.QueryServices;

public class AvailabilityQueryService(
    IAvailabilityCalendarRepository calendarRepository,
    IBlockedDateRepository blockedDateRepository,
    IAvailabilityRuleRepository ruleRepository) : IAvailabilityQueryService
{
    public async Task<IEnumerable<AvailabilityCalendar>> Handle(GetAvailabilityCalendarByLocalIdQuery query)
    {
        return await calendarRepository.FindByLocalIdAndDateRangeAsync(
            query.LocalId,
            query.StartDate,
            query.EndDate);
    }

    public async Task<IEnumerable<BlockedDate>> Handle(GetBlockedDatesByLocalIdQuery query)
    {
        return await blockedDateRepository.FindByLocalIdAsync(query.LocalId);
    }

    public async Task<IEnumerable<AvailabilityRule>> Handle(GetAvailabilityRulesByLocalIdQuery query)
    {
        return await ruleRepository.FindByLocalIdAsync(query.LocalId);
    }

    public async Task<bool> Handle(CheckAvailabilityQuery query)
    {
        // Check 1: Availability Calendar (unavailable periods)
        var unavailablePeriods = await calendarRepository.FindConflictsAsync(
            query.LocalId,
            query.StartDate,
            query.EndDate);

        if (unavailablePeriods.Any())
            return false;

        // Check 2: Blocked Dates
        var blockedDates = await blockedDateRepository.FindByLocalIdAndDateRangeAsync(
            query.LocalId,
            query.StartDate,
            query.EndDate);

        var currentDate = query.StartDate.Date;
        while (currentDate < query.EndDate.Date)
        {
            foreach (var blockedDate in blockedDates)
            {
                if (blockedDate.IsDateBlocked(currentDate))
                    return false;
            }
            currentDate = currentDate.AddDays(1);
        }

        // Check 3: Availability Rules (day of week and time restrictions)
        var rules = await ruleRepository.FindByLocalIdAsync(query.LocalId);

        if (rules.Any())
        {
            var checkDateTime = query.StartDate;
            while (checkDateTime < query.EndDate)
            {
                var dayOfWeek = (int)checkDateTime.DayOfWeek;
                var dayRules = rules.Where(r => r.DayOfWeek == dayOfWeek).ToList();

                if (dayRules.Any())
                {
                    var timeOfDay = checkDateTime.TimeOfDay;
                    var isTimeAvailable = dayRules.Any(r => r.IsTimeAvailable(timeOfDay));

                    if (!isTimeAvailable)
                        return false;
                }

                checkDateTime = checkDateTime.AddHours(1);
            }
        }

        return true;
    }
}
