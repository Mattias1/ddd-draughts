namespace Draughts.Application.Shared;

public sealed class AppSettings {
    public string? BaseUrl { get; set; }
    public string? LogDir { get; set; }
    public string? LogLevel { get; set; }
    public int? DbPort { get; set; }
    public string? DbPassword { get; set; }
}
