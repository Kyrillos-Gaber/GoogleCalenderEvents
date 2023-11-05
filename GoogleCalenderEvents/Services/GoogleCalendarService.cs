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
        string[] scopes = { "https://www.googleapis.com/auth/calendar", "https://www.googleapis.com/auth/calendar/events" };

        string appName = configuration.GetSection("GoogleAppName").Value!;
        //string appName = "ATechnologyTask-KyrillosGaber";

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
            var res = await calendarService.Events.Get(CALENDAR_ID, eventId).ExecuteAsync();
            return mapper.Map<GoogleCalendar>(res);
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


    public async Task<IEnumerable<GoogleCalendar>> GetEventsAsync()
    {
        try
        {
            EventsResource.ListRequest request = calendarService.Events.List(CALENDAR_ID);
            request.MaxResults = 2500;
            Events events = await request.ExecuteAsync();

            var list = mapper.Map<IEnumerable<GoogleCalendar>>(events.Items);

            return list;
        }
        catch (Exception ex)
        {
            // Handle exception or log the error.
            throw ex;
        }
    }

    public void DeleteEvent(string eventId)
    {
        try
        {
            calendarService.Events.Delete(CALENDAR_ID, eventId).Execute();
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

}

