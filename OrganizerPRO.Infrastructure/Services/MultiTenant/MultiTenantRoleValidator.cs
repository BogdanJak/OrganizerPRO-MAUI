namespace OrganizerPRO.Infrastructure.Services.MultiTenant;


internal class MultiTenantRoleValidator(ApplicationDbContext context) : RoleValidator<ApplicationRole>
{
    private readonly ApplicationDbContext _context = context;

    public override async Task<IdentityResult> ValidateAsync(RoleManager<ApplicationRole> manager, ApplicationRole role)
    {
        var errors = new List<IdentityError>();
        var duplicateRole = await _context.Roles
            .FirstOrDefaultAsync(r => r.Name == role.Name && r.TenantId == role.TenantId);

        if (duplicateRole != null && duplicateRole.Id != role.Id)
        {
            errors.Add(new IdentityError
            {
                Code = "DuplicateRoleName",
                Description = $"Role name '{role.Name}' already exists in the tenant."
            });
        }

        if (errors.Count > 0)
        {
            return IdentityResult.Failed(errors.ToArray());
        }

        return IdentityResult.Success;
    }
}

