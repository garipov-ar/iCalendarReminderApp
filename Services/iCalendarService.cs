using iCalendarReminderApp.Models;
using Ical.Net;
using System.Globalization;

namespace iCalendarReminderApp.Services;

public class ICalendarService
{
    public List<Event> ImportFromICalendar(string filePath)
    {
        var calendarCollection = CalendarCollection.Load(File.ReadAllText(filePath));
        var events = new List<Event>();

        foreach (var calendar in calendarCollection)
        {
            foreach (var e in calendar.Events)
            {
                events.Add(new Event
                {
                    Title = e.Summary,
                    Location = e.Location,
                    StartTime = e.DtStart.AsSystemLocal,
                    EndTime = e.DtEnd.AsSystemLocal,
                    Description = e.Description,
                });
            }
        }

        return events;
    }
}
