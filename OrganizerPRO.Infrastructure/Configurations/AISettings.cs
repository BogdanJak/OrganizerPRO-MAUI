namespace OrganizerPRO.Infrastructure.Configurations;


public class AISettings : IAISettings
{
    public const string Key = "AI";
    public string GeminiApiKey { get; set; } = string.Empty;
} 