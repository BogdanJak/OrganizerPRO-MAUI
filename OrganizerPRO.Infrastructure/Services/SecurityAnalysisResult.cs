namespace OrganizerPRO.Infrastructure.Services;


public class SecurityAnalysisResult
{
    public int RiskScore { get; set; }
    public SecurityRiskLevel RiskLevel { get; set; }
    public List<string> RiskFactors { get; set; } = new();
    public List<string> SecurityAdvice { get; set; } = new();
    public HashSet<string> TriggeredRules { get; set; } = new();
    public Dictionary<string, int> RiskScoreBreakdown { get; set; } = new();
}

public class RiskAnalysisRuleResult
{
    public string RuleName { get; set; } = string.Empty;
    public int Score { get; set; }
    public List<string> Factors { get; set; } = new();
    public bool IsTriggered => Score > 0;
}

