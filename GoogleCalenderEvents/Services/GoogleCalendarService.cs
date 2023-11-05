using AutoMapper;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Calendar.v3;
using Google.Apis.Calendar.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using GoogleCalenderEvents.Models;

namespace GoogleCalenderEvents.Services;

public class GoogleCalendarService : IGoogleCalendarService
{

    private readonly IConfiguration configuration;
    private readonly IMapper mapper;
    private readonly CalendarService calendarService;
    private const string CALENDAR_ID = "primary";

    public GoogleCalendarService(IConfiguration configuration, IMapper mapper)
    {
        this.configuration = configuration;
        this.mapper = mapper;
        calendarService = CreateCalendarService();
    }

    private CalendarService CreateCalendarService()
    {
        string[] scopes = { "https://www.googleapis.com/auth/calendar" };

        string appName = configuration.GetSection("GoogleAppName").Value!;

        UserCredential credential;

        using (var stream = new FileStream(
            Path.Combine(Directory.GetCurrentDirectory(), "Credentials", "client_secret.json"),
            FileMode.Open, FileAccess.Read))
        {
            string credentialPath = "token.json";
            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStreamAsync(stream).Result.Secrets,
                scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(credentialPath, true)).Result;
        }
        
        return
            new CalendarService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = appName,
            });
    }

    public async Task<GoogleCalendar> GetEvent(string eventId)
    {
        try
        {
            var @event = await calendarService.Events.Get(CALENDAR_ID, eventId).ExecuteAsync();

            return mapper.Map<GoogleCalendar>(@event);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    private void CheckDateNotHolidayOrInPast(DateTime dateTime)
    {
        if (dateTime < DateTime.Now)
            throw new InvalidOperationException("Cannot create events in the past.");
        if (dateTime.DayOfWeek is DayOfWeek.Friday or DayOfWeek.Saturday)
            throw new InvalidOperationException("Cannot create events on Fridays or Saturdays.");
    }

    public async Task<GoogleCalendar> Create(GoogleCalendarCreate @event)
    {
        try
        {
            CheckDateNotHolidayOrInPast(@event.Start);

            Event eventCalendar = mapper.Map<Event>(@event);

            EventsResource.InsertRequest eventRequest = calendarService.Events.Insert(eventCalendar, CALENDAR_ID);
            Event requestCreate = await eventRequest.ExecuteAsync();

            return mapper.Map<GoogleCalendar>(requestCreate);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }


    public async Task<Tuple<IEnumerable<GoogleCalendar>, string>> GetEventsAsync(
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? pageToken = null,
        int? resultsCount = null,
        string? searchQuery = null)
    {
        try
        {
            EventsResource.ListRequest request = calendarService.Events.List(CALENDAR_ID);

            if (fromDate.HasValue)
                request.TimeMin = fromDate.Value;
            if (toDate.HasValue)
                request.TimeMax = toDate.Value;

            if (!string.IsNullOrWhiteSpace(searchQuery))
                request.Q = searchQuery;

            request.MaxResults =
                (resultsCount <= 0 || resultsCount > 2500 || resultsCount is null) ? 10 : resultsCount;

            if (pageToken is not null)
                request.PageToken = pageToken;

            Events events = await request.ExecuteAsync();

            var result = mapper.Map<IEnumerable<GoogleCalendar>>(events.Items);

            return
                new Tuple<IEnumerable<GoogleCalendar>, string>(result, events.NextPageToken);
        }
        catch (Exception ex)
        {
            // Handle exception or log the error.
            throw ex;
        }
    }

    public async Task DeleteEvent(string eventId)
    {
        try
        {
            await calendarService.Events.Delete(CALENDAR_ID, eventId).ExecuteAsync();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

}

