namespace OrganizerPRO.Application.Features.Identity.DTOs;


public class ApplicationUserDto
{
    public string Id { get; set; } = string.Empty;

    public string UserName { get; set; } = string.Empty;

    public string? DisplayName { get; set; }

    public string? Provider { get; set; } = "Local";

    public string? TenantId { get; set; }

    public TenantDto? Tenant { get; set; }

    public string? ProfilePictureDataUrl { get; set; }

    public string Email { get; set; } = string.Empty;

    public string? PhoneNumber { get; set; }

    public string? SuperiorId { get; set; }

    public ApplicationUserDto? Superior { get; set; }

    public string[]? AssignedRoles { get; set; }

    public string? DefaultRole => AssignedRoles?.FirstOrDefault();

    public bool IsActive { get; set; }

    public bool IsLive { get; set; }

    public string? Password { get; set; }

    public string? ConfirmPassword { get; set; }
    public bool EmailConfirmed { get; set; }

    public DateTimeOffset? LockoutEnd { get; set; }
    public string? TimeZoneId { get; set; }
    public TimeSpan LocalTimeOffset => string.IsNullOrEmpty(TimeZoneId)
    ? TimeZoneInfo.Local.BaseUtcOffset
    : TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId).BaseUtcOffset;
    
    public string? LanguageCode { get; set; }
    
    public DateTime? LastModified { get; set; }
    
    public string? LastModifiedBy { get; set; }
    
    public DateTime? Created { get; set; }
    
    public string? CreatedBy { get; set; }
 
    public UserProfile ToUserProfile()
    {
        return new UserProfile(
            UserId: Id,
            UserName: UserName,
            Email: Email,
            Provider: Provider,
            SuperiorName: Superior?.UserName,
            SuperiorId: SuperiorId,
            ProfilePictureDataUrl: ProfilePictureDataUrl,
            DisplayName: DisplayName,
            PhoneNumber: PhoneNumber,
            DefaultRole: DefaultRole,
            AssignedRoles: AssignedRoles,
            IsActive: IsActive,
            TenantId: TenantId,
            TenantName: Tenant?.Name,
            TimeZoneId: TimeZoneId,
            LanguageCode: LanguageCode
        );
    }

    public bool IsInRole(string role)
    {
        return AssignedRoles?.Contains(role) ?? false;
    }

#pragma warning disable CS8619
#pragma warning disable CS8601
    private class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<ApplicationUser, ApplicationUserDto>(MemberList.None)
                .ForMember(x => x.LocalTimeOffset, s => s.Ignore())
                .ForMember(x => x.EmailConfirmed, s => s.MapFrom(y => y.EmailConfirmed))
                .ForMember(x => x.AssignedRoles, s => s.MapFrom(y => y.UserRoles.Select(r => r.Role.Name)))
                .ForMember(x => x.Superior, s => s.MapFrom(y => y.Superior != null ? new ApplicationUserDto()
                {
                    Id = y.Superior.Id,
                    UserName = y.Superior.UserName,
                    DisplayName = y.Superior.DisplayName,
                    Email = y.Superior.Email,
                    PhoneNumber = y.Superior.PhoneNumber,
                    ProfilePictureDataUrl = y.Superior.ProfilePictureDataUrl,
                    IsActive = y.Superior.IsActive,
                    TenantId = y.Superior.TenantId,
                    AssignedRoles = y.Superior.UserRoles.Select(r => r.Role.Name).ToArray(),
                    TimeZoneId = y.Superior.TimeZoneId,
                    LanguageCode = y.Superior.LanguageCode
                } : null));
                 
                 

        }
    }
}

public class ApplicationUserDtoValidator : AbstractValidator<ApplicationUserDto>
{
    private readonly IStringLocalizer<ApplicationUserDtoValidator> _localizer;

    public ApplicationUserDtoValidator(IStringLocalizer<ApplicationUserDtoValidator> localizer)
    {
        _localizer = localizer;
        RuleFor(v => v.TenantId)
            .MaximumLength(128).WithMessage(_localizer["Tenant id must be less than 128 characters"])
            .NotEmpty().WithMessage(_localizer["Tenant name cannot be empty"]);
        RuleFor(v => v.Provider)
            .MaximumLength(128).WithMessage(_localizer["Provider must be less than 100 characters"])
            .NotEmpty().WithMessage(_localizer["Provider cannot be empty"]);
        RuleFor(x => x.UserName)
            .NotEmpty().WithMessage(_localizer["User name cannot be empty"])
            .Length(2, 100).WithMessage(_localizer["User name must be between 2 and 100 characters"]);
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(_localizer["E-mail cannot be empty"])
            .MaximumLength(100).WithMessage(_localizer["E-mail must be less than 100 characters"])
            .EmailAddress().WithMessage(_localizer["E-mail must be a valid email address"]);

        RuleFor(x => x.DisplayName)
            .MaximumLength(128).WithMessage(_localizer["Full name must be less than 128 characters"]);

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage(_localizer["Phone number must be less than 20 digits"]);
   
    }
}