using SQLite;

namespace iCalendarReminderApp.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _database;

    public DatabaseService(string dbPath)
    {
        _database = new SQLiteAsyncConnection(dbPath);
        _database.CreateTableAsync<Event>().Wait(); // Убедитесь, что таблица создается
    }

    public async Task<List<Event>> GetEventsAsync()
    {
        try
        {
            // Извлекаем все записи из таблицы Event
            var events = await _database.Table<Event>().ToListAsync();
            Console.WriteLine($"Successfully retrieved {events.Count} events from the database.");
            return events;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error retrieving events: {ex.Message}");
            return new List<Event>(); // Возвращаем пустой список в случае ошибки
        }
    }


    public async Task SaveEventAsync(Event ev)
    {
        // Добавьте проверку, чтобы убедиться, что объект сохраняется
        if (ev.Id == 0)
        {
            await _database.InsertAsync(ev); // Сохраняем новый объект
        }
        else
        {
            await _database.UpdateAsync(ev); // Обновляем, если уже существует
        }
    }

    public async Task DeleteEventAsync(Event eventToDelete)
    {
        try
        {
            // Удаление записи по уникальному идентификатору
            await _database.Table<Event>().DeleteAsync(e => e.Id == eventToDelete.Id);

            Console.WriteLine($"Event with ID {eventToDelete.Id} successfully deleted.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting event from database: {ex.Message}");
        }
    }


}
