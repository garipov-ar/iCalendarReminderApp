using iCalendarReminderApp.ViewModels;

namespace iCalendarReminderApp.Views;

public partial class MainPage : ContentPage
{
    private readonly MainViewModel _viewModel;

    public MainPage()
    {
        InitializeComponent();

        // Путь к локальной базе данных SQLite
        var databasePath = Path.Combine(FileSystem.AppDataDirectory, "events.db");

        // Инициализация ViewModel
        _viewModel = new MainViewModel(databasePath);

        // Привязываем ViewModel к странице
        BindingContext = _viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        try
        {
            // Загружаем события из базы данных при отображении страницы
            await _viewModel.LoadEventsAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading events: {ex.Message}");
        }
    }

    private async void OnImportClicked(object sender, EventArgs e)
    {
        try
        {
            // Определяем фильтр для файлов .ics
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
        {
            { DevicePlatform.Android, new[] { "application/octet-stream", "text/calendar" } }, // Android
            { DevicePlatform.iOS, new[] { "public.text", "public.data" } },                   // iOS
            { DevicePlatform.MacCatalyst, new[] { "public.text", "public.data" } },           // MacCatalyst
            { DevicePlatform.WinUI, new[] { ".ics" } }                                        // Windows
        });

            var pickOptions = new PickOptions
            {
                PickerTitle = "Выберите файл iCalendar",
                FileTypes = customFileType
            };

            // Выбираем файл
            var result = await FilePicker.PickAsync(pickOptions);

            if (result != null)
            {
                // Импортируем выбранный файл
                await _viewModel.ImportCalendar(result.FullPath);
                await DisplayAlert("Успех", "События успешно импортированы!", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ошибка", $"Не удалось импортировать файл: {ex.Message}", "OK");
        }
    }

    private async void OnNavigateToLinkClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var link = (string)button.CommandParameter;

        if (Uri.TryCreate(link, UriKind.Absolute, out var uri))
        {
            await Launcher.OpenAsync(uri);
        }
        else
        {
            await DisplayAlert("Ошибка", "Некорректная ссылка.", "OK");
        }
    }

    private async void OnEditEventClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var eventToEdit = (Event)button.CommandParameter;

        string result = await DisplayPromptAsync(
            "Редактировать событие",
            "Введите ссылку на трансляцию:",
            initialValue: eventToEdit.Link,
            placeholder: "https://");

        if (!string.IsNullOrWhiteSpace(result))
        {
            eventToEdit.Link = result;

            try
            {
                // Обновляем запись в базе данных
                await _viewModel.UpdateEventAsync(eventToEdit);
                await DisplayAlert("Успех", "Ссылка обновлена.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось обновить ссылку: {ex.Message}", "OK");
            }
        }
    }

    private void OnThemeToggleClicked(object sender, EventArgs e)
    {
        var currentTheme = Application.Current.UserAppTheme;

        if (currentTheme == AppTheme.Light)
        {
            // Переключаем на темную тему
            Application.Current.UserAppTheme = AppTheme.Dark;

            // Меняем иконку на солнце
            ThemeToggleButton.ImageSource = "sun_icon.png"; // Меняем иконку
        }
        else
        {
            // Переключаем на светлую тему
            Application.Current.UserAppTheme = AppTheme.Light;

            // Меняем иконку на луну
            ThemeToggleButton.ImageSource = "moon_icon.png"; // Меняем иконку
        }
    }


    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        var eventToDelete = (Event)button.CommandParameter;

        if (eventToDelete == null)
        {
            Console.WriteLine("CommandParameter is null.");
            await DisplayAlert("Ошибка", "Событие не найдено.", "OK");
            return;
        }

        Console.WriteLine($"Attempting to delete: Id={eventToDelete.Id}, Title={eventToDelete.Title}");

        if (eventToDelete.Id <= 0)
        {
            await DisplayAlert("Ошибка", "Некорректный идентификатор события.", "OK");
            return;
        }

        var confirm = await DisplayAlert("Подтверждение", "Вы уверены, что хотите удалить это событие?", "Да", "Нет");
        if (confirm)
        {
            try
            {
                await _viewModel.DeleteEventAsync(eventToDelete);
                await DisplayAlert("Удалено", "Событие успешно удалено.", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Ошибка", $"Не удалось удалить событие: {ex.Message}", "OK");
            }
        }
    }

    private async void OnButtonPressed(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button != null)
        {
            await button.ScaleTo(1.2, 100); // Увеличение кнопки при нажатии

        }
    }

    private async void OnButtonReleased(object sender, EventArgs e)
    {
        var button = sender as Button;
        if (button != null)
        {
            await button.ScaleTo(1, 100); // Восстановление нормального размера
            
        }
    }



}
