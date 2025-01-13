using SQLite;

public class Event
{
    [PrimaryKey, AutoIncrement] // SQLite требует использования этих атрибутов
    public int Id { get; set; }

    [MaxLength(100), NotNull] // Используем SQLite-аналог для атрибута Required
    public string Title { get; set; } = string.Empty;

    [MaxLength(200)]
    public string Location { get; set; } = string.Empty;

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Link { get; set; } = string.Empty;

    [MaxLength(300)]
    public string Notes { get; set; } = string.Empty;
}
