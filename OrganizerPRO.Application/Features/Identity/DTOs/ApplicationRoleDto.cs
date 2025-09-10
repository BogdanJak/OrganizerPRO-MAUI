namespace OrganizerPRO.Application.Features.Identity.DTOs;

[Description("Roles")]
public class ApplicationRoleDto
{
    [Description("Id")] public string Id { get; set; } = Guid.NewGuid().ToString();
    [Description("Name")] public string Name { get; set; } = string.Empty;
    [Description("Tenant Id")] public string? TenantId { get; set; }

    [Description("Normalized Name")] public string? NormalizedName { get; set; }
    [Description("Description")] public string? Description { get; set; }
    [Description("Tenant")]  public TenantDto? Tenant { get; set; }
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ApplicationRole, ApplicationRoleDto>(MemberList.None);
                
        }
    }
}