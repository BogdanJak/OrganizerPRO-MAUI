namespace OrganizerPRO.Application.Common.Security;

public static partial class Permissions
{
    [DisplayName("Dashboard Permissions")]
    [Description("Set permissions for dashboard operations")]
    public static class Dashboards
    {
        [Description("Allows viewing dashboard details")]
        public const string View = "Permissions.Dashboards.View";
    }
}

public class DashboardsAccessRights
{
    public bool View { get; set; }
} 