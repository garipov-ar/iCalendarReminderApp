using SQLite;
using iCalendarReminderApp.Models;

namespace iCalendarReminderApp.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

    public DatabaseService(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<Event>().Wait();
    }

    public Task<List<Event>> GetEventsAsync() => _database.Table<Event>().ToListAsync();

    public Task<int> SaveEventAsync(Event ev) => _database.InsertOrReplaceAsync(ev);

    public Task<int> DeleteEventAsync(Event ev) => _database.DeleteAsync(ev);
}
