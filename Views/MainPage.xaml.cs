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

        // Загружаем события из базы данных при отображении страницы
        await _viewModel.LoadEventsAsync();
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

}
