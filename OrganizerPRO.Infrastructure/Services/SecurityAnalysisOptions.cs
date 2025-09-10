namespace OrganizerPRO.Infrastructure.Services;


public class SecurityAnalysisOptions
{
    public const string SectionName = "SecurityAnalysis";
    public int HistoryDays { get; set; } = 30;
    public int BruteForceWindowMinutes { get; set; } = 10;
    public int AccountBruteForceThreshold { get; set; } = 3;
    public int IpBruteForceThreshold { get; set; } = 10;
    public int IpBruteForceAccountThreshold { get; set; } = 3;
    public int AccountBruteForceScore { get; set; } = 50;
    public int IpBruteForceScore { get; set; } = 50;
    public int NewDeviceLocationScore { get; set; } = 25;
    public int UnusualTimeScore { get; set; } = 15;
    public int UnusualTimeStartHour { get; set; } = 22;
    public int UnusualTimeEndHour { get; set; } = 6;
    public int MediumRiskThreshold { get; set; } = 40;
    public int HighRiskThreshold { get; set; } = 60;
    public int CriticalRiskThreshold { get; set; } = 80;
    public int CacheExpirationMinutes { get; set; } = 15;
}

