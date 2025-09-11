namespace OrganizerPRO.Infrastructure.Persistence;


public class ApplicationDbContextInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationDbContextInitializer> _logger;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly UserManager<ApplicationUser> _userManager;

    private List<string> permissions { get; set; } = new();
    private List<Permission> perm { get; set; } = new();


    public ApplicationDbContextInitializer(ILogger<ApplicationDbContextInitializer> logger,
        IDbContextFactory<ApplicationDbContext> dbContextFactory,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _logger = logger;
        _context = dbContextFactory.CreateDbContext();
        _userManager = userManager;
        _roleManager = roleManager;
    }

    public async Task InitialiseAsync()
    {
        try
        {
            if (_context.Database.IsRelational())
                await _context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initialising the database");
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await SeedTenantsAsync();
            await SeedRolesAsync();
            await SeedUsersAsync();
            await SeedDataAsync();
            _context.ChangeTracker.Clear();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while seeding the database");
            throw;
        }
    }

    private async Task SeedTenantsAsync()
    {
        if (await _context.Tenants.AnyAsync()) return;

        _logger.LogInformation("Seeding organizations...");
        var tenants = new[]
        {
                new Tenant { Name = "Master", Description = "Master Site" },
            };

        await _context.Tenants.AddRangeAsync(tenants);
        await _context.SaveChangesAsync();
    }

    private async Task SeedRolesAsync()
    {
        if (await _roleManager.RoleExistsAsync(Roles.SuperAdministrator)) return;
        _logger.LogInformation("Seeding roles...");

        var superadmin = new ApplicationRole(Roles.SuperAdministrator)
        {
            Description = "Grupa Administratorów",
            TenantId = (await _context.Tenants.FirstAsync()).Id,
        };

        await _roleManager.CreateAsync(superadmin);

        var firstrole = await _roleManager.FindByNameAsync(Roles.SuperAdministrator).ConfigureAwait(false);
        Guid firstroleid = Guid.Parse(firstrole.Id);

        if (!_context.Permissions.Any())
        {
            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = false,
                Icon = "home",
                Href = "/",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = false,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                MenuName = "Home",
                UrlAddress = "/"
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = true,
                Icon = "apps",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = false,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                MenuName = "Application",
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = true,
                Icon = "manage_accounts",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = false,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                MenuName = "System_Management",
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = true,
                Icon = "lock_open",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = false,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 3,
                MenuName = "Authorization",
            });

            await _context.SaveChangesAsync();


            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = true,
                Icon = "tune",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = false,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 3,
                MenuName = "System",
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = false,
                Icon = "group",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = true,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 4,
                MenuName = "Users_Management",
                UrlAddress = "usersmanager",
                View = true,
                Create = true,
                Edit = true,
                Delete = true,
                Search = true,
                Export = true,
                Import = true,
                ManageRoles = true,
                ManagePermissions = true,
                RestPassword = true,
                Active = true,
                Empty1 = false,
                Empty2 = false,
                Empty3 = false
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = false,
                Icon = "user_attributes",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = true,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 4,
                MenuName = "User_Profile",
                UrlAddress = "userprofilemanager",
                View = true,
                Create = true,
                Edit = true,
                Delete = true,
                Search = true,
                Export = true,
                Import = true,
                ManageRoles = true,
                ManagePermissions = true,
                RestPassword = true,
                Active = true,
                Empty1 = false,
                Empty2 = false,
                Empty3 = false
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = false,
                Icon = "passkey",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = true,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                RoleId = firstroleid,
                ParentElementID = 4,
                MenuName = "Role_Management",
                UrlAddress = "rolesmanager",
                View = true,
                Create = true,
                Edit = true,
                Delete = true,
                Search = true,
                Export = true,
                Import = true,
                ManageRoles = true,
                ManagePermissions = true,
                RestPassword = true,
                Active = true,
                Empty1 = false,
                Empty2 = false,
                Empty3 = false
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = false,
                Icon = "tenancy",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = true,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 4,
                MenuName = "Tenant_Management",
                UrlAddress = "tenantsmanager",
                View = true,
                Create = true,
                Edit = true,
                Delete = true,
                Search = true,
                Export = true,
                Import = true,
                ManageRoles = true,
                ManagePermissions = true,
                RestPassword = true,
                Active = true,
                Empty1 = false,
                Empty2 = false,
                Empty3 = false
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = false,
                Icon = "account_balance",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = true,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 4,
                MenuName = "Permission_Management",
                UrlAddress = "permissionmanager",
                View = true,
                Create = true,
                Edit = true,
                Delete = true,
                Search = true,
                Export = true,
                Import = true,
                ManageRoles = true,
                ManagePermissions = true,
                RestPassword = true,
                Active = true,
                Empty1 = false,
                Empty2 = false,
                Empty3 = false
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = false,
                Icon = "language",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = true,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 4,
                MenuName = "Localization_Management",
                UrlAddress = "localizersmanager",
                View = true,
                Create = true,
                Edit = true,
                Delete = true,
                Search = true,
                Export = true,
                Import = true,
                ManageRoles = true,
                ManagePermissions = true,
                RestPassword = true,
                Active = true,
                Empty1 = false,
                Empty2 = false,
                Empty3 = false
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = false,
                Icon = "table_eye",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = true,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 5,
                MenuName = "Audit_Trail",
                UrlAddress = "/Pages/System_Pages/AuditTrails/AuditTrails",
                View = true,
                Create = true,
                Edit = true,
                Delete = true,
                Search = true,
                Export = true,
                Import = true,
                ManageRoles = true,
                ManagePermissions = true,
                RestPassword = true,
                Active = true,
                Empty1 = false,
                Empty2 = false,
                Empty3 = false
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = false,
                Icon = "table_edit",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = true,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 5,
                MenuName = "Loggs",
                UrlAddress = "/Pages/System_Pages/Logs/Logs",
                View = true,
                Create = true,
                Edit = true,
                Delete = true,
                Search = true,
                Export = true,
                Import = true,
                ManageRoles = true,
                ManagePermissions = true,
                RestPassword = true,
                Active = true,
                Empty1 = false,
                Empty2 = false,
                Empty3 = false
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = false,
                Icon = "dashboard",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = true,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 5,
                MenuName = "Dasboard_Panels",
                UrlAddress = "/Pages/System_Pages/DashboardPanel/DashboardPanel",
                View = true,
                Create = true,
                Edit = true,
                Delete = true,
                Search = true,
                Export = true,
                Import = true,
                ManageRoles = true,
                ManagePermissions = true,
                RestPassword = true,
                Active = true,
                Empty1 = false,
                Empty2 = false,
                Empty3 = false
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = false,
                Icon = "database",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = true,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 5,
                MenuName = "Database_Manager",
                UrlAddress = "/Pages/System_Pages/DataBaseManagers/DataBaseManager",
                View = true,
                Create = true,
                Edit = true,
                Delete = true,
                Search = true,
                Export = true,
                Import = true,
                ManageRoles = true,
                ManagePermissions = true,
                RestPassword = true,
                Active = true,
                Empty1 = false,
                Empty2 = false,
                Empty3 = false
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = false,
                Icon = "settings_account_box",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = true,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 5,
                MenuName = "App_Setting",
                UrlAddress = "/Pages/System_Pages/AppSettings/AppSettingManager",
                View = true,
                Create = true,
                Edit = true,
                Delete = true,
                Search = true,
                Export = true,
                Import = true,
                ManageRoles = true,
                ManagePermissions = true,
                RestPassword = true,
                Active = true,
                Empty1 = false,
                Empty2 = false,
                Empty3 = false
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = false,
                Icon = "corporate_fare",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = true,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 4,
                MenuName = "Organization_Management",
                UrlAddress = "/Pages/System_Pages/Permission/OrganizationManager",
                View = true,
                Create = true,
                Edit = true,
                Delete = true,
                Search = true,
                Export = true,
                Import = true,
                ManageRoles = true,
                ManagePermissions = true,
                RestPassword = true,
                Active = true,
                Empty1 = false,
                Empty2 = false,
                Empty3 = false
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = true,
                Icon = "local_post_office",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = false,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 2,
                MenuName = "Emails",
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = true,
                Icon = "pending_actions",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = false,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 2,
                MenuName = "Calendars",
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = true,
                Icon = "contacts",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = false,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 2,
                MenuName = "Contacts",
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = true,
                Icon = "youtube_activity",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = false,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 2,
                MenuName = "Youtube",
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = true,
                Icon = "radio",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = false,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 2,
                MenuName = "RadioNET",
            });

            await _context.SaveChangesAsync();

            _context.Permissions.Add(new Domain.Entities.Permissions.Permission()
            {
                HaveSubfolders = true,
                Icon = "docs",
                PageStatus = PageStatus.Completed,
                Expand = true,
                HavePermission = false,
                RoleId = firstroleid,
                TenantId = Guid.Parse((await _context.Tenants.FirstAsync()).Id),
                ParentElementID = 2,
                MenuName = "Document_Manager",
            });

            await _context.SaveChangesAsync();


            perm = _context.Permissions.ToList();

            foreach (var permission in perm)
            {
                if (permission.View == true)
                {
                    permissions.Add("Permissions." + permission.MenuName + ".View");
                }

                if (permission.Create == true)
                {
                    permissions.Add("Permissions." + permission.MenuName + ".Create");
                }

                if (permission.Edit == true)
                {
                    permissions.Add("Permissions." + permission.MenuName + ".Edit");
                }

                if (permission.Delete == true)
                {
                    permissions.Add("Permissions." + permission.MenuName + ".Delete");
                }

                if (permission.Search == true)
                {
                    permissions.Add("Permissions." + permission.MenuName + ".Search");
                }

                if (permission.Export == true)
                {
                    permissions.Add("Permissions." + permission.MenuName + ".Export");
                }

                if (permission.Import == true)
                {
                    permissions.Add("Permissions." + permission.MenuName + ".Import");
                }

                if (permission.ManageRoles == true)
                {
                    permissions.Add("Permissions." + permission.MenuName + ".ManageRoles");
                }

                if (permission.RestPassword == true)
                {
                    permissions.Add("Permissions." + permission.MenuName + ".RestPassword");
                }

                if (permission.Active == true)
                {
                    permissions.Add("Permissions." + permission.MenuName + ".Active");
                }

                if (permission.ManagePermissions == true)
                {
                    permissions.Add("Permissions." + permission.MenuName + ".ManagePermissions");
                }

                if (permission.Empty1 == true)
                {
                    permissions.Add("Permissions." + permission.MenuName + ".Empty1");
                }

                if (permission.Empty2 == true)
                {
                    permissions.Add("Permissions." + permission.MenuName + ".Empty2");
                }

                if (permission.Empty3 == true)
                {
                    permissions.Add("Permissions." + permission.MenuName + ".Empty3");
                }
            }

            foreach (var permission in permissions)
            {
                var claim = new Claim(ApplicationClaimTypes.Permission, permission);
                await _roleManager.AddClaimAsync(superadmin, claim);
            }
        }
    }

    private async Task SeedUsersAsync()
    {
        if (await _userManager.Users.AnyAsync()) return;

        _logger.LogInformation("Seeding users...");

        var adminUser = new ApplicationUser
        {
            UserName = Users.SuperAdministrator,
            Provider = "Local",
            IsActive = true,
            TenantId = (await _context.Tenants.FirstAsync()).Id,
            TenantName = (await _context.Tenants.FirstAsync()).Name,
            DisplayName = Users.SuperAdministrator,
            Email = "bogdan.jak.66@gmail.com",
            EmailConfirmed = true,
            LanguageCode = "pl-PL",
            TimeZoneId = "Central European Standard Time",
            TwoFactorEnabled = false,
        };

        await _userManager.CreateAsync(adminUser, Users.DefaultPassword);
        await _userManager.AddToRoleAsync(adminUser, Roles.SuperAdministrator);

    }


    private async Task SeedDataAsync()
    {
        if (!_context.PanelLists.Any())
        {
            _context.PanelLists.Add(new PanelList()
            {
                NamePanel = "Weathers Panel Admin",
                ComponentTypeText = "WeathersCommponent",
                AllowDragging = true,
                Column = 1,
                Row = 1,
                SizeX = 1,
                SizeY = 1,
                PermissionType = "Administrators"
            });

            _context.PanelLists.Add(new PanelList()
            {
                NamePanel = "Weathers Panel Users",
                ComponentTypeText = "WeathersCommponent",
                AllowDragging = true,
                Column = 1,
                Row = 1,
                SizeX = 1,
                SizeY = 1,
                PermissionType = "Users"
            });

            _context.PanelLists.Add(new PanelList()
            {
                NamePanel = "Users Panel",
                ComponentTypeText = "UsersCommponent",
                AllowDragging = true,
                Column = 1,
                Row = 1,
                SizeX = 1,
                SizeY = 1,
                PermissionType = "Administrators"
            });

            _context.PanelLists.Add(new PanelList()
            {
                NamePanel = "Roles Panel",
                ComponentTypeText = "RolesCommponent",
                AllowDragging = true,
                Column = 1,
                Row = 1,
                SizeX = 1,
                SizeY = 1,
                PermissionType = "Administrators"
            });

            _context.PanelLists.Add(new PanelList()
            {
                NamePanel = "Tenants Panel",
                ComponentTypeText = "TenantsCommponent",
                AllowDragging = true,
                Column = 1,
                Row = 1,
                SizeX = 1,
                SizeY = 1,
                PermissionType = "Administrators"
            });

            _context.PanelLists.Add(new PanelList()
            {
                NamePanel = "Email Panel",
                ComponentTypeText = "EmailCommponent",
                AllowDragging = true,
                Column = 1,
                Row = 1,
                SizeX = 1,
                SizeY = 1,
                PermissionType = "Users"
            });

            _context.PanelLists.Add(new PanelList()
            {
                NamePanel = "EmailUnRead Panel",
                ComponentTypeText = "EmailUnReadCommponent",
                AllowDragging = true,
                Column = 1,
                Row = 1,
                SizeX = 1,
                SizeY = 1,
                PermissionType = "Users"
            });

            _context.PanelLists.Add(new PanelList()
            {
                NamePanel = "Scheduler Panel Users",
                ComponentTypeText = "SchedulerCommponent",
                AllowDragging = true,
                Column = 1,
                Row = 1,
                SizeX = 1,
                SizeY = 1,
                PermissionType = "Users"
            });

            _context.PanelLists.Add(new PanelList()
            {
                NamePanel = "Scheduler Panel Admin",
                ComponentTypeText = "SchedulerCommponent",
                AllowDragging = true,
                Column = 1,
                Row = 1,
                SizeX = 1,
                SizeY = 1,
                PermissionType = "Administrators"
            });

            await _context.SaveChangesAsync();
        }


        if (!_context.Localizers.Any())
        {
            _logger.LogInformation("Seeding data...");

            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_By_Calendar", EnEN = "By Calendar", PlPL = "Po kalendarzu", DeDE = "Nach Kalender", FrFR = "Par calendrier", RuRU = "По календарю", EsES = "Por calendario" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_By_Date", EnEN = "By Date", PlPL = "Po dacie", DeDE = "Nach Datum", FrFR = "Par date", RuRU = "По дате", EsES = "Por fecha" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Day_End_Hour", EnEN = "Day end hour", PlPL = "Godzina zakończenia dnia", DeDE = "Tagesendstunde", FrFR = "Heure de fin de journée", RuRU = "Конец дня, час", EsES = "Hora de fin del día" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Day_Start_Hour", EnEN = "Day Start Hour", PlPL = "Godzina rozpoczęcia dnia", DeDE = "Tagesstartstunde", FrFR = "Heure de début de journée", RuRU = "Начало дня, час", EsES = "Hora de inicio del día" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_First_Day", EnEN = "First Day of Week", PlPL = "Pierwszy dzień tygodnia", DeDE = "Erster Tag der Woche", FrFR = "Premier jour de la semaine", RuRU = "Первый день недели", EsES = "Primer dia de la semana" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Fit_Row", EnEN = "Auto fit rows", PlPL = "Automatyczne dopasowanie wierszy", DeDE = "Zeilen automatisch anpassen", FrFR = "Ajustement automatique des lignes", RuRU = "Автоподбор строк", EsES = "Filas de ajuste automático" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Name", EnEN = "Calendar Name", PlPL = "Nazwa kalendarza", DeDE = "Kalendername", FrFR = "Nom du calendrier", RuRU = "Название календаря", EsES = "Nombre del calendario" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Priority", EnEN = "Priority", PlPL = "Priorytet", DeDE = "Priorität", FrFR = "Priorité", RuRU = "Приоритет", EsES = "Prioridad" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Priority_High", EnEN = "Priority HIGH - color", PlPL = "Priorytet WYSOKI - kolor", DeDE = "Priorität HOCH – Farbe", FrFR = "Priorité ÉLEVÉE - couleur", RuRU = "Приоритет ВЫСОКИЙ – цвет", EsES = "Prioridad ALTA - color" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Priority_Low", EnEN = "Priority LOW - color", PlPL = "Priorytet NISKI - kolor", DeDE = "Priorität NIEDRIG – Farbe", FrFR = "Priorité FAIBLE - couleur", RuRU = "НИЗКИЙ приоритет – цвет", EsES = "Prioridad BAJA - color" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Priority_Normal", EnEN = "Priority NORMAL - color", PlPL = "Priorytet NORMALNY - kolor", DeDE = "Priorität NORMAL – Farbe", FrFR = "Priorité NORMAL - couleur", RuRU = "Приоритет НОРМАЛЬНЫЙ – цвет", EsES = "Prioridad NORMAL - color" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Resources", EnEN = "Activ calendar", PlPL = "Aktywny kalendarz", DeDE = "Aktiver Kalender", FrFR = "Calendrier actif", RuRU = "Активный календарь", EsES = "Calendario de actividades" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Settings", EnEN = "Settings", PlPL = "Ustawienia", DeDE = "Einstellungen", FrFR = "Paramètres", RuRU = "Настройки", EsES = "Ajustes" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Settingss", EnEN = "Your calendar settings", PlPL = "Ustawienia Twojego kalendarza", DeDE = "Ihre Kalendereinstellungen", FrFR = "Paramètres de votre calendrier", RuRU = "Настройки вашего календаря", EsES = "La configuración de tu calendario" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Slot_Duration", EnEN = "Slot Duration", PlPL = "Jednostka czasu", DeDE = "Slot-Dauer", FrFR = "Durée du créneau", RuRU = "Продолжительность слота", EsES = "Duración del espacio" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Slot_Interval", EnEN = "Slot interval", PlPL = "Podział jednostki czasu", DeDE = "Slot-Intervall", FrFR = "Intervalle de créneau", RuRU = "Интервал слота", EsES = "Intervalo de ranura" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Status", EnEN = "Status", PlPL = "Status", DeDE = "Status", FrFR = "Statut", RuRU = "Положение дел", EsES = "Estado" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Time_Format", EnEN = "Time format", PlPL = "Format czasu", DeDE = "Zeitformat", FrFR = "Format de l'heure", RuRU = "Формат времени", EsES = "Formato de tiempo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Time_Slots", EnEN = "Time slots", PlPL = "Przedział czasu", DeDE = "Zeitfenster", FrFR = "Tranches de temps", RuRU = "Временные интервалы", EsES = "Ranuras de tiempo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Timeline_View", EnEN = "Timeline View", PlPL = "Linie czasu", DeDE = "Zeitleistenansicht", FrFR = "Vue chronologique", RuRU = "Просмотр временной шкалы", EsES = "Vista de línea de tiempo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Week_Number", EnEN = "Week number", PlPL = "Numer tygodnia", DeDE = "Wochennummer", FrFR = "Numéro de semaine", RuRU = "Номер недели", EsES = "Número de la semana" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Work_Days", EnEN = "Work days", PlPL = "Dni robocze", DeDE = "Arbeitstage", FrFR = "Jours de travail", RuRU = "Рабочие дни", EsES = "Días laborables" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Work_End_Hour", EnEN = "Work end hour", PlPL = "Godzina zakończenia pracy", DeDE = "Arbeitsende Stunde", FrFR = "Heure de fin de travail", RuRU = "Час окончания работы", EsES = "Hora de fin de trabajo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendar_Work_Start_Hour", EnEN = "Work start hour", PlPL = "Godzina rozpoczęcia pracy", DeDE = "Arbeitsbeginnstunde", FrFR = "Heure de début de travail", RuRU = "Час начала работы", EsES = "Hora de inicio del trabajo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Add_More", EnEN = "Add more", PlPL = "Dodaj więcej", DeDE = "Fügen Sie weitere hinzu", FrFR = "Ajouter plus", RuRU = "Добавить больше", EsES = "Añadir más" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Add_More+", EnEN = "+ Add More", PlPL = "+  Dodaj więcej", DeDE = "+  Weitere hinzufügen", FrFR = "+  Ajouter plus", RuRU = "+  Добавить еще", EsES = "+  Agregar más" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Add_new_contact", EnEN = "Add new contact", PlPL = "Dodaj nowy kontakt", DeDE = "Neuer Kontakt", FrFR = "Nouveau contact", RuRU = "Новый контакт", EsES = "Añadir nuevo contacto" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Additional_Address", EnEN = "Additional address", PlPL = "Dodatkowy adres", DeDE = "Zusätzliche Adresse", FrFR = "Adresse supplémentaire", RuRU = "Дополнительный адрес", EsES = "Dirección adicional" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Additional_email_address", EnEN = "Additional email address", PlPL = "Dodatkowy adres email", DeDE = "Zusätzliche E-Mail-Adresse", FrFR = "Adresse e-mail supplémentaire", RuRU = "Дополнительный адрес электронной почты", EsES = "Dirección de correo electrónico adicional" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Additional_Email_Address", EnEN = "Additional email address", PlPL = "Dodatkowy adres email", DeDE = "Zusätzliche E-Mail-Adresse", FrFR = "Adresse e-mail supplémentaire", RuRU = "Дополнительный адрес электронной почты", EsES = "Dirección de correo electrónico adicional" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Additional_Information", EnEN = "Additional information", PlPL = "Dodatkowe informacje", DeDE = "Weitere Informationen", FrFR = "Informations Complémentaires", RuRU = "Дополнительная информация", EsES = "Información adicional" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Chat_Address", EnEN = "Chat address", PlPL = "Adres czatu", DeDE = "Chat-Adresse", FrFR = "Adresse de chat", RuRU = "Адрес чата", EsES = "dirección de chat" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_City", EnEN = "City", PlPL = "Miejscowość", DeDE = "Stadt", FrFR = "Ville", RuRU = "Город", EsES = "Ciudad" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Commentary", EnEN = "Contact commentary", PlPL = "Komentarz do kontaktu", DeDE = "Kontaktkommentar", FrFR = "Contacter le commentaire", RuRU = "Контактный комментарий", EsES = "Comentario de contacto" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Company_Address", EnEN = "Company address", PlPL = "Adres firmowy", DeDE = "Firmenanschrift", FrFR = "Adresse de la société", RuRU = "Адрес компании", EsES = "Dirección de la empresa" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Company_Name", EnEN = "Company name", PlPL = "Nazwa firmy", DeDE = "Name der Firma", FrFR = "Nom de l'entreprise", RuRU = "Название компании", EsES = "Nombre de empresa" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Contact", EnEN = "Contact", PlPL = "Kontakt", DeDE = "Kontakt", FrFR = "Contact", RuRU = "Контакт", EsES = "Contacto" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Contact_Information", EnEN = "Contact information", PlPL = "Kontakt informacje", DeDE = "Kontaktinformationen", FrFR = "Coordonnées", RuRU = "Контактная информация", EsES = "Información del contacto" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Country", EnEN = "Country", PlPL = "Państwo", DeDE = "Land", FrFR = "Pays", RuRU = "Страна", EsES = "País" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Date_Of_Birth", EnEN = "Date of birth", PlPL = "Data urodzin", DeDE = "Geburtsdatum", FrFR = "Date de naissance", RuRU = "Дата рождения", EsES = "Fecha de nacimiento" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Day", EnEN = "Day", PlPL = "Dzień", DeDE = "Tag", FrFR = "Jour", RuRU = "День", EsES = "Día" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Delete_contact", EnEN = "Delete contact", PlPL = "Usuń kontakt", DeDE = "Kontakt löschen", FrFR = "Supprimer le contact", RuRU = "Удалить контакт", EsES = "Borrar contacto" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Designation", EnEN = "Job position", PlPL = "Stanowisko służbowe", DeDE = "Arbeitsposition", FrFR = "Poste", RuRU = "Должность", EsES = "Puesto de trabajo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Edit_contact", EnEN = "Edit contact", PlPL = "Edytuj kontakt", DeDE = "Kontakt bearbeiten", FrFR = "Modifier le contact", RuRU = "Изменить контакт", EsES = "Editar contacto" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Edit_Contact_Details", EnEN = "Edit contact details", PlPL = "Edytuj dane kontaktu", DeDE = "Kontaktdaten bearbeiten", FrFR = "Modifier les coordonnées", RuRU = "Изменить контактную информацию", EsES = "Editar detalles de contacto" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_E-mail_Messages", EnEN = "E-mail messages", PlPL = "Wiadomości email", DeDE = "E-Mail-Nachrichten", FrFR = "Messages électroniques", RuRU = "Сообщения электронной почты", EsES = "Mensajes de correo electrónico" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Files", EnEN = "Files", PlPL = "Pliki", DeDE = "Dateien", FrFR = "Des dossiers", RuRU = "Файлы", EsES = "Archivos" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Filterer", EnEN = "Contact filterer", PlPL = "Filtruj kontakty", DeDE = "Kontaktfilter", FrFR = "Filtre de contact", RuRU = "Контактный фильтр", EsES = "Filtro de contactos" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Home_Address", EnEN = "Home address", PlPL = "Adres domowy", DeDE = "Heimatadresse", FrFR = "Adresse du domicile", RuRU = "домашний адрес", EsES = "Direccion de casa" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Home_Phone", EnEN = "Home phone", PlPL = "Telefon domowy", DeDE = "Festnetztelefon", FrFR = "Téléphone fixe", RuRU = "Домашний телефон", EsES = "Teléfono de casa" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Last_Files", EnEN = "User's recent email files and attachments :", PlPL = "Ostatnie pliki e-mail użytkownika i załączniki :", DeDE = "Aktuelle E-Mail-Dateien und Anhänge des Benutzers :", FrFR = "Fichiers et pièces jointes des e-mails récents de l'utilisateur :", RuRU = "Недавние файлы электронной почты и вложения пользователя :", EsES = "Archivos adjuntos y archivos de correo electrónico recientes del usuario :" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Last_Name", EnEN = "Last name", PlPL = "Nazwisko", DeDE = "Nachname", FrFR = "Nom de famille", RuRU = "Фамилия", EsES = "Apellido" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Manager_contact", EnEN = "Manage contacts", PlPL = "Zarządzaj kontaktami", DeDE = "Kontakte verwalten", FrFR = "Gérer les contacts", RuRU = "Управление контактами", EsES = "Gestionar contactos" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Mobile_Number", EnEN = "Mobile number", PlPL = "Telefon komórkowy", DeDE = "Handy Nummer", FrFR = "Numéro de portable", RuRU = "Номер мобильного телефона", EsES = "Número de teléfono móvil" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Month", EnEN = "Month", PlPL = "Miesiąc", DeDE = "Monat", FrFR = "Mois", RuRU = "Месяц", EsES = "Mes" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Name", EnEN = "Name", PlPL = "Imię", DeDE = "Name", FrFR = "Nom", RuRU = "Имя", EsES = "Nombre" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Nick", EnEN = "Nick", PlPL = "Nick", DeDE = "Nick", FrFR = "Pseudo", RuRU = "Ник", EsES = "Mella" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Notes", EnEN = "Notes", PlPL = "Notatki", DeDE = "Notizbuch", FrFR = "Carnet de notes", RuRU = "Блокнот", EsES = "Computadora portátil" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Organization", EnEN = "Organization", PlPL = "Organizacja", DeDE = "Organisation", FrFR = "Organisation", RuRU = "Oрганизация", EsES = "Organización" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Overview", EnEN = "Overview", PlPL = "Przegląd", DeDE = "Überblick", FrFR = "Aperçu", RuRU = "Обзор", EsES = "Descripción general" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Partner", EnEN = "Partner", PlPL = "Partner", DeDE = "Partner", FrFR = "Partenaire", RuRU = "Партнер", EsES = "Pareja" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Personal_Website", EnEN = "Personal website", PlPL = "Osobista strona WWW", DeDE = "Persönliche Webseite", FrFR = "Site Web personnel", RuRU = "персональный сайт", EsES = "Sitio web personal" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Primary_email_address", EnEN = "Primary email address", PlPL = "Podstawowy adres email", DeDE = "Haupt-Email-Adresse", FrFR = "Adresse e-mail principale", RuRU = "Основной адрес электронной почты", EsES = "Dirección de correo principal" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Region", EnEN = "Region", PlPL = "Województwo", DeDE = "Region", FrFR = "Région", RuRU = "Область", EsES = "Región" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Salutation", EnEN = "Salutation", PlPL = "Zwrot grzecznościowy", DeDE = "Gruß", FrFR = "Saluer", RuRU = "Отдать честь", EsES = "Saludo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Second_Name", EnEN = "Second name", PlPL = "Drugie imię", DeDE = "Zweitname", FrFR = "Nom de famille", RuRU = "Второе имя", EsES = "Segundo nombre" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Send_Email", EnEN = "Send email", PlPL = "Wyślij email", DeDE = "E-Mail senden", FrFR = "Envoyer un e-mail", RuRU = "Отправить письмо", EsES = "Enviar correo electrónico" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Street", EnEN = "Street", PlPL = "Ulica", DeDE = "Straße", FrFR = "rue", RuRU = "улица", EsES = "calle" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Suffix", EnEN = "Suffix", PlPL = "Sufiks", DeDE = "Suffix", FrFR = "Suffixe", RuRU = "суффикс", EsES = "Sufijo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Title", EnEN = "Title", PlPL = "Stanowisko", DeDE = "Titel", FrFR = "Titre", RuRU = "Заголовок", EsES = "Título" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Wedding_Anniversary", EnEN = "Wedding Anniversary", PlPL = "Rocznica ślubu", DeDE = "Hochzeitstag", FrFR = "Anniversaire de mariage", RuRU = "Годовщина свадьбы", EsES = "Aniversario de bodas" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Work", EnEN = "Work", PlPL = "Praca", DeDE = "Arbeiten", FrFR = "Travail", RuRU = "Работа", EsES = "Trabajar" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Work_Phone", EnEN = "Work phone", PlPL = "Telefon firmowy", DeDE = "Arbeitshandy", FrFR = "Téléphone de travail", RuRU = "Рабочий телефон", EsES = "Teléfono del trabajo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Year", EnEN = "Year", PlPL = "Rok", DeDE = "Jahr", FrFR = "Année", RuRU = "Год", EsES = "Año" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Zadzwon", EnEN = "Call me", PlPL = "Zadzwoń", DeDE = "Ruf mich an", FrFR = "Appelez-moi", RuRU = "Позвони мне", EsES = "Llámame" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contact_Zip_Code", EnEN = "Zip code", PlPL = "Kod pocztowy", DeDE = "PLZ", FrFR = "Code postal", RuRU = "Почтовый индекс", EsES = "Código postal" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Document_Delete_Document", EnEN = "Delete document", PlPL = "Usuń dokument", DeDE = "Dokument löschen", FrFR = "Supprimer le document", RuRU = "Удалить документ", EsES = "Eliminar documento" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Document_Edit_Document", EnEN = "Edit document", PlPL = "Edytuj dokument", DeDE = "Dokument bearbeiten", FrFR = "Modifier le document", RuRU = "Редактировать документ", EsES = "Editar documento" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Document_New_Document", EnEN = "Add new document", PlPL = "Dodaj nowy dokument", DeDE = "Neues Dokument hinzufügen", FrFR = "Ajouter un nouveau document", RuRU = "Добавить новый документ", EsES = "Agregar nuevo documento" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Ad_New_Folder", EnEN = "Add new folder", PlPL = "Dodaj nowy folder", DeDE = "Fügen Sie einen neuen Ordner hinzu", FrFR = "Ajouter un nouveau dossier", RuRU = "Добавить новую папку", EsES = "Agregar nueva carpeta" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_All", EnEN = "All Messages", PlPL = "Wszystkie", DeDE = "Alle Nachrichten", FrFR = "Tous les messages", RuRU = "Все сообщения", EsES = "Todos los mensajes" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Archive", EnEN = "Archive", PlPL = "Archiwizuj", DeDE = "Archiv", FrFR = "Archive", RuRU = "Архив", EsES = "Archivo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Block", EnEN = "Block the sender", PlPL = "Zablokuj nadawcę", DeDE = "Blockieren Sie den Absender", FrFR = "Bloquer l'expéditeur", RuRU = "Заблокировать отправителя", EsES = "Bloquear al remitente" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Category_Email", EnEN = "Category", PlPL = "Kategoria", DeDE = "Kategorie", FrFR = "Catégorie", RuRU = "Категория", EsES = "Categoría" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Clear", EnEN = "Clear", PlPL = "Wyczyść", DeDE = "Klar", FrFR = "Dégager", RuRU = "чистый", EsES = "Claro" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Copy_Email", EnEN = "Copy", PlPL = "Kopiuj", DeDE = "Kopieren", FrFR = "Copie", RuRU = "Копировать", EsES = "Copiar" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Delete_Email", EnEN = "Delete", PlPL = "Usuń", DeDE = "Löschen", FrFR = "Effacer", RuRU = "Удалить", EsES = "Borrar" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Delete_Folder", EnEN = "Delete folder", PlPL = "Usuń folder", DeDE = "Löschen Sie den Ordner", FrFR = "Supprimer le dossier", RuRU = "Удалить папку", EsES = "Eliminar carpeta" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Down_Folder", EnEN = "Move Down", PlPL = "Przenieś w dół", DeDE = "Sich abwärts bewegen", FrFR = "Descendre", RuRU = "Двигаться вниз", EsES = "Mover hacia abajo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Empty_Folder", EnEN = "Empty Folder", PlPL = "Opróżnij folder", DeDE = "Leeren Sie den Ordner", FrFR = "Vider le dossier", RuRU = "Очистить папку", EsES = "Carpeta vacía" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Filter", EnEN = "Filter", PlPL = "Filtr", DeDE = "Filter", FrFR = "Filtre", RuRU = "Фильтр", EsES = "Filtrar" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Flag_Email", EnEN = "Flag", PlPL = "Flaga", DeDE = "Flagge", FrFR = "Drapeau", RuRU = "Флаг", EsES = "Bandera" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Folder_Properties", EnEN = "Folder Properties", PlPL = "Właściwości folderu", DeDE = "Ordnereigenschaften", FrFR = "Propriétés du dossier", RuRU = "Свойства папки", EsES = "Propiedades de carpeta" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Forward", EnEN = "Forward", PlPL = "Prześlij", DeDE = "Nach vorne", FrFR = "Avant", RuRU = "Вперед", EsES = "Adelante" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Local_Delete", EnEN = "Delete Local", PlPL = "Usuń lokalnie", DeDE = "Lokal löschen", FrFR = "Supprimer le local", RuRU = "Удалить локальный", EsES = "Eliminar local" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Mark", EnEN = "Mark", PlPL = "Zaznacz", DeDE = "Markieren", FrFR = "Marquer", RuRU = "Отметка", EsES = "Marca" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Mark_All_Read", EnEN = "Mark all read", PlPL = "znacz wszystkie jako przeczytane", DeDE = "Markiere alles Gelesene", FrFR = "Marquer tout comme lu", RuRU = "Отметить все как прочитанные", EsES = "Marcar todo como leido" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Move_Email", EnEN = "Move", PlPL = "Przenieś", DeDE = "Bewegen", FrFR = "Déplacer", RuRU = "Шаг", EsES = "Mover" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Move_To_Archive", EnEN = "Move to Archive", PlPL = "Przenieś do archiwum", DeDE = "Ins Archiv verschieben", FrFR = "Déplacer vers l'archive", RuRU = "Переместить в архив", EsES = "Mover al archivo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_NewEmail", EnEN = "New message", PlPL = "Nowa wiadomość", DeDE = "Neue Nachricht", FrFR = "Nouveau message", RuRU = "Новое сообщение", EsES = "Nuevo mensaje" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Open_In_New_Window", EnEN = "Open in new window", PlPL = "Otwórz w nowym oknie", DeDE = "In einem neuen Fenster öffnen", FrFR = "Ouvrir dans une nouvelle fenêtre", RuRU = "Открыть в новом окне", EsES = "Abrir en Nueva ventana" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Other", EnEN = "Other", PlPL = "Inne", DeDE = "Sonstiges", FrFR = "Autre", RuRU = "Другой", EsES = "Otro" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Print_Email", EnEN = "Print", PlPL = "Drukuj", DeDE = "Drucken", FrFR = "Imprimer", RuRU = "Распечатать", EsES = "Imprimir" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Priority", EnEN = "Priority", PlPL = "Priorytetowe", DeDE = "Priorität", FrFR = "Priorité", RuRU = "приоритет", EsES = "Prioridad" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Put_It_Down", EnEN = "Put It Down", PlPL = "Odłóż", DeDE = "Leg es runter", FrFR = "Pose-le", RuRU = "Положи", EsES = "Bajalo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Read_Email", EnEN = "Read / Unread", PlPL = "Przeczytane / Nieprzeczytane", DeDE = "Gelesen / Ungelesen", FrFR = "Lu / Non lu", RuRU = "Прочитано / Непрочитано", EsES = "Leído / No leído" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Recalculate_Folder", EnEN = "Recalculate Folder", PlPL = "Przelicz folder", DeDE = "Ordner neu berechnen", FrFR = "Recalculer le dossier", RuRU = "Пересчитать папку", EsES = "Recalcular carpeta" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Rename_Folder", EnEN = "Rename Folder", PlPL = "Zmień nazwę folderu", DeDE = "Benennen Sie den Ordner um", FrFR = "Renommer le dossier", RuRU = "Переименуйте папку", EsES = "Renombrar carpeta" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Reply", EnEN = "Reply", PlPL = "Odpowiedz", DeDE = "Antwort", FrFR = "Répondre", RuRU = "Отвечать", EsES = "Responder" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Reply_All", EnEN = "Reply All", PlPL = "Odpowiedz wszystkim", DeDE = "Allen antworten", FrFR = "Répondre à tous", RuRU = "Ответить всем", EsES = "Responder a todos" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Show_Email", EnEN = "Show", PlPL = "Pokaż", DeDE = "Zeigen", FrFR = "Spectacle", RuRU = "Показывать", EsES = "Espectáculo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Sort", EnEN = "Sorting", PlPL = "Sortowanie", DeDE = "Sortierung", FrFR = "Tri", RuRU = "Сортировка", EsES = "Clasificación" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_spam", EnEN = "Junk mail", PlPL = "Wiadomości śmieci", DeDE = "Junk-Mail", FrFR = "Courrier indésirable", RuRU = "Спам", EsES = "Correo no deseado" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Unread", EnEN = "Unread", PlPL = "Nieprzeczytane", DeDE = "Ungelesen", FrFR = "Non lu", RuRU = "Непрочитано", EsES = "No leído" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Up_Folder", EnEN = "Move Up", PlPL = "Przenieś w górę", DeDE = "Bewegen Sie sich nach oben", FrFR = "Déplacer vers le haut", RuRU = "Вверх", EsES = "Ascender" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Gloal_NotAuthorized", EnEN = "You are not authorized to be here. For more information, contact your system administrator.", PlPL = "Nie masz pozwolenia, żeby tu być. Aby uzyskać więcej informacji, skontaktuj się z administratorem systemu.", DeDE = "Sie sind nicht berechtigt, hier zu sein. Für weitere Informationen wenden Sie sich an Ihren Systemadministrator.", FrFR = "Vous n'êtes pas autorisé à être ici. Pour plus d'informations, contactez votre administrateur système.", RuRU = "Вам не разрешено находиться здесь. Для получения дополнительной информации обратитесь к своему системному администратору.", EsES = "No estás autorizado a estar aquí. Para más información, contacte a su administrador de sistema." });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Action", EnEN = "Action", PlPL = "Akcja", DeDE = "Aktion", FrFR = "Action", RuRU = "Действие", EsES = "Acción" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Address_Email", EnEN = "Email address", PlPL = "Email", DeDE = "E-Mail-Adresse", FrFR = "Adresse e-mail", RuRU = "Адрес электронной почты", EsES = "Dirección de correo electrónico" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Agree_Terms", EnEN = "I agree to the terms and privacy", PlPL = "Zgadzam się na warunki i prywatność", DeDE = "Ich stimme den Nutzungsbedingungen und dem Datenschutz zu", FrFR = "J'accepte les termes et la confidentialité", RuRU = "Я согласен с условиями и конфиденциальностью", EsES = "Acepto los términos y la privacidad" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Associate_Account", EnEN = "Associate your {0} account", PlPL = "Powiąż swoje konto {0}", DeDE = "Verknüpfen Sie Ihr {0}-Konto", FrFR = "Associez votre compte {0}", RuRU = "Свяжите свою учетную запись {0}", EsES = "Asocia tu cuenta {0}" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Audit_Type", EnEN = "Audit type", PlPL = "Rodzaj zapisu", DeDE = "Prüfungstyp", FrFR = "Type de vérification", RuRU = "Тип аудита", EsES = "Tipo de auditoría" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Authenticator_Code", EnEN = "Authenticator code", PlPL = "Kod uwierzytelniający", DeDE = "Authentifizierungscode", FrFR = "Code d'authentification", RuRU = "Код аутентификатора", EsES = "código de autenticación" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Authorizing", EnEN = "Please wait, we are authorizing you...", PlPL = "Proszę czekać, trwa autoryzacja użytkownika ...", DeDE = "Bitte warten Sie, wir autorisieren Sie...", FrFR = "Veuillez patienter, nous vous autorisons…", RuRU = "Пожалуйста, подождите, мы разрешаем вам…", EsES = "Por favor espera, te estamos autorizando…" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_BadAddress", EnEN = "Sorry, there's nothing at this address.", PlPL = "Przepraszamy, pod tym adresem nic nie ma.", DeDE = "Unter dieser Adresse ist leider nichts zu finden.", FrFR = "Désolé, il n'y a rien à cette adresse.", RuRU = "Извините, по этому адресу ничего нет.", EsES = "Lo sentimos, no hay nada en esta dirección." });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Cancel", EnEN = "Cancel", PlPL = "Anuluj", DeDE = "Stornieren", FrFR = "Annuler", RuRU = "Отмена", EsES = "Cancelar" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Check_Inbox", EnEN = "Check Your Inbox", PlPL = "Sprawdź swoją skrzynkę odbiorczą", DeDE = "Überprüfe deinen Posteingang", FrFR = "Vérifiez votre boîte de réception", RuRU = "Проверь свои входящие", EsES = "Revisa tu correo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Column_Name", EnEN = "Column name", PlPL = "Nazwa kolumny", DeDE = "Spaltenname", FrFR = "Nom de colonne", RuRU = "Имя столбца", EsES = "Nombre de la columna" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Confirm", EnEN = "Confirm", PlPL = "Potwierdź", DeDE = "Bestätigen", FrFR = "Confirmer", RuRU = "Подтверждать", EsES = "Confirmar" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Confirm_email", EnEN = "Confirm email", PlPL = "Potwierdź adres email", DeDE = "E-Mail bestätigen", FrFR = "Confirmez votre email", RuRU = "Подтвердите адрес электронной почты", EsES = "Confirmar correo electrónico" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Confirm_Password", EnEN = "Confirm Password", PlPL = "Potwierdź hasło", DeDE = "Bestätige das Passwort", FrFR = "Confirmez le mot de passe", RuRU = "Подтвердите пароль", EsES = "confirmar Contraseña" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Date_Mod", EnEN = "Date of modification", PlPL = "Data modyfikacji", DeDE = "Datum der Änderung", FrFR = "Date de modification", RuRU = "Дата модификации", EsES = "Fecha de modificación" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Delete", EnEN = "Delete", PlPL = "Usuń", DeDE = "Löschen", FrFR = "Supprimer", RuRU = "Удалить", EsES = "Borrar" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Edit", EnEN = "Edit", PlPL = "Edycja", DeDE = "Bearbeiten", FrFR = "Modifier", RuRU = "Редактировать", EsES = "Editar" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Email_Confirm_Error", EnEN = "Your email address has not been confirmed. Please check your inbox for a confirmation email or request a new confirmation link.", PlPL = "Twój adres e-mail nie został potwierdzony. Sprawdź, czy w skrzynce odbiorczej znajduje się e-mail z potwierdzeniem lub poproś o nowy link potwierdzający.", DeDE = "Ihre E-Mail-Adresse wurde nicht bestätigt. Bitte überprüfen Sie Ihren Posteingang auf eine Bestätigungs-E-Mail oder fordern Sie einen neuen Bestätigungslink an.", FrFR = "Votre adresse e-mail n'a pas été confirmée. Veuillez vérifier votre boîte de réception pour un e-mail de confirmation ou demander un nouveau lien de confirmation.", RuRU = "Ваш адрес электронной почты не подтвержден. Пожалуйста, проверьте свой почтовый ящик на наличие письма с подтверждением или запросите новую ссылку для подтверждения.", EsES = "Su dirección de correo electrónico no ha sido confirmada. Revise su bandeja de entrada para recibir un correo electrónico de confirmación o solicite un nuevo enlace de confirmación." });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Email_Send", EnEN = "Send E-mail", PlPL = "Wyślij email", DeDE = "E-Mail senden", FrFR = "Envoyer un e-mail", RuRU = "Отправить электронное письмо", EsES = "Enviar correo electrónico" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Email_Support", EnEN = "The default UI requires a user store with email support.", PlPL = "Domyślny UI wymaga użytkownika z obsługą poczty elektronicznej.", DeDE = "Die Standardbenutzeroberfläche erfordert einen Benutzerspeicher mit E-Mail-Unterstützung.", FrFR = "L'interface utilisateur par défaut nécessite un magasin d'utilisateurs avec prise en charge par courrier électronique.", RuRU = "Для пользовательского интерфейса по умолчанию требуется хранилище пользователей с поддержкой по электронной почте.", EsES = "La interfaz de usuario predeterminada requiere una tienda de usuarios con soporte por correo electrónico." });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Error_Code", EnEN = "Error: Invalid authenticator code", PlPL = "Błąd: nieprawidłowy kod uwierzytelniający", DeDE = "Fehler: Ungültiger Authentifizierungscode", FrFR = "Erreur : code d'authentification invalide", RuRU = "Ошибка: неверный код аутентификатора.", EsES = "Error: código de autenticación no válido" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Error_Code_Entered", EnEN = "Error: Invalid recovery code entered", PlPL = "Błąd: wprowadzono nieprawidłowy kod odzyskiwania", DeDE = "Fehler: Ungültiger Wiederherstellungscode eingegeben", FrFR = "Erreur : code de récupération non valide saisi", RuRU = "Ошибка: введен неверный код восстановления.", EsES = "Error: se ingresó un código de recuperación no válido" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Excel_Export", EnEN = "Export to Excel", PlPL = "Eksportuj do Excela", DeDE = "Nach Excel exportieren", FrFR = "Exporter vers Excel", RuRU = "Экспорт в Excel", EsES = "Exportar a Excel" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Excel_Import", EnEN = "Import from Excel", PlPL = "Import z Excela", DeDE = "Import aus Excel", FrFR = "Importer depuis Excel", RuRU = "Импорт из Excel", EsES = "Importar desde Excel" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_External_Login", EnEN = "External Login", PlPL = "Logowanie zewnętrzne", DeDE = "Externe Anmeldung", FrFR = "Connexion externe", RuRU = "Внешний вход", EsES = "Inicio de sesión externo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Forgot_confirmation", EnEN = "Forgot password confirmation", PlPL = "Potwierdzenie zapomnianego hasła", DeDE = "Passwortbestätigung vergessen", FrFR = "Confirmation du mot de passe oublié", RuRU = "Забыли подтверждение пароля", EsES = "Olvidé la confirmación de contraseña" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Forgot_Pass", EnEN = "Forgot password?", PlPL = "Zapomniałeś hasło?", DeDE = "Passwort vergessen?", FrFR = "Mot de passe oublié?", RuRU = "Забыли пароль?", EsES = "¿Has olvidado tu contraseña?" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Go_Home", EnEN = "Go Home Page", PlPL = "Przejdź do strony domowej", DeDE = "Gehen Sie zur Startseite", FrFR = "Aller à la page d'accueil", RuRU = "Перейти на главную страницу", EsES = "Ir a la página de inicio" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Id_Record", EnEN = "Record Id", PlPL = "Id rekordu", DeDE = "Datensatz-ID", FrFR = "Identifiant d'enregistrement", RuRU = "Идентификатор записи", EsES = "Identificación de registro" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Id_User", EnEN = "User Id", PlPL = "Id użytkownika", DeDE = "Benutzer-ID", FrFR = "ID de l'utilisateur", RuRU = "ID пользователя", EsES = "Identificación de usuario" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Invalid_password_reset", EnEN = "Invalid password reset", PlPL = "Resetowanie nieprawidłowego hasła", DeDE = "Ungültiges Passwort zurückgesetzt", FrFR = "Réinitialisation d'un mot de passe invalide", RuRU = "Неверный сброс пароля", EsES = "Restablecimiento de contraseña no válida" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Invalid_User", EnEN = "Invalid user", PlPL = "Nieprawidłowy użytkownik", DeDE = "Ungültiger Benutzer", FrFR = "Utilisateur invalide", RuRU = "Недействительный пользователь", EsES = "Usuario invalido" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Invalid_User_Pass", EnEN = "Invalid username and/or password", PlPL = "Niepoprawna nazwa użytkownika i / lub hasło", DeDE = "Ungültiger Benutzername und / oder Passwort", FrFR = "Nom d'utilisateur et / ou mot de passe incorrect", RuRU = "Неверное имя пользователя и / или пароль", EsES = "Nombre de usuario y / o contraseña inválido" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_LangaugeCode", EnEN = "Language Code", PlPL = "Kod języka", DeDE = "Sprachcode", FrFR = "Code de langue", RuRU = "Код языка", EsES = "Código de idioma" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Locked_Account", EnEN = "This account has been locked out, please try again later", PlPL = "To konto zostało zablokowane. Spróbuj ponownie później", DeDE = "Dieses Konto wurde gesperrt. Bitte versuchen Sie es später erneut", FrFR = "Ce compte a été verrouillé, veuillez réessayer plus tard", RuRU = "Эта учетная запись заблокирована. Повторите попытку позже.", EsES = "Esta cuenta ha sido bloqueada, inténtalo de nuevo más tarde." });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Locked_Out", EnEN = "Locked out", PlPL = "Zablokowany", DeDE = "Ausgeschlossen", FrFR = "Enfermé dehors", RuRU = "Заблокировано", EsES = "Bloqueado" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Login", EnEN = "LOGIN", PlPL = "ZALOGUJ SIĘ", DeDE = "ANMELDUNG", FrFR = "CONNEXION", RuRU = "АВТОРИЗОВАТЬСЯ", EsES = "ACCESO" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Login_Auth", EnEN = "You've successfully authenticated with {0}. Please enter an email address for this site below and click the Register button to finish logging in.", PlPL = "Pomyślnie uwierzytelniłeś się w {0}. Wprowadź poniżej adres e-mail tej witryny i kliknij przycisk Zarejestruj, aby zakończyć logowanie.", DeDE = "Sie haben sich erfolgreich bei {0} authentifiziert. Bitte geben Sie unten eine E-Mail-Adresse für diese Website ein und klicken Sie auf die Schaltfläche „Registrieren“, um die Anmeldung abzuschließen.", FrFR = "Vous vous êtes authentifié avec succès auprès de {0}. Veuillez saisir une adresse e-mail pour ce site ci-dessous et cliquez sur le bouton S'inscrire pour terminer la connexion.", RuRU = "Вы успешно прошли аутентификацию с помощью {0}. Пожалуйста, введите адрес электронной почты этого сайта ниже и нажмите кнопку «Зарегистрироваться», чтобы завершить вход.", EsES = "Te has autenticado correctamente con {0}. Ingrese una dirección de correo electrónico para este sitio a continuación y haga clic en el botón Registrarse para terminar de iniciar sesión." });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_LogOut_Header", EnEN = "Logout", PlPL = "Wyloguj", DeDE = "Ausloggen", FrFR = "Se déconnecter", RuRU = "Выйти", EsES = "Cerrar sesión" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_LogOut1", EnEN = "You are attempting to log out of application.", PlPL = "Próbujesz wylogować się z aplikacji.", DeDE = "Sie versuchen, sich von der Anwendung abzumelden. ", FrFR = "Vous essayez de vous déconnecter de l'application. ", RuRU = "Вы пытаетесь выйти из приложения.", EsES = "Está intentando cerrar sesión en la aplicación. " });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_LogOut2", EnEN = "Do you really want to log out?", PlPL = "Czy na pewno chcesz się wylogować?", DeDE = "Möchten Sie sich wirklich abmelden?", FrFR = "Voulez-vous vraiment vous déconnecter ?", RuRU = "Вы действительно хотите выйти из системы?", EsES = " ¿Realmente quieres cerrar sesión?" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_New_Value", EnEN = "New value", PlPL = "Nowa wartość", DeDE = "Neuer Wert", FrFR = "Nouvelle valeur", RuRU = "Новое значение", EsES = "Nuevo valor" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_No_Email_Found", EnEN = "No user found with the provided email address.", PlPL = "Nie znaleziono użytkownika o podanym adresie e-mail.", DeDE = "Kein Benutzer mit der angegebenen E-Mail-Adresse gefunden.", FrFR = "Aucun utilisateur trouvé avec l'adresse e-mail fournie.", RuRU = "Ни один пользователь не найден с указанным адресом электронной почты.", EsES = "No se encontró ningún usuario con la dirección de correo electrónico proporcionada." });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_NotAccount", EnEN = "Don't have an account?  -", PlPL = "Nie masz jeszcze konta ? -", DeDE = "Sie haben noch kein Konto? -", FrFR = "Vous n'avez pas de compte ? -", RuRU = "У вас нет аккаунта? -", EsES = "¿No tienes una cuenta? -" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Old_value", EnEN = "Old value", PlPL = "Poprzednia wartość", DeDE = "Alter Wert", FrFR = "Ancienne valeur", RuRU = "Старое значение", EsES = "Valor antiguo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Page_Not_Found", EnEN = "The page with the given address was not found", PlPL = "Strona o podanym adresie nie została odnaleziona", DeDE = "Die Seite mit der angegebenen Adresse wurde nicht gefunden", FrFR = "La page avec l'adresse indiquée n'a pas été trouvée", RuRU = "Страница с данным адресом не найдена", EsES = "No se encontró la página con la dirección proporcionada." });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Password", EnEN = "Password", PlPL = "Hasło", DeDE = "Passwort", FrFR = "Mot de passe", RuRU = "Пароль", EsES = "Contraseña" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Password_Reset_Link", EnEN = "The password reset link is invalid", PlPL = "Link do resetowania hasła jest nieprawidłowy", DeDE = "Der Link zum Zurücksetzen des Passworts ist ungültig", FrFR = "Le lien de réinitialisation du mot de passe n'est pas valide", RuRU = "Ссылка для сброса пароля недействительна", EsES = "El enlace para restablecer la contraseña no es válido" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Primary_Key", EnEN = "Primary Key", PlPL = "Indeks główny", DeDE = "Primärschlüssel", FrFR = "Clé primaire", RuRU = "Основной ключ", EsES = "Clave primaria" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Recovery_Code", EnEN = "Recovery Code", PlPL = "Kod odzyskiwania", DeDE = "Wiederherstellungscode", FrFR = "Code de récupération", RuRU = "Код восстановления", EsES = "Código de recuperación" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Recovery_Code_Verification", EnEN = "Recovery code verification", PlPL = "Weryfikacja kodu odzyskiwania", DeDE = "Überprüfung des Wiederherstellungscodes", FrFR = "Vérification du code de récupération", RuRU = "Проверка кода восстановления", EsES = "Verificación del código de recuperación" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Recovery_Code_Verification_Text", EnEN = "You have requested to log in with a recovery code. This login will not be remembered until you provide an authenticator app code at log in or disable 2FA and log in again", PlPL = "Zażądałeś zalogowania się przy użyciu kodu odzyskiwania. Ten login nie zostanie zapamiętany, dopóki nie podasz kodu aplikacji uwierzytelniającej podczas logowania lub nie wyłączysz 2FA i zalogujesz się ponownie", DeDE = "Sie haben die Anmeldung mit einem Wiederherstellungscode beantragt. Diese Anmeldung wird erst gespeichert, wenn Sie bei der Anmeldung einen Authentifizierungs-App-Code angeben oder 2FA deaktivieren und sich erneut anmelden", FrFR = "Vous avez demandé à vous connecter avec un code de récupération. Cette connexion ne sera pas mémorisée jusqu'à ce que vous fournissiez un code d'application d'authentification lors de la connexion ou que vous désactiviez 2FA et que vous vous reconnectiez.", RuRU = "Вы запросили вход с кодом восстановления. Этот логин не будет запомнен до тех пор, пока вы не предоставите код приложения для аутентификации при входе в систему или не отключите 2FA и не войдете снова.", EsES = "Ha solicitado iniciar sesión con un código de recuperación. Este inicio de sesión no se recordará hasta que proporcione un código de aplicación de autenticación al iniciar sesión o deshabilite 2FA e inicie sesión nuevamente" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Register", EnEN = "Register", PlPL = "Rejestruj", DeDE = "Registrieren", FrFR = "Registre", RuRU = "регистр", EsES = "Registro" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Register_Confirmation", EnEN = "Register Confirmation", PlPL = "Potwierdzenie rejestracji", DeDE = "Anmeldebestätigung", FrFR = "Confirmation d'inscription", RuRU = "Подтверждение регистрации", EsES = "Confirmación de registro" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Register_Confirmation_1", EnEN = "This app does not currently have a real email sender registered, see", PlPL = "Ta aplikacja nie ma obecnie zarejestrowanego prawdziwego nadawcy wiadomości e-mail, patrz", DeDE = "Für diese App ist derzeit kein echter E-Mail-Absender registriert, siehe", FrFR = "Cette application n'a actuellement pas de véritable expéditeur d'e-mail enregistré, voir", RuRU = "В этом приложении в настоящее время не зарегистрирован настоящий отправитель электронной почты, см.", EsES = "Esta aplicación actualmente no tiene un remitente de correo electrónico real registrado, consulte" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Register_Confirmation_2", EnEN = "these docs", PlPL = "te dokumenty", DeDE = "diese Dokumente", FrFR = "ces documents", RuRU = "эти документы", EsES = "estos documentos" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Register_Confirmation_3", EnEN = "how to configure a real email sender. Normally this would be emailed:", PlPL = "", DeDE = "So konfigurieren Sie einen echten E-Mail-Absender. Normalerweise würde dies per E-Mail gesendet werden:", FrFR = "comment configurer un véritable expéditeur d'e-mail. Normalement, cela serait envoyé par e-mail :", RuRU = "как настроить настоящего отправителя электронной почты. Обычно это отправляется по электронной почте:", EsES = "cómo configurar un remitente de correo electrónico real. Normalmente esto se enviaría por correo electrónico:" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Register_Confirmation_4", EnEN = "here to confirm your account", PlPL = "tutaj, aby potwierdzić swoje konto", DeDE = "Hier können Sie Ihr Konto bestätigen", FrFR = "ici pour confirmer votre compte", RuRU = "здесь, чтобы подтвердить свой аккаунт", EsES = "aquí para confirmar tu cuenta" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Register_Confirmation_5", EnEN = "Please check your email to confirm your account", PlPL = "Sprawdź swoją pocztę e-mail, aby potwierdzić swoje konto", DeDE = "Bitte überprüfen Sie Ihre E-Mails, um Ihr Konto zu bestätigen", FrFR = "Veuillez vérifier votre courrier électronique pour confirmer votre compte", RuRU = "Пожалуйста, проверьте свою электронную почту, чтобы подтвердить свою учетную запись", EsES = "Por favor revise su correo electrónico para confirmar su cuenta." });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Register_Confirmation_6", EnEN = "Error finding user for unspecified email", PlPL = "Błąd podczas wyszukiwania użytkownika dla nieokreślonego adresu e-mail", DeDE = "Fehler beim Suchen des Benutzers für eine nicht angegebene E-Mail-Adresse", FrFR = "Erreur lors de la recherche d'un utilisateur pour une adresse e-mail non spécifiée", RuRU = "Ошибка поиска пользователя по неопределенному адресу электронной почты.", EsES = "Error al encontrar usuario para correo electrónico no especificado" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Remember_me", EnEN = "Remember me?", PlPL = "Zapamiętaj mnie ?", DeDE = "mich erinnern?", FrFR = "souviens-toi de moi?", RuRU = "запомнить меня?", EsES = "¿Acuérdate de mí?" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Reset_Pass", EnEN = "Reset password", PlPL = "Resetuj hasło", DeDE = "Passwort zurücksetzen", FrFR = "Réinitialiser le mot de passe", RuRU = "Сброс пароля", EsES = "Restablecer la contraseña" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Reset_Password_1", EnEN = "Reset password confirmation", PlPL = "Potwierdzenie resetu hasła", DeDE = "Passwortbestätigung zurücksetzen", FrFR = "Réinitialiser la confirmation du mot de passe", RuRU = "Сброс подтверждения пароля", EsES = "Restablecer confirmación de contraseña" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Reset_Password_2", EnEN = "Your password has been reset", PlPL = "Twoje hasło zostało zresetowane", DeDE = "Dein Passwort wurde zurück gesetzt", FrFR = "votre mot de passe a été réinitialisé", RuRU = "Ваш пароль был сброшен", EsES = "Tu contraseña ha sido restablecida" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Reset_Password_3", EnEN = "Please click here to log in", PlPL = "Kliknij tutaj, aby się zalogować", DeDE = "Bitte klicken Sie hier, um sich anzumelden", FrFR = "Veuillez cliquer ici pour vous connecter", RuRU = "Пожалуйста кликните сюда для авторизации", EsES = "Por favor haga clic aquí para iniciar sesión" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Reset_Text", EnEN = "No worries! Just enter your email address below and we'll send you a link to reset your password.", PlPL = "Bez obaw! Po prostu wpisz poniżej swój adres e-mail, a my wyślemy Ci link umożliwiający zresetowanie hasła.", DeDE = "Keine Sorge! Geben Sie einfach unten Ihre E-Mail-Adresse ein und wir senden Ihnen einen Link zum Zurücksetzen Ihres Passworts.", FrFR = "Pas de soucis! Entrez simplement votre adresse e-mail ci-dessous et nous vous enverrons un lien pour réinitialiser votre mot de passe.", RuRU = "Не беспокойся! Просто введите свой адрес электронной почты ниже, и мы вышлем вам ссылку для сброса пароля.", EsES = "¡No hay problema! Simplemente ingrese su dirección de correo electrónico a continuación y le enviaremos un enlace para restablecer su contraseña." });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Reset_Your_Password_1", EnEN = "Reset Your Password", PlPL = "Zresetuj swoje hasło", DeDE = "Setze dein Passwort zurück", FrFR = "Réinitialisez votre mot de passe", RuRU = "Сбросить пароль", EsES = "Restablecer su contraseña" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Reset_Your_Password_2", EnEN = "Please enter your new password below. Make sure your password is strong and memorable for you", PlPL = "Wpisz poniżej swoje nowe hasło. Upewnij się, że Twoje hasło jest silne i łatwe do zapamiętania", DeDE = "Bitte geben Sie Ihr neues Passwort ein. Stellen Sie sicher, dass Ihr Passwort sicher und einprägsam für Sie ist", FrFR = "S'il vous plaît entrez votre nouveau mot de passe ci-dessous. Assurez-vous que votre mot de passe est fort et mémorable pour vous", RuRU = "Пожалуйста, введите новый пароль ниже. Убедитесь, что ваш пароль надежный и запоминающийся для вас.", EsES = "Por favor ingrese su nueva contraseña a continuación. Asegúrese de que su contraseña sea segura y fácil de recordar" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Reset_Your_Password_3", EnEN = "New password", PlPL = "Nowe hasło", DeDE = "Neues Kennwort", FrFR = "Nouveau mot de passe", RuRU = "Новый пароль", EsES = "Nueva contraseña" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Reset_Your_Password_4", EnEN = "Reset password", PlPL = "Resetuj hasło", DeDE = "Passwort zurücksetzen", FrFR = "Réinitialiser le mot de passe", RuRU = "Сброс пароля", EsES = "Restablecer la contraseña" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Return_to_Login", EnEN = "Return to Login", PlPL = "Wróć do logowania", DeDE = "Zurück zum Login", FrFR = "Revenir à la connexion", RuRU = "Вернуться к входу", EsES = "Volver a iniciar sesión" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Save", EnEN = "Save", PlPL = "Zapisz", DeDE = "Speichern", FrFR = "Sauvegarder", RuRU = "Сохранять", EsES = "Ahorrar" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Search_Text", EnEN = "Insert search text", PlPL = "Wprowadź szukany tekst", DeDE = "Suchtext einfügen", FrFR = "Insérer le texte de recherche", RuRU = "Вставить поисковый текст", EsES = "Insertar texto de búsqueda" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Send_Check", EnEN = "We've sent an email to the address you provided. Please check your inbox for a link to reset your password. If you don't see the email, be sure to check your spam or junk folder.", PlPL = "Wysłaliśmy wiadomość e-mail na podany adres. Sprawdź, czy w Twojej skrzynce odbiorczej znajduje się link umożliwiający zresetowanie hasła. Jeśli nie widzisz wiadomości e-mail, sprawdź folder ze spamem lub wiadomościami-śmieciami.", DeDE = "Wir haben eine E-Mail an die von Ihnen angegebene Adresse gesendet. Bitte überprüfen Sie Ihren Posteingang auf einen Link zum Zurücksetzen Ihres Passworts. Wenn Sie die E-Mail nicht sehen, überprüfen Sie unbedingt Ihren Spam- oder Junk-Ordner.", FrFR = "Nous avons envoyé un e-mail à l'adresse que vous avez fournie. Veuillez vérifier votre boîte de réception pour obtenir un lien permettant de réinitialiser votre mot de passe. Si vous ne voyez pas l'e-mail, assurez-vous de vérifier votre dossier spam ou courrier indésirable.", RuRU = "Мы отправили электронное письмо на указанный вами адрес. Пожалуйста, проверьте свой почтовый ящик на наличие ссылки для сброса пароля. Если вы не видите письмо, обязательно проверьте папку «Спам» или «Нежелательная почта».", EsES = "Le hemos enviado un correo electrónico a la dirección que proporcionó. Por favor revise su bandeja de entrada para ver un enlace para restablecer su contraseña. Si no ve el correo electrónico, asegúrese de revisar su carpeta de correo no deseado o basura." });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Set_In_Active", EnEN = "Set in active", PlPL = "Ustaw aktywny", DeDE = "Auf „Aktiv“ setzen", FrFR = "Activer", RuRU = "Установить в активном состоянии", EsES = "Establecer en activo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Set_Permission", EnEN = "Set permission", PlPL = "Ustaw uprawnienia", DeDE = "Berechtigung festlegen", FrFR = "Définir l'autorisation", RuRU = "Установить разрешение", EsES = "Establecer permiso" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Sign_in", EnEN = "Sign in", PlPL = "Zaloguj się", DeDE = "Anmelden", FrFR = "S'identifier", RuRU = "Войти", EsES = "Iniciar sesión" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Sign_up", EnEN = "Sign up", PlPL = "Zarejestruj się", DeDE = "Anmeldung", FrFR = "S'inscrire", RuRU = "Зарегистрироваться", EsES = "Inscribirse" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Table_Name", EnEN = "Table Name", PlPL = "Nazwa tabeli", DeDE = "Tabellenname", FrFR = "Nom de la table", RuRU = "Имя таблицы", EsES = "Nombre de la tabla" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Terms_Agree", EnEN = "I agree to the terms and privacy", PlPL = "Zgadzam się na warunki i prywatność", DeDE = "Ich stimme den Nutzungsbedingungen und dem Datenschutz zu", FrFR = "J'accepte les conditions et la confidentialité", RuRU = "Я согласен с условиями и конфиденциальностью", EsES = "Acepto los términos y privacidad." });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_TimeZone", EnEN = "TimeZone Name", PlPL = "Nazwa strefy czasowej", DeDE = "Name der Zeitzone", FrFR = "Nom du fuseau horaire", RuRU = "Название часового пояса", EsES = "Nombre de la zona horaria" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Two-factor_Authentication", EnEN = "Two-factor authentication", PlPL = "Uwierzytelnianie dwuskładnikowe", DeDE = "Zwei-Faktor-Authentifizierung", FrFR = "Authentification à deux facteurs", RuRU = "Двухфакторная аутентификация", EsES = "Autenticación de dos factores" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Two-factor_Code", EnEN = "Your login is secured with two-factor authentication via email. Please enter the code sent to your email address below", PlPL = "Twój login jest zabezpieczony dwuskładnikowym uwierzytelnianiem za pośrednictwem poczty elektronicznej. Wpisz poniżej kod wysłany na Twój adres e-mail", DeDE = "Ihr Login ist durch eine Zwei-Faktor-Authentifizierung per E-Mail gesichert. Bitte geben Sie unten den Code ein, der an Ihre E-Mail-Adresse gesendet wurde", FrFR = "Votre connexion est sécurisée avec une authentification à deux facteurs par e-mail. Veuillez saisir ci-dessous le code envoyé à votre adresse email", RuRU = "Ваш вход защищен двухфакторной аутентификацией по электронной почте. Пожалуйста, введите код, отправленный на ваш адрес электронной почты ниже", EsES = "Su inicio de sesión está protegido con autenticación de dos factores por correo electrónico. Por favor ingrese el código enviado a su dirección de correo electrónico a continuación" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Two-Factor_Unable", EnEN = "Unable to load two-factor authentication user", PlPL = "Nie można wczytać użytkownika z uwierzytelnianiem dwuskładnikowym", DeDE = "Der Benutzer für die Zwei-Faktor-Authentifizierung konnte nicht geladen werden", FrFR = "Impossible de charger l'utilisateur d'authentification à deux facteurs", RuRU = "Невозможно загрузить пользователя двухфакторной аутентификации.", EsES = "No se puede cargar el usuario de autenticación de dos factores" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_User_Name", EnEN = "User Name", PlPL = "Nazwa użytkownika", DeDE = "Nutzername", FrFR = "Nom d'utilisateur", RuRU = "имя пользователя", EsES = "nombre de usuario" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Localizer_Actions", EnEN = "Actions", PlPL = "Akcja", DeDE = "Aktionen", FrFR = "Actions", RuRU = "Действия", EsES = "Comportamiento" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Localizer_Add_New_Text", EnEN = "Add new localization text", PlPL = "Dodaj nowy tekst lokalizacyjny", DeDE = "Neuen Lokalisierungstext hinzufügen", FrFR = "Ajouter un nouveau texte de localisation", RuRU = "Добавить новый текст локализации", EsES = "Agregar nuevo texto de localización" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Localizer_Cancel", EnEN = "Cancel", PlPL = "Anuluj", DeDE = "Absagen", FrFR = "Annuler", RuRU = "Отмена", EsES = "Cancelar" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Localizer_Create", EnEN = "Save", PlPL = "Zapisz", DeDE = "Speichern", FrFR = "Sauvegarder", RuRU = "Сохранять", EsES = "Ahorrar" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Localizer_Delete", EnEN = "Delete", PlPL = "Usuń", DeDE = "Löschen", FrFR = "Effacer", RuRU = "Удалить", EsES = "Borrar" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Localizer_Deleted_Text", EnEN = "Deleted text", PlPL = "Usuwany tekst", DeDE = "Text gelöscht", FrFR = "Texte supprimé", RuRU = "Удаленный текст", EsES = "Eliminar texto" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Localizer_Edit", EnEN = "Edit", PlPL = "Edycja", DeDE = "Bearbeiten", FrFR = "Éditer", RuRU = "Редактировать", EsES = "Editar" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Localizer_Edit_Text", EnEN = "Edit the localization text", PlPL = "Edytuj tekst lokalizacyjny", DeDE = "Bearbeiten Sie den Lokalisierungstext", FrFR = "Modifier le texte de localisation", RuRU = "Отредактируйте текст локализации", EsES = "Editar el texto de localización" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Localizer_English_Text", EnEN = "Text - English", PlPL = "Tekst - Angielski", DeDE = "Texte - Englisch", FrFR = "Texte - Anglais", RuRU = "Текст - английский", EsES = "Texto - Inglés" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Localizer_France_Text", EnEN = "Text - French", PlPL = "Tekst - francuski", DeDE = "Text - Französisch", FrFR = "Texte - français", RuRU = "Текст - французский", EsES = "Texto - francés" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Localizer_German_Text", EnEN = "Text - German", PlPL = "Tekst - niemiecki", DeDE = "Text - Deutsch", FrFR = "Texte - allemand", RuRU = "Текст - немецкий", EsES = "Texto - alemán" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Localizer_Key_Text", EnEN = "Localize key text", PlPL = "Klucz tekstu lokalizacyjnego", DeDE = "Schlüsseltext lokalisieren", FrFR = "Localiser le texte clé", RuRU = "Локализация ключевого текста", EsES = "" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Localizer_Polish_Text", EnEN = "Text - Polish", PlPL = "Tekst - polski", DeDE = "Text - Polnisch", FrFR = "Texte - polonais", RuRU = "Текст - польский", EsES = "Texto - Polaco" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Localizer_Russian_Text", EnEN = "Text - Russian", PlPL = "Tekst - rosyjski", DeDE = "Text - Russisch", FrFR = "Texte - russe", RuRU = "Текст - русский", EsES = "Texto - ruso" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Localizer_Spanish_Text", EnEN = "Text - Spanish", PlPL = "Text - Hiszpański", DeDE = "Text - Spanisch", FrFR = "Texte - Espagnol", RuRU = "Текст - испанский", EsES = "Texto - español" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Localizer_Sure_Delete", EnEN = "Are you sure you want to delete the selected text?", PlPL = "Czy napewno chcesz usunąć wybrany tekst?", DeDE = "Möchten Sie den ausgewählten Text wirklich löschen?", FrFR = "Voulez-vous vraiment supprimer le texte sélectionné ?", RuRU = "Вы уверены, что хотите удалить выделенный текст?", EsES = "¿Está seguro de que desea eliminar el texto seleccionado?" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Log_With", EnEN = "Login with {0}", PlPL = "Zaloguj się za pomocą {0}", DeDE = "Melden Sie sich mit {0} an", FrFR = "Connectez-vous avec {0}", RuRU = "Войдите с помощью {0}", EsES = "Iniciar sesión con {0}" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "MailAccount_Edit_Account", EnEN = "Edit account data", PlPL = "Edytuj dane konta", DeDE = "Kontodatum bearbeiten", FrFR = "Modifier la date du compte", RuRU = "Изменить дату аккаунта", EsES = "Editar fecha de cuenta" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "MailAccount_Email_Address", EnEN = "Email address", PlPL = "Adres email", DeDE = "E-Mail-Addresse", FrFR = "Adresse e-mail", RuRU = "Адрес электронной почты", EsES = "Dirección de correo electrónico" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "MailAccount_EncType", EnEN = "ENC type", PlPL = "Typ ENC", DeDE = "ENC-Typ", FrFR = "Type d'ENC", RuRU = "Тип ЭНК", EsES = "Tipo ENC" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "MailAccount_GetFolders", EnEN = "Get folders", PlPL = "Pobierz foldery", DeDE = "Holen Sie sich Ordner", FrFR = "Obtenir des dossiers", RuRU = "Получить папки", EsES = "Obtener carpetas" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "MailAccount_In_Port", EnEN = "In Port", PlPL = "Port przychodzący", DeDE = "Im Hafen", FrFR = "Au port", RuRU = "В порту", EsES = "En puerto" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "MailAccount_In_Server", EnEN = "In Server", PlPL = "Server przychodzący", DeDE = "Im Server", FrFR = "Dans le serveur", RuRU = "На сервере", EsES = "En el servidor" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "MailAccount_In_settings", EnEN = "Incoming mail server settings", PlPL = "Ustawienia serwera poczty przychodzącej", DeDE = "Einstellungen des Posteingangsservers", FrFR = "Paramètres du serveur de courrier entrant", RuRU = "Настройки сервера входящей почты", EsES = "Configuración del servidor de correo entrante" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "MailAccount_Incoming_SSL", EnEN = "Incoming Is SSL", PlPL = "Przychodzące jest SSL", DeDE = "Eingehend ist SSL", FrFR = "Entrant est SSL", RuRU = "Входящий SSL", EsES = "Entrante es SSL" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "MailAccount_Login", EnEN = "Login", PlPL = "Login", DeDE = "Anmeldung", FrFR = "Connexion", RuRU = "Авторизоваться", EsES = "Acceso" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "MailAccount_New_Account", EnEN = "Create new account", PlPL = "Utwórz nowe konto email", DeDE = "Ein neues Konto erstellen", FrFR = "Créer un nouveau compte", RuRU = "Создать новый аккаунт", EsES = "Crea una cuenta nueva" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "MailAccount_Out_Port", EnEN = "Out port", PlPL = "Port wychodzący", DeDE = "Out-Port", FrFR = "Port de sortie", RuRU = "из порта", EsES = "Puerto de salida" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "MailAccount_Out_Server", EnEN = "Out Server", PlPL = "Server wychodzący", DeDE = "Out-Server", FrFR = "Hors serveur", RuRU = "внешний сервер", EsES = "Fuera del servidor" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "MailAccount_Out_settings", EnEN = "Outgoing mail server settings", PlPL = "Ustawienia serwera poczty wychodzącej", DeDE = "Einstellungen des Postausgangsservers", FrFR = "Paramètres du serveur de courrier sortant", RuRU = "Настройки сервера исходящей почты", EsES = "Configuración del servidor de correo saliente" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "MailAccount_Outgoing_Authentication", EnEN = "Does the outgoing server require authentication?", PlPL = "Czy server wychodzący wymaga uwierzytelnienia?", DeDE = "Erfordert der Postausgangsserver eine Authentifizierung?", FrFR = "Le serveur sortant nécessite-t-il une authentification ?", RuRU = "Исходящий сервер требует аутентификации?", EsES = "¿El servidor saliente requiere autenticación?" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "MailAccount_Outgoing_SSL", EnEN = "Outgoing port Is SSL?", PlPL = "Port wychodzący czy SSL?", DeDE = "Ist der ausgehende Port SSL?", FrFR = "Le port sortant est-il SSL ?", RuRU = "Исходящий порт SSL?", EsES = "Puerto de salida ¿Es SSL?" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "MailAccount_SSL", EnEN = "SSL", PlPL = "SSL", DeDE = "SSL", FrFR = "SSL", RuRU = "SSL", EsES = "SSL" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "MailAccount_ViewFolders", EnEN = "View folders", PlPL = "Pokaż foldery", DeDE = "Ordner anzeigen", FrFR = "Afficher les dossiers", RuRU = "Просмотр папок", EsES = "Ver carpetas" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Panel_Active", EnEN = "Panel Is Active", PlPL = "Panel jest aktywny", DeDE = "Panel ist aktiv", FrFR = "Le panneau est actif", RuRU = "Панель активна", EsES = "El panel está activo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Panel_Add_New_Text", EnEN = "Parameters of the new panel", PlPL = "Parametry nowego panelu", DeDE = "Parameter des neuen Panels", FrFR = "Paramètres du nouveau panneau", RuRU = "Параметры новой панели", EsES = "Parámetros del nuevo panel." });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Panel_Allow_Dragging", EnEN = "Allow Dragging", PlPL = "Zezwalaj na przeciąganie", DeDE = "Ziehen zulassen", FrFR = "Autoriser le glisser", RuRU = "Разрешить перетаскивание", EsES = "Permitir arrastrar" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Panel_Column_Number", EnEN = "Column Number", PlPL = "Numer kolumny", DeDE = "Spaltennummer", FrFR = "Numéro de colonne", RuRU = "Номер столбца", EsES = "Número de columna" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Panel_Columns_Count", EnEN = "Number of columns in Dashboard Layouts", PlPL = "Ilość column w Dashboard Layouts", DeDE = "Anzahl der Spalten in Dashboard-Layouts", FrFR = "Nombre de colonnes dans les présentations du tableau de bord", RuRU = "Количество столбцов в макетах информационной панели", EsES = "Número de columnas en diseños de panel" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Panel_Component_Name", EnEN = "Component Type", PlPL = "Typ komponentu", DeDE = "Komponententyp", FrFR = "Type de composant", RuRU = "Тип компонента", EsES = "Tipo de componente" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Panel_Component_Users", EnEN = "Panel can use only by", PlPL = "Panel może być używany tylko przez", DeDE = "Das Panel kann nur von verwendet werden", FrFR = "Le panneau ne peut être utilisé que par", RuRU = "Панелью может пользоваться только", EsES = "El panel sólo puede ser utilizado por" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Panel_Delete", EnEN = "Delete panel", PlPL = "Usuń panel", DeDE = "Panel löschen", FrFR = "Supprimer le panneau", RuRU = "Удалить панель", EsES = "Eliminar panel" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Panel_Edit", EnEN = "Edit panel", PlPL = "Edycja panelu", DeDE = "Bearbeitungsfeld", FrFR = "Panneau d'édition", RuRU = "Панель редактирования", EsES = "Editar panel" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Panel_MaxSizeX", EnEN = "Max Size X", PlPL = "Max Rozmiar X", DeDE = "Max Größe X", FrFR = "Max Taille X", RuRU = "Max Размер Х", EsES = "Max Talla X" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Panel_MaxSizeY", EnEN = "Max Size Y", PlPL = "Max Rozmiar Y", DeDE = "Max Größe Y", FrFR = "Max Taille Y", RuRU = "Max Размер Y", EsES = "Max Talla Y" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Panel_Name", EnEN = "Panel Name", PlPL = "Nazwa Panelu", DeDE = "Panelname", FrFR = "Nom du panneau", RuRU = "Название панели", EsES = "Nombre del panel" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Panel_New", EnEN = "New panel", PlPL = "Nowy panel", DeDE = "Neues Panel", FrFR = "Nouveau panneau", RuRU = "Новая панель", EsES = "Nuevo panel" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Panel_Row_Number", EnEN = "Row Number", PlPL = "Numer wiersza", DeDE = "Zeilennummer", FrFR = "Numéro de ligne", RuRU = "Номер строки", EsES = "Numero de fila" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Panel_SizeX", EnEN = "Size X", PlPL = "Rozmiar X", DeDE = "Größe X", FrFR = "Taille X", RuRU = "Размер Х", EsES = "Talla X" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Panel_SizeY", EnEN = "Size Y", PlPL = "Rozmiar Y", DeDE = "Größe Y", FrFR = "Taille Y", RuRU = "Размер Y", EsES = "Talla Y" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Active", EnEN = "Set active", PlPL = "Aktywowanie", DeDE = "Aktivieren", FrFR = "Définir actif", RuRU = "Установить активное", EsES = "Establecer activo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Add_New", EnEN = "Add new menu position", PlPL = "Dodaj nową pozycję do menu", DeDE = "Neue Menüposition hinzufügen", FrFR = "Ajouter une nouvelle position dans le menu", RuRU = "Добавить новую позицию меню", EsES = "Agregar nueva posición del menú" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Create", EnEN = "Create new", PlPL = "Tworzyć nowe", DeDE = "Erstelle neu", FrFR = "Créer un nouveau", RuRU = "Создавать новое", EsES = "Crear nuevo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Delete", EnEN = "Delete data", PlPL = "Usuwanie danych", DeDE = "Daten löschen", FrFR = "Suprimmer les données", RuRU = "Удалить данные", EsES = "Borrar datos" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Edit", EnEN = "Edit data", PlPL = "Edycja danych", DeDE = "Daten bearbeiten", FrFR = "Modifier les données", RuRU = "Редактировать данные", EsES = "Editar datos" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Empty1", EnEN = "Empty 1", PlPL = "Zapas 1", DeDE = "Leer 1", FrFR = "Vide 1", RuRU = "Пусто 1", EsES = "Vacío 1" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Empty2", EnEN = "Empty 2", PlPL = "Zapas 2", DeDE = "Leer 2", FrFR = "Vide 2", RuRU = "Пусто 2", EsES = "Vacío 2" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Empty3", EnEN = "Empty 3", PlPL = "Zapas 3", DeDE = "Leer 3", FrFR = "Vide 3", RuRU = "Пусто 3", EsES = "Vacío 3" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Expand", EnEN = "Node expanded", PlPL = "Węzeł rozwinięty", DeDE = "Knoten erweitert", FrFR = "Nœud développé", RuRU = "Узел расширен", EsES = "Nodo expandido" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Export", EnEN = "Export data", PlPL = "Export danych", DeDE = "Daten exportieren", FrFR = "Exporter des données", RuRU = "Экспорт данных", EsES = "Exportar datos" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Have_Permission", EnEN = "Menu have permission", PlPL = "Menu posiada uprawnienia", DeDE = "Menü haben Berechtigung", FrFR = "Le menu a la permission", RuRU = "Меню имеет разрешение", EsES = "Menú tiene permiso" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Have_Subfolders", EnEN = "Menu have subfolders", PlPL = "Menu posiada podfoldery", DeDE = "Das Menü hat Unterordner", FrFR = "Le menu a des sous-dossiers", RuRU = "Меню имеет подпапки", EsES = "El menú tiene subcarpetas." });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Icon", EnEN = "Icon", PlPL = "Ikona", DeDE = "Symbol", FrFR = "Icône", RuRU = "Икона", EsES = "Icono" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Import", EnEN = "Import data", PlPL = "Import danych", DeDE = "Daten importieren", FrFR = "Importer des données", RuRU = "Импортировать данные", EsES = "Datos de importacion" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Manage_Permission", EnEN = "Manage Permission", PlPL = "Zarządzaj uprawnieniami", DeDE = "Berechtigung verwalten", FrFR = "Gérer les autorisations", RuRU = "Управление разрешениями", EsES = "Administrar permiso" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Manage_Roles", EnEN = "Manage roles", PlPL = "Zarządzanie rolami", DeDE = "Rollen verwalten", FrFR = "Gérer les rôles", RuRU = "Управление ролями", EsES = "Administrar roles" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Menu_Name", EnEN = "Menu Name", PlPL = "Nazwa menu", DeDE = "Menüname", FrFR = "Nom du menu", RuRU = "Название меню", EsES = "Nombre del menú" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Page_Address", EnEN = "Page address", PlPL = "Adres strony", DeDE = "Seitenadresse", FrFR = "Adresse de la page", RuRU = "Адрес страницы", EsES = "Dirección de la página" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Page_Status", EnEN = "Page Status", PlPL = "Status strony", DeDE = "Seitenstatus", FrFR = "Statut de la page", RuRU = "Статус страницы", EsES = "Estado de la página" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Parent_number", EnEN = "Parent element number", PlPL = "Numer elementu nadrzędnego", DeDE = "Nummer des übergeordneten Elements", FrFR = "Numéro de l'élément parent", RuRU = "Номер родительского элемента", EsES = "Número de elemento principal" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Reset_Password", EnEN = "Reset password", PlPL = "Resetowanie hasła", DeDE = "Passwort zurücksetzen", FrFR = "Réinitialiser le mot de passe", RuRU = "Сброс пароля", EsES = "Restablecer la contraseña" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Role_List", EnEN = "Select role", PlPL = "Wybierz rolę", DeDE = "Rolle auswählen", FrFR = "Sélectionnez un rôle", RuRU = "Выберите роль", EsES = "Seleccionar rol" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Search", EnEN = "Search data", PlPL = "Szukanie danych", DeDE = "Suchdaten", FrFR = "Rechercher des données", RuRU = "Поиск данных", EsES = "Datos de búsqueda" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Target", EnEN = "Open page in ..", PlPL = "Otwórz stronę w ..", DeDE = "Seite öffnen in ..", FrFR = "Ouvrir la page dans ..", RuRU = "Открыть страницу в ..", EsES = "Abrir página en .." });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_URL_Address", EnEN = "Page URL address", PlPL = "Adres URL strony", DeDE = "URL-Adresse der Seite", FrFR = "Adresse URL de la page", RuRU = "URL-адрес страницы", EsES = "Dirección URL de la página" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_View", EnEN = "View", PlPL = "Widok", DeDE = "Sicht", FrFR = "Voir", RuRU = "Вид", EsES = "Vista" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "PlayList_Add", EnEN = "Add Play List", PlPL = "Dodaj playlistę", DeDE = "Playlist hinzufügen", FrFR = "Ajouter une liste de lecture", RuRU = "Добавить плейлист", EsES = "Agregar lista de reproducción" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "PlayList_Delete", EnEN = "Delete Play List", PlPL = "Usuń playlistę", DeDE = "Wiedergabeliste löschen", FrFR = "Supprimer la liste de lecture", RuRU = "Удалить список воспроизведения", EsES = "Eliminar lista de reproducción" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "PlayList_Edit", EnEN = "Edit Play List", PlPL = "Edytuj playlistę", DeDE = "Playlist bearbeiten", FrFR = "Modifier la liste de lecture", RuRU = "Изменить плейлист", EsES = "Editar lista de reproducción" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_Agenda", EnEN = "Agenda", PlPL = "Program", DeDE = "Agenda", FrFR = "Ordre du jour", RuRU = "Повестка дня", EsES = "Agenda" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_AllDay", EnEN = "All day", PlPL = "Cały dzień", DeDE = "Den ganzen Tag", FrFR = "Toute la journée", RuRU = "Весь день", EsES = "Todo el dia" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_Day", EnEN = "Day", PlPL = "Dzień", DeDE = "Tag", FrFR = "journée", RuRU = "День", EsES = "Día" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_Event_End_Date", EnEN = "Event End Date", PlPL = "Data zakończenia wydarzenia", DeDE = "Enddatum der Veranstaltung", FrFR = "Date de fin de l'événement", RuRU = "Дата окончания мероприятия", EsES = "Fecha de finalización del evento" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_Event_Start_Date", EnEN = "Event Start Date", PlPL = "Data rozpoczęcia wydarzenia", DeDE = "Beginndatum der Veranstaltung", FrFR = "Date de début de l'événement", RuRU = "Дата начала мероприятия", EsES = "Fecha de inicio del evento" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_Event_Type", EnEN = "Event Type", PlPL = "Typ wydarzenia", DeDE = "Ereignistyp", FrFR = "Type d'événement", RuRU = "Тип события", EsES = "Tipo de evento" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_Grouping", EnEN = "Grouping calendars", PlPL = "Grupuj kalendarze", DeDE = "Gruppieren von Kalendern", FrFR = "Regroupement des calendriers", RuRU = "Группировка календарей", EsES = "Calendarios de agrupación" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_Localization", EnEN = "Event Localization ", PlPL = "Lokalizacja wydarzenia", DeDE = "Veranstaltungslokalisierung", FrFR = "Localisation d'événements", RuRU = "Локализация событий", EsES = "Localización de eventos" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_Month", EnEN = "Month", PlPL = "Miesiąc", DeDE = "Monat", FrFR = "Mois", RuRU = "Месяц", EsES = "Mes" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_MonthAgenda", EnEN = "Month Agenda", PlPL = "Agenda miesiąca", DeDE = "Monatsagenda", FrFR = "Agenda du mois", RuRU = "Повестка дня", EsES = "Agenda del mes" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_NewEvent", EnEN = "New Event", PlPL = "Nowe wydarzenie", DeDE = "Neues Event", FrFR = "Nouvel évènement", RuRU = "Новое событие", EsES = "Nuevo evento" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_Priority", EnEN = "Event Priority", PlPL = "Priorytet wydarzenia", DeDE = "Ereignispriorität", FrFR = "Priorité de l'événement", RuRU = "Приоритет события", EsES = "Prioridad del evento" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_Subject", EnEN = "Event Subject", PlPL = "Temat wydarzenia", DeDE = "Betreff der Veranstaltung", FrFR = "Objet de l'événement", RuRU = "Тема мероприятия", EsES = "Asunto del evento" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_Time_Zone", EnEN = "Select your time zone", PlPL = "Wybierz strefę czasową", DeDE = "Wählen Sie Ihre Zeitzone", FrFR = "Sélectionnez votre fuseau horaire", RuRU = "Выберите свой часовой пояс", EsES = "Seleccione su zona horaria" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_Timezone", EnEN = "Time zone", PlPL = "Strefa czasowa", DeDE = "Zeitzone", FrFR = "Fuseau horaire", RuRU = "Часовой пояс", EsES = "Zona horaria" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_Tooltip", EnEN = "Tooltip", PlPL = "Etykietka", DeDE = "Tooltip", FrFR = "Info-bulle", RuRU = "подсказка", EsES = "Información sobre herramientas" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_Week", EnEN = "Week", PlPL = "Tydzień", DeDE = "Woche", FrFR = "La semaine", RuRU = "Неделя", EsES = "Semana" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_WorkWeek", EnEN = "Work Week", PlPL = "Tydzień pracy", DeDE = "Arbeitswoche", FrFR = "Semaine de travail", RuRU = "Рабочая неделя", EsES = "Semana de trabajo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Scheduler_Year", EnEN = "Year", PlPL = "Rok", DeDE = "Jahr", FrFR = "Année", RuRU = "Год", EsES = "Año" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Tenant_Add_New", EnEN = "Add new Tenant", PlPL = "Dodaj nowego najemcę", DeDE = "Neuen Mieter hinzufügen", FrFR = "Ajouter un nouveau locataire", RuRU = "Добавить нового арендатора", EsES = "Agregar nuevo inquilino" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Tenant_Delete", EnEN = "Are you sure you want to remove this Tenant?", PlPL = "Czy napewno chcesz usuną tego Najemcę?", DeDE = "Sind Sie sicher, dass Sie diesen Mandanten entfernen möchten?", FrFR = "Voulez-vous vraiment supprimer ce locataire ?", RuRU = "Вы уверены, что хотите удалить этого арендатора?", EsES = "¿Está seguro de que desea eliminar este inquilino?" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Tenant_Desc", EnEN = "Tenant description", PlPL = "Opis najemcy", DeDE = "Mieterbeschreibung", FrFR = "Description du locataire", RuRU = "Описание арендатора", EsES = "Descripción del inquilino" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Tenant_Edit_Data", EnEN = "Edit Tenant data", PlPL = "Zmień dane najemcy", DeDE = "Mieterdaten bearbeiten", FrFR = "Modifier les données du locataire", RuRU = "Изменить данные арендатора", EsES = "Editar datos de arrendatario" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Tenant_Id", EnEN = "Tenant Id", PlPL = "Numer najemcy", DeDE = "Mieter-ID", FrFR = "Identifiant du locataire", RuRU = "Идентификатор арендатора", EsES = "Identificación del inquilino" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Tenant_Name", EnEN = "Tenant name", PlPL = "Nazwa najemcy", DeDE = "Name des Mieters", FrFR = "Nom du locataire", RuRU = "Имя арендатора", EsES = "Nombre del inquilino" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Tenant_New_Data", EnEN = "Insert new Tenant data", PlPL = "Wstaw nowe dane Najemcy", DeDE = "Fügen Sie neue Mieterdaten ein", FrFR = "Insérer de nouvelles données de locataire", RuRU = "Вставить новые данные арендатора", EsES = "Insertar nuevos datos de arrendatario" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Tenant_New_Tenant", EnEN = "New Tenant", PlPL = "Nowy najemca", DeDE = "Neuer Mieter", FrFR = "Nouveau locataire", RuRU = "Новый арендатор", EsES = "Nuevo inquilino" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "User_New_User", EnEN = "Create new user", PlPL = "Utwórz nowego użytkownika", DeDE = "Neuen Benutzer erstellen", FrFR = "Créer un nouvel utilisateur", RuRU = "Создать нового пользователя", EsES = "Crear nuevo usuario" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "User_role", EnEN = "Default Role", PlPL = "Domyślna rola", DeDE = "Standardrollen", FrFR = "Rôles par défaut", RuRU = "Роли по умолчанию", EsES = "Roles predeterminados" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Users_Create_New_User", EnEN = "Create new User", PlPL = "Utwórz nowego użytkownika", DeDE = "Neuen Benutzer erstellen", FrFR = "Créer un nouvel utilisateur", RuRU = "Создать нового пользователя", EsES = "Crear nuevo usuario" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Users_Delete_User_Data", EnEN = "Delete User data", PlPL = "Usuwanie danych użytkownika", DeDE = "Benutzerdaten löschen", FrFR = "Supprimer les données utilisateur", RuRU = "Удалить данные пользователя", EsES = "Eliminar datos de usuario" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Users_Edit_User_Data", EnEN = "Edit User data", PlPL = "Edytuj dane użytkownika", DeDE = "Benutzerdaten bearbeiten", FrFR = "Modifier les données utilisateur", RuRU = "Редактировать данные пользователя", EsES = "Editar datos de usuario" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Users_Email_Phone", EnEN = "Email / Phone", PlPL = "Email / Telefon", DeDE = "E-Mail / Telefon", FrFR = "Courriel / Téléphone", RuRU = "Электронная почта / Телефон", EsES = "Correo electrónico / Teléfono" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Users_Reset_User_Pass", EnEN = "Reset User Password", PlPL = "Resetowanie hasła użytkownika", DeDE = "Benutzerpasswort zurücksetzen", FrFR = "Réinitialiser le mot de passe utilisateur", RuRU = "Сбросить пароль пользователя", EsES = "Restablecer contraseña de usuario" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Yotube_Download_Link", EnEN = "    Download Link", PlPL = "    Pobierz link", DeDE = "    Download-Link", FrFR = "    Lien de téléchargement", RuRU = "    Ссылка для скачивания", EsES = "    Enlace de descarga" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Required_Value", EnEN = "This value is required. Please provide it.", PlPL = "Ta wartość jest wymagana. Proszę podać.", DeDE = "Dieser Wert ist erforderlich. Bitte geben Sie es an.", FrFR = "Cette valeur est obligatoire. Veuillez le fournir.", RuRU = "Это значение является обязательным. Пожалуйста, предоставьте его.", EsES = "Este valor es obligatorio. Por favor proporciónelo." });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "MenuName", EnEN = "Text_en_EN", PlPL = "Text_pl_PL", DeDE = "Text_de_DE", FrFR = "Text_fr_FR", RuRU = "Text_ru_RU", EsES = "Text_es_ES" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Home", EnEN = "Home", PlPL = "Strona domowa", DeDE = "Startseite", FrFR = "Page d'accueil", RuRU = "домашняя страница", EsES = "Página de inicio" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Application", EnEN = "Application", PlPL = "Aplikacje", DeDE = "Anwendung", FrFR = "Application", RuRU = "приложение", EsES = "Solicitud" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "System_Management", EnEN = "System Management", PlPL = "Administrowanie systemem", DeDE = "Systemmanagement", FrFR = "La gestion du système", RuRU = "yправление системой", EsES = "Gestión del sistema" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Authorization", EnEN = "Authorization", PlPL = "Autoryzacja", DeDE = "Genehmigung", FrFR = "Autorisation", RuRU = "авторизация", EsES = "Autorización" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "System", EnEN = "System", PlPL = "System", DeDE = "System", FrFR = "Système", RuRU = "cистема", EsES = "Sistema" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Users_Management", EnEN = "Users Management", PlPL = "Zarządzanie użytkownikami", DeDE = "Benutzerverwaltung", FrFR = "Gestion des utilisateurs", RuRU = "yправление пользователями", EsES = "Gestión de usuarios" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "User_Profile", EnEN = "User Profile", PlPL = "Profil użytkownika", DeDE = "Benutzerprofil", FrFR = "Profil de l'utilisateur", RuRU = "Профиль пользователя", EsES = "Perfil del usuario" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Role_Management", EnEN = "Role Management", PlPL = "Zarządzanie rolami", DeDE = "Rollenmanagement", FrFR = "Gestion des rôles", RuRU = "yправление ролями", EsES = "Gestión de roles" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Tenant_Management", EnEN = "Tenant Management", PlPL = "Zarządzanie najemcami", DeDE = "Mietermanagement", FrFR = "Gestion des locataires", RuRU = "yправление арендаторами", EsES = "Gestión de inquilinos" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Permission_Management", EnEN = "Permission Management", PlPL = "Zarządzanie uprawnieniami", DeDE = "Berechtigungsverwaltung", FrFR = "Gestion des autorisations", RuRU = "yправление разрешениями", EsES = "Gestión de permisos" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Localization_Management", EnEN = "Localization Management", PlPL = "Zarządzanie lokalizacją", DeDE = "Lokalisierungsmanagement", FrFR = "Gestion de la localisation", RuRU = "yправление локализацией", EsES = "Gestión de localización" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Audit_Trail", EnEN = "Audit Trail", PlPL = "Ścieżka audytu", DeDE = "Buchungskontrolle", FrFR = "Piste d'audit", RuRU = "Аудиторский след", EsES = "Pista de auditoría" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Loggs", EnEN = "Loggs", PlPL = "Logi", DeDE = "Protokolle", FrFR = "Journaux", RuRU = "Логгс", EsES = "Registros" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Dasboard_Panels", EnEN = "Dashboard Panels", PlPL = "Panele pulpitu", DeDE = "Dashboard-Panels", FrFR = "Panneaux du tableau de bord", RuRU = "Панели приборной панели", EsES = "Paneles de tablero" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Database_Manager", EnEN = "Database Manager", PlPL = "Menedżer bazy danych", DeDE = "Datenbankmanager", FrFR = "Gestionnaire de base de données", RuRU = "Менеджер базы данных", EsES = "Administrador de base de datos" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "App_Setting", EnEN = "Application Settings", PlPL = "Ustawienia aplikacji", DeDE = "Anwendungseinstellungen", FrFR = "Paramètres de l'application", RuRU = "Настройки приложения", EsES = "Configuración de la aplicación" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Organization_Management", EnEN = "Organization Manager", PlPL = "Menedżer organizacji", DeDE = "Organisationsmanager", FrFR = "Responsable de l'organisation", RuRU = "Менеджер организации", EsES = "Gerente de la organización" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Emails", EnEN = "Emails", PlPL = "Wiadomości email", DeDE = "E-Mails", FrFR = "E-mails", RuRU = "электронные письма", EsES = "Correos electrónicos" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Calendars", EnEN = "Calendars", PlPL = "Kalendarze", DeDE = "Kalender", FrFR = "Calendriers", RuRU = "Календари", EsES = "Calendarios" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Contacts", EnEN = "Contacts", PlPL = "Kontakty", DeDE = "Kontakte", FrFR = "Contacts", RuRU = "Контакты", EsES = "Contactos" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Youtube", EnEN = "Youtube Manager", PlPL = "Menedżer Youtube'a", DeDE = "Youtube-Manager", FrFR = "Gestionnaire YouTube", RuRU = "Ютуб-менеджер", EsES = "Administrador de Youtube" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "RadioNET", EnEN = "RadioNET", PlPL = "RadioNET", DeDE = "RadioNET", FrFR = "RadioNET", RuRU = "RadioNET", EsES = "RadioNET" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Document_Manager", EnEN = "Document Manager", PlPL = "Menedżer dokumentów", DeDE = "Dokumentenmanager", FrFR = "Gestionnaire de documents", RuRU = "Менеджер документов", EsES = "Administrador de documentos" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "User_Gender", EnEN = "Gender", PlPL = "Płeć", DeDE = "Geschlecht", FrFR = "Genre", RuRU = "Пол", EsES = "Género" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "User_Is_Active", EnEN = "Is Active", PlPL = "Aktywny", DeDE = "Ist aktiv", FrFR = "Est actif", RuRU = "Активен", EsES = "Está activo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "User_Lockout", EnEN = "Is Lockout", PlPL = "Zablokowany", DeDE = "Aussperrung", FrFR = "verrouillage", RuRU = "Блокировка", EsES = "Cierre patronal" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "User_EmailConfirmed", EnEN = "Email Confirmed", PlPL = "Mail potwierdzony", DeDE = "E-Mail bestätigt", FrFR = "E-mail confirmé", RuRU = "Электронная почта подтверждена", EsES = "Correo electrónico confirmado" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Audit_Audit_Type", EnEN = "Audit Type", PlPL = "Typ audtytu", DeDE = "Audit-Typ", FrFR = "Type d'audit", RuRU = "Тип аудита", EsES = "Tipo de auditoría" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Audit_Table_Name", EnEN = "Table Name", PlPL = "Nazwa tabeli", DeDE = "Tabellenname", FrFR = "Nom de la table", RuRU = "Название таблицы", EsES = "Nombre de la tabla" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Audit_Primary_Key", EnEN = "Primary Key", PlPL = "Klucz podstawowy", DeDE = "Primärschlüssel", FrFR = "Clé primaire", RuRU = "Первичный ключ", EsES = "Clave principal" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Audit_Created_Date_Time", EnEN = "Created Date Time", PlPL = "Data i godzina utworzenia", DeDE = "Erstellungsdatum/-zeit", FrFR = "Date et heure de création", RuRU = "Дата создания Время", EsES = "Fecha y hora de creación" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Global_Email_Value", EnEN = "This is not a valid email address. Please enter a valid address.", PlPL = "To nie jest prawidłowy adres email. Proszę podać poprawny adres.", DeDE = "Dies ist keine gültige E-Mail-Adresse. Bitte geben Sie eine gültige Adresse ein.", FrFR = "Cette adresse e-mail n'est pas valide. Veuillez saisir une adresse valide.", RuRU = "Это недействительный адрес электронной почты. Введите действительный адрес.", EsES = "Esta no es una dirección de correo electrónico válida. Por favor, introduzca una dirección válida." });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Emails_Manager", EnEN = "Emails_Manager", PlPL = "Klient Email", DeDE = "E-Mail-Client", FrFR = "Client de messagerie", RuRU = "Клиент электронной почты", EsES = "Cliente de correo electrónico" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Accounts_Manager", EnEN = "Account Manager", PlPL = "Menadżer kont", DeDE = "Account-Manager", FrFR = "Gestionnaire de compte", RuRU = "Менеджер по работе с клиентами", EsES = "Gerente de cuentas" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "ContextMenu_CopyToFolderHeader", EnEN = "Copy to folder", PlPL = "Kopiuj do folderu", DeDE = "In Ordner kopieren", FrFR = "Copier dans le dossier", RuRU = "Копировать в папку", EsES = "Copiar a la carpeta" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "ContextMenu_MoveToFolderHeader", EnEN = "Move to folder", PlPL = "Przenieś do folderu", DeDE = "In Ordner verschieben", FrFR = "Déplacer vers le dossier", RuRU = "Переместить в папку", EsES = "Mover a la carpeta" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Folders_Manager", EnEN = "Folders Manager", PlPL = "Menadżer folderów", DeDE = "Ordner-Manager", FrFR = "Gestionnaire de dossiers", RuRU = "Менеджер папок", EsES = "Administrador de carpetas" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Folders_Folder_Move", EnEN = "Folder - Move", PlPL = "Folder - Przenieś", DeDE = "Ordner - Verschieben", FrFR = "Dossier - Déplacer", RuRU = "Папка - Переместить", EsES = "Carpeta - Mover" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Folders_Folder_Copy", EnEN = "Folder - Copy", PlPL = "Folder - Kopiuj", DeDE = "Ordner - Kopieren", FrFR = "Dossier - Copie", RuRU = "Папка - Копировать", EsES = "Carpeta - Copiar" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Folders_CanEmpty", EnEN = "Can Empty", PlPL = "Można opróżnić", DeDE = "Kann entleert werden", FrFR = "Peut être vide", RuRU = "Может быть пустым", EsES = "Lata vacía" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Folders_CanDelete", EnEN = "Can Delete", PlPL = "Można usunąć", DeDE = "Kann löschen", FrFR = "Peut supprimer", RuRU = "Можно удалить", EsES = "Puede eliminar" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Folders_CanRename", EnEN = "Can Rename", PlPL = "Można zmieniąc nazwę", DeDE = "Kann umbenannt werden", FrFR = "Peut renommer", RuRU = "Можно переименовать", EsES = "Puede cambiar el nombre" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Folders_Unread", EnEN = "Unread", PlPL = "Nieprzeczytane", DeDE = "Ungelesen", FrFR = "Non lu", RuRU = "Непрочитано", EsES = "No leído" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Folders_All", EnEN = "All", PlPL = "Wszystkie", DeDE = "Alle", FrFR = "Tous", RuRU = "Все", EsES = "Todo" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Folders_MaxUid", EnEN = "Max UID", PlPL = "Max UID", DeDE = "Max UID", FrFR = "Max UID", RuRU = "Max UID", EsES = "Max UID" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Folders_EmailCount", EnEN = "Email Count", PlPL = "Ilość emaili", DeDE = "Anzahl der E-Mails", FrFR = "Nombre d'e-mails", RuRU = "Количество электронных писем", EsES = "Recuento de correos electrónicos" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Folders_Choice_Email", EnEN = "For further information, please contact email address.", PlPL = "Wybierz adres konta email", DeDE = "Wybierz-Adresse Konto E-Mail", FrFR = "Indiquez votre adresse e-mail", RuRU = "Для получения дополнительной информации обращайтесь по адресу электронной почты.", EsES = "Dirección de correo electrónico de Wybierz" });
            _context.Localizers.Add(new Domain.Entities.Localizes.Localizer() { Keytext = "Email_Save_Email", EnEN = "Save as", PlPL = "Zapisz jako", DeDE = "Speichern unter", FrFR = "Enregistrer sous", RuRU = "Сохранить как", EsES = "Guardar como" });


            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

    }
}

