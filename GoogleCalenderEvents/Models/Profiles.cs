using AutoMapper;
using Google.Apis.Calendar.v3.Data;
using System.ComponentModel;

namespace GoogleCalenderEvents.Models;

public class Profiles : Profile
{
    public Profiles()
    {
        CreateMap<Event, GoogleCalendar>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Start, opt => opt.MapFrom(src => src.Start.DateTime))
            .ForMember(dest => dest.End, opt => opt.MapFrom(src => src.End.DateTime))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location));

        CreateMap<GoogleCalendarCreate, Event>()
            .ForPath(dest => dest.Start.DateTimeDateTimeOffset, opt => opt.MapFrom(src => src.Start))
            .ForPath(dest => dest.Start.TimeZone, opt => opt.MapFrom(src => src.TimeZone))
            .ForPath(dest => dest.End.DateTimeDateTimeOffset, opt => opt.MapFrom(src => src.End))
            .ForPath(dest => dest.End.TimeZone, opt => opt.MapFrom(src => src.TimeZone))
            .ForMember(dest => dest.Summary, opt => opt.MapFrom(src => src.Summary))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Location, opt => opt.MapFrom(src => src.Location));

    }
}
