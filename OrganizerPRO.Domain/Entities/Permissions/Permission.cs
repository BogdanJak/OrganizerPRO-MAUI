namespace OrganizerPRO.Domain.Entities.Permissions;


public class Permission : BaseAuditableSoftDeleteEntity
{
    public int Id { get; set; }
    public int? ParentElementID { get; set; }
    public Guid TenantId { get; set; }
    public Guid RoleId { get; set; }
    public bool HavePermission { get; set; }
    public bool HaveSubfolders { get; set; }
    public string? Icon { get; set; }
    public string? Href { get; set; }
    public string? Target { get; set; }
    public PageStatus PageStatus { get; set; } = PageStatus.Completed;
    public bool Expand { get; set; }
    public string MenuName { get; set; } = string.Empty;
    public string Text_en_EN { get; set; } = string.Empty;
    public string Text_pl_PL { get; set; } = string.Empty;
    public string Text_de_DE { get; set; } = string.Empty;
    public string Text_fr_FR { get; set; } = string.Empty;
    public string Text_ru_RU { get; set; } = string.Empty;
    public string Text_es_ES { get; set; } = string.Empty;
    public string? UrlAddress { get; set; }
    public bool? View { get; set; }
    public bool? Create { get; set; }
    public bool? Edit { get; set; }
    public bool? Delete { get; set; }
    public bool? Search { get; set; }
    public bool? Export { get; set; }
    public bool? Import { get; set; }
    public bool? ManageRoles { get; set; }
    public bool? RestPassword { get; set; }
    public bool? Active { get; set; }
    public bool? ManagePermissions { get; set; }
    public bool? Empty1 { get; set; }
    public bool? Empty2 { get; set; }
    public bool? Empty3 { get; set; }
}

