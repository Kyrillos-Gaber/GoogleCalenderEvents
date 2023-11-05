﻿namespace GoogleCalenderEvents.Models;

public class GoogleCalendar
{
    public string? Id { get; set; }

    public string? Summary { get; set; }

    public string? Description { get; set; }
    
    public string? Status { get; set; }

    public string? Location { get; set; }

    public DateTime Start { get; set; }
    
    public DateTime End { get; set; }

}
