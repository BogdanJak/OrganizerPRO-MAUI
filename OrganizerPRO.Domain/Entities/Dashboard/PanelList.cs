namespace OrganizerPRO.Domain.Entities.Dashboard;


public class PanelList : BaseAuditableEntity, IAuditableEntity
{
    public Guid Id { get; set; }
    public string PermissionType { get; set; }
    public string NamePanel { get; set; }
    public string ComponentTypeText { get; set; }
    public bool? AllowDragging { get; set; }
    public int? Column { get; set; }
    public int? Col { get; set; }
    public int? Row { get; set; }
    public string? CssClass { get; set; }
    public bool? Enabled { get; set; }
    public int? SizeX { get; set; }
    public int? SizeY { get; set; }
    public int? MaxSizeX { get; set; }
    public int? MaxSizeY { get; set; }
    public int? MinSizeX { get; set; }
    public int? MinSizeY { get; set; }
    public int? ZIndex { get; set; }
    public byte[]? ScreenPhoto { get; set; }
}
