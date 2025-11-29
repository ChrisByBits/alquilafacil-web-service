using System.Net.Mime;
using AlquilaFacilPlatform.Availability.Domain.Model.Commands;
using AlquilaFacilPlatform.Availability.Domain.Model.Queries;
using AlquilaFacilPlatform.Availability.Domain.Services;
using AlquilaFacilPlatform.Availability.Interfaces.REST.Resources;
using AlquilaFacilPlatform.Availability.Interfaces.REST.Transform;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AlquilaFacilPlatform.Availability.Interfaces.REST;

[ApiController]
[Route("api/v1/[controller]")]
[Produces(MediaTypeNames.Application.Json)]
[Authorize]
public class AvailabilityController(
    IAvailabilityCommandService commandService,
    IAvailabilityQueryService queryService) : ControllerBase
{
    // Availability Calendar Endpoints

    [HttpPost("calendar")]
    public async Task<IActionResult> CreateAvailabilityCalendar([FromBody] CreateAvailabilityCalendarResource resource)
    {
        var command = new CreateAvailabilityCalendarCommand(
            resource.LocalId,
            resource.StartDate,
            resource.EndDate,
            resource.IsAvailable,
            resource.CreatedBy,
            resource.Reason);

        var calendar = await commandService.Handle(command);

        if (calendar == null)
            return Conflict(new { message = "Availability calendar conflicts with existing entries" });

        var calendarResource = AvailabilityCalendarResourceFromEntityAssembler.ToResourceFromEntity(calendar);
        return Created(string.Empty, calendarResource);
    }

    [HttpGet("calendar/local/{localId:int}")]
    public async Task<IActionResult> GetAvailabilityCalendarByLocalId(
        int localId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var query = new GetAvailabilityCalendarByLocalIdQuery(localId, startDate, endDate);
        var calendars = await queryService.Handle(query);

        var resources = calendars.Select(AvailabilityCalendarResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpDelete("calendar/{calendarId:int}")]
    public async Task<IActionResult> DeleteAvailabilityCalendar(int calendarId)
    {
        var command = new DeleteAvailabilityCalendarCommand(calendarId);
        var success = await commandService.Handle(command);

        if (!success)
            return NotFound(new { message = "Availability calendar not found" });

        return Ok(new { message = "Availability calendar deleted successfully" });
    }

    // Blocked Dates Endpoints

    [HttpPost("blocked-dates")]
    public async Task<IActionResult> CreateBlockedDate([FromBody] CreateBlockedDateResource resource)
    {
        var command = new CreateBlockedDateCommand(
            resource.LocalId,
            resource.Date,
            resource.Reason,
            resource.CreatedBy,
            resource.IsRecurring,
            resource.RecurringDayOfWeek);

        var blockedDate = await commandService.Handle(command);

        if (blockedDate == null)
            return BadRequest(new { message = "Cannot create blocked date" });

        var blockedDateResource = BlockedDateResourceFromEntityAssembler.ToResourceFromEntity(blockedDate);
        return Created(string.Empty, blockedDateResource);
    }

    [HttpGet("blocked-dates/local/{localId:int}")]
    public async Task<IActionResult> GetBlockedDatesByLocalId(int localId)
    {
        var query = new GetBlockedDatesByLocalIdQuery(localId);
        var blockedDates = await queryService.Handle(query);

        var resources = blockedDates.Select(BlockedDateResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpDelete("blocked-dates/{blockedDateId:int}")]
    public async Task<IActionResult> DeleteBlockedDate(int blockedDateId)
    {
        var command = new DeleteBlockedDateCommand(blockedDateId);
        var success = await commandService.Handle(command);

        if (!success)
            return NotFound(new { message = "Blocked date not found" });

        return Ok(new { message = "Blocked date deleted successfully" });
    }

    // Availability Rules Endpoints

    [HttpPost("rules")]
    public async Task<IActionResult> CreateAvailabilityRule([FromBody] CreateAvailabilityRuleResource resource)
    {
        if (!TimeSpan.TryParse(resource.StartTime, out var startTime) ||
            !TimeSpan.TryParse(resource.EndTime, out var endTime))
        {
            return BadRequest(new { message = "Invalid time format. Use HH:mm:ss" });
        }

        var command = new CreateAvailabilityRuleCommand(
            resource.LocalId,
            resource.DayOfWeek,
            startTime,
            endTime,
            resource.IsAvailable,
            resource.CreatedBy);

        var rule = await commandService.Handle(command);

        if (rule == null)
            return BadRequest(new { message = "Cannot create availability rule" });

        var ruleResource = AvailabilityRuleResourceFromEntityAssembler.ToResourceFromEntity(rule);
        return Created(string.Empty, ruleResource);
    }

    [HttpGet("rules/local/{localId:int}")]
    public async Task<IActionResult> GetAvailabilityRulesByLocalId(int localId)
    {
        var query = new GetAvailabilityRulesByLocalIdQuery(localId);
        var rules = await queryService.Handle(query);

        var resources = rules.Select(AvailabilityRuleResourceFromEntityAssembler.ToResourceFromEntity);
        return Ok(resources);
    }

    [HttpDelete("rules/{ruleId:int}")]
    public async Task<IActionResult> DeleteAvailabilityRule(int ruleId)
    {
        var command = new DeleteAvailabilityRuleCommand(ruleId);
        var success = await commandService.Handle(command);

        if (!success)
            return NotFound(new { message = "Availability rule not found" });

        return Ok(new { message = "Availability rule deleted successfully" });
    }

    // Availability Check Endpoint

    [HttpGet("check")]
    public async Task<IActionResult> CheckAvailability(
        [FromQuery] int localId,
        [FromQuery] DateTime startDate,
        [FromQuery] DateTime endDate)
    {
        var query = new CheckAvailabilityQuery(localId, startDate, endDate);
        var isAvailable = await queryService.Handle(query);

        return Ok(new
        {
            localId,
            startDate,
            endDate,
            isAvailable,
            message = isAvailable
                ? "The local is available for the requested period"
                : "The local is NOT available for the requested period"
        });
    }
}
