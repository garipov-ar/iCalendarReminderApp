using System.Collections.ObjectModel;
using iCalendarReminderApp.Services;
using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace iCalendarReminderApp.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<IGrouping<DateTime, Event>> GroupedEvents { get; set; }
        public ObservableCollection<Event> Events { get; set; }

        private readonly DatabaseService _databaseService;
        private readonly ICalendarService _iCalendarService;

        public MainViewModel(string dbPath)
        {
            _databaseService = new DatabaseService(dbPath);
            _iCalendarService = new ICalendarService();

            Events = new ObservableCollection<Event>();
            GroupedEvents = new ObservableCollection<IGrouping<DateTime, Event>>();
        }

        public async Task DeleteEventAsync(Event eventToDelete)
        {
            try
            {
                // Удаляем событие из базы данных
                await _databaseService.DeleteEventAsync(eventToDelete);

                // Перезагружаем все события после удаления, чтобы обновить GroupedEvents
                await LoadEventsAsync();  // Это обновит GroupedEvents и синхронизирует интерфейс с базой данных

                // Уведомление интерфейса об изменениях
                OnPropertyChanged(nameof(GroupedEvents));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting event: {ex.Message}");
                throw new Exception("Error deleting event", ex);  // Генерируем исключение, если не удалось удалить
            }
        }

        public async Task LoadEventsAsync(bool showPastEvents = false)
        {
            try
            {
                var events = await _databaseService.GetEventsAsync();

                // Фильтрация событий: показываем только будущие или все события, в зависимости от флага
                if (!showPastEvents)
                {
                    events = events.Where(e => e.StartTime >= DateTime.Now).ToList();
                }

                foreach (var ev in events)
                {
                    // Лог для проверки, что `Id` у объектов установлен
                    Console.WriteLine($"Loaded Event: Id={ev.Id}, Title={ev.Title}");
                }

                var groupedEvents = events
                    .OrderBy(e => e.StartTime)
                    .GroupBy(e => e.StartTime.Date)
                    .ToList();

                GroupedEvents.Clear();

                foreach (var group in groupedEvents)
                {
                    GroupedEvents.Add(group);
                }

                OnPropertyChanged(nameof(GroupedEvents));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading events: {ex.Message}");
            }
        }



        public async Task UpdateEventAsync(Event eventToUpdate)
        {
            try
            {
                await _databaseService.SaveEventAsync(eventToUpdate); // Обновляем запись
                await LoadEventsAsync(); // Перезагружаем список
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating event: {ex.Message}");
                throw;
            }
        }

        public async Task DeleteAllEventsAsync()
        {
            try
            {
                await _databaseService.DeleteAllEventsAsync();  // Вызов метода для удаления всех событий
                await LoadEventsAsync();  // Перезагружаем все события
                OnPropertyChanged(nameof(GroupedEvents));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error deleting all events: {ex.Message}");
            }
        }

        public async Task ImportCalendar(string filePath)
        {
            var groupedEvents = _iCalendarService.ImportAndGroupByDay(filePath);

            // Сохранение событий в базу данных
            foreach (var group in groupedEvents)
            {
                foreach (var ev in group.Value)
                {
                    ev.Id = 0; // Заставляем SQLite присваивать новый идентификатор
                    await _databaseService.SaveEventAsync(ev);
                }
            }
            await LoadEventsAsync();  // Это обновит GroupedEvents и синхронизирует интерфейс с базой данных


         

            // Уведомление интерфейса об изменениях
            OnPropertyChanged(nameof(GroupedEvents));
        }

        // Реализация INotifyPropertyChanged для обновления UI
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }



    // Класс Grouping для работы с IGrouping
    public class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly IEnumerable<TElement> _elements;

        public Grouping(TKey key, IEnumerable<TElement> elements)
        {
            Key = key;
            _elements = elements;
        }

        public TKey Key { get; }

        public IEnumerator<TElement> GetEnumerator() => _elements.GetEnumerator();

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
