using System.Collections.ObjectModel;
using iCalendarReminderApp.Models;
using iCalendarReminderApp.Services;

namespace iCalendarReminderApp.ViewModels;

public class MainViewModel
{
    public ObservableCollection<Event> Events { get; set; }

    private readonly DatabaseService _databaseService;
    private readonly ICalendarService _iCalendarService;

    public MainViewModel(string dbPath)
    {
        _databaseService = new DatabaseService(dbPath);
        _iCalendarService = new ICalendarService();

        Events = new ObservableCollection<Event>();
    }

    public async Task LoadEventsAsync()
    {
        var events = await _databaseService.GetEventsAsync();
        Events.Clear();

        foreach (var ev in events)
            Events.Add(ev);
    }

    public async Task ImportCalendar(string filePath)
    {
        var importedEvents = _iCalendarService.ImportFromICalendar(filePath);

        foreach (var ev in importedEvents)
        {
            await _databaseService.SaveEventAsync(ev);
            Events.Add(ev);
        }
    }
}
