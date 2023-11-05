using GoogleCalenderEvents.Models;

namespace GoogleCalenderEvents.Services;

public interface IGoogleCalendarService
{
    void DeleteEvent(string eventId);

    Task<Tuple<IEnumerable<GoogleCalendar>, string>> GetEventsAsync(DateTime? fromDate = null, 
        DateTime? toDate = null, string? pageToken = null, int? resultsCount = null, string ? searchQuery = null);

    Task<GoogleCalendar> Create(GoogleCalendarCreate @event);

    Task<GoogleCalendar> GetEvent(string eventId);
}
