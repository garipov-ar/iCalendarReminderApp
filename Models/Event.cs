namespace iCalendarReminderApp.Models;

public class Event
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Link { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
