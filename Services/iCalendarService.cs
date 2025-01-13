using Ical.Net;
using System.Globalization;
using System.Linq;
namespace iCalendarReminderApp.Services;

public class ICalendarService
{
    public Dictionary<DateTime, List<Event>> ImportAndGroupByDay(string filePath)
    {
        try
        {
            if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
            {
                throw new FileNotFoundException($"File not found: {filePath}");
            }

            var fileContent = File.ReadAllText(filePath);

            if (string.IsNullOrWhiteSpace(fileContent))
            {
                throw new InvalidDataException("The file is empty or invalid.");
            }

            var calendarCollection = CalendarCollection.Load(fileContent);
            var events = new List<Event>();

            foreach (var calendar in calendarCollection)
            {
                foreach (var e in calendar.Events)
                {
                    events.Add(new Event
                    {
                        Title = e.Summary ?? string.Empty,
                        Location = e.Location ?? string.Empty,
                        StartTime = e.DtStart.AsSystemLocal,
                        EndTime = e.DtEnd.AsSystemLocal,
                        Description = e.Description ?? string.Empty,
                    });
                }
            }

            // Сортируем и группируем по дате (без учета времени)
            var groupedEvents = events
                .OrderBy(ev => ev.StartTime) // Сортируем по времени начала
                .GroupBy(ev => ev.StartTime.Date) // Группируем по дате
                .ToDictionary(g => g.Key, g => g.ToList()); // Преобразуем в словарь

            return groupedEvents;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error importing iCalendar: {ex.Message}");
            return new Dictionary<DateTime, List<Event>>();
        }
    }

}
