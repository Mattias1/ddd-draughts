namespace Draughts.Application.Shared;

public sealed class AppSettings {
    public string? BaseUrl { get; set; }
    public int? Port { get; set; }
    public int? HttpsPort { get; set; }
    public string? LogDir { get; set; }
    public string? LogLevel { get; set; }
    public string? DbServer { get; set; }
    public int? DbPort { get; set; }
    public string? DbPassword { get; set; }
    public string? CertificateFile { get; set; }
    public string? CertificatePassword { get; set; }
}
