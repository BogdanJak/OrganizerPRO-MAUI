namespace OrganizerPRO.Infrastructure.Configurations;

public class AppConfigurationSettings : IApplicationSettings
{
    public const string Key = nameof(AppConfigurationSettings);
    public string ApplicationUrl { get; set; } = string.Empty;
    public string Company { get; set; } = "Samcho Corp. Ltd.";
    public string Copyright { get; set; } = "@2025 Copyright";
    public string Version { get; set; } = "1.0.0";
    public string App { get; set; } = "Organizer PRO";
    public string AppName { get; set; } = "Organizer PRO";
}