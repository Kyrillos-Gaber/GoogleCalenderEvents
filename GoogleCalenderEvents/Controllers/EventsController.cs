using GoogleCalenderEvents.Models;
using GoogleCalenderEvents.Services;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;

namespace GoogleCalenderEvents.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    private readonly IGoogleCalendarService calendarService;

    public EventsController(IGoogleCalendarService calendarService)
    {
        this.calendarService = calendarService;
    }

    [HttpGet("{eventId}")]
    public async Task<IActionResult> GetEvent(string eventId)
    {
        try
        {
            var res = await calendarService.GetEvent(eventId);
            return Ok(new { Status = "Success", Result = res });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Status = "BadRquest", Error = ex.Message });
        }
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateEvent(GoogleCalendarCreate calendarEvent)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
        try
        {
            var res = await calendarService.Create(calendarEvent);
            return Ok(new { Status = "Success", Result = res });
        }
        catch (Exception ex)
        {
            return BadRequest(new { Status = "BadRquest", Error = ex.Message });
        }
    }


    [HttpGet]
    public async Task<IActionResult> GetEvents()
    {
        var res = await calendarService.GetEventsAsync();
        return Ok(new { Status = "Success", Result = res });
    }

    [HttpDelete("{eventId}")]
    public IActionResult DeleteEvent(string eventId)
    {
        try
        {
            calendarService.DeleteEvent(eventId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { Status = "BadRquest", Error = ex.Message });
        }
    }
}
