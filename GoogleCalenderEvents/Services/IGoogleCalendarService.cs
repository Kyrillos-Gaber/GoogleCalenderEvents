using GoogleCalenderEvents.Models;

namespace GoogleCalenderEvents.Services;

public interface IGoogleCalendarService
{
    void DeleteEvent(string eventId);

    Task<IEnumerable<GoogleCalendar>> GetEventsAsync();

    Task<GoogleCalendar> Create(GoogleCalendarCreate @event);

    Task<GoogleCalendar> GetEvent(string eventId);
}
