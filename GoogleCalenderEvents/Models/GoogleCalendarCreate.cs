using System.ComponentModel.DataAnnotations;

namespace GoogleCalenderEvents.Models;

public class GoogleCalendarCreate
{
    [MaxLength(200)]
    public string? Summary { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(100)]
    public string? Location { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime Start { get; set; }

    [Required, DataType(DataType.Date)]
    public DateTime End { get; set; }

    [MaxLength(100)]
    public string? TimeZone { get; set; }
}
