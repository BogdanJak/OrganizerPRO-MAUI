using OrganizerPRO.Application.Common.Interfaces;

namespace OrganizerPRO.Web.Services;


public static class OrganizerPROServices
{
    public static IServiceCollection AddOrganizerPROServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddRazorComponents().AddInteractiveServerComponents().AddHubOptions(options => options.MaximumReceiveMessageSize = 64 * 1024);
        services.AddCascadingAuthenticationState();
        services.AddServerSideBlazor().AddHubOptions(o => { o.MaximumReceiveMessageSize = 102400000; });

        services.AddControllers();

        services.AddScoped<IApplicationHubWrapper, ServerHubWrapper>()
            .AddSignalR(options =>
            {
                options.MaximumReceiveMessageSize = 64 * 1024;
                options.AddFilter<UserContextHubFilter>();
            });

        services.AddScoped<LocalizationCookiesMiddleware>()
            .Configure<RequestLocalizationOptions>(options =>
            {
                options.AddSupportedUICultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
                options.AddSupportedCultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());
                options.FallBackToParentUICultures = true;
            })
            .AddLocalization(options => options.ResourcesPath = LocalizationConstants.ResourcesPath);

        services.AddExceptionHandler<GlobalExceptionHandler>();
        services.AddProblemDetails();
        services.AddHealthChecks();

        services.AddScoped<LocalTimeOffset>();
        services.AddScoped<HubClient>();

        services
            .AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>()
            .AddScoped<IPermissionHelper, PermissionHelper>()
            .AddScoped<UserPermissionAssignmentService>()
            .AddScoped<RolePermissionAssignmentService>()
            .AddScoped<BlazorDownloadFileService>()
            .AddScoped<InMemoryNotificationService>()
            .AddScoped<INotificationService>(sp =>
            {
                var service = sp.GetRequiredService<InMemoryNotificationService>();
                service.Preload();
                return service;
            });

        return services;
    }

    public static WebApplication ConfigureServer(this WebApplication app, IConfiguration config)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseExceptionHandler("/Error", true);
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseStatusCodePagesWithRedirects("/404");
        app.MapHealthChecks("/health");
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseAntiforgery();
        app.UseHttpsRedirection();
        app.MapStaticAssets();


        if (!Directory.Exists(Path.Combine(Directory.GetCurrentDirectory(), @"Files")))
            Directory.CreateDirectory(Path.Combine(Directory.GetCurrentDirectory(), @"Files"));



        app.UseStaticFiles(new StaticFileOptions
        {
            FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), @"Files")),
            RequestPath = new PathString("/Files")
        });

        var localizationOptions = new RequestLocalizationOptions()
            .SetDefaultCulture(LocalizationConstants.DefaultLanguageCode)
            .AddSupportedCultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray())
            .AddSupportedUICultures(LocalizationConstants.SupportedLanguages.Select(x => x.Code).ToArray());

        // Remove AcceptLanguageHeaderRequestCultureProvider to prevent the browser's Accept-Language header from taking effect
        var acceptLanguageProvider = localizationOptions.RequestCultureProviders
            .OfType<AcceptLanguageHeaderRequestCultureProvider>()
            .FirstOrDefault();
        if (acceptLanguageProvider != null)
        {
            localizationOptions.RequestCultureProviders.Remove(acceptLanguageProvider);
        }
        app.UseRequestLocalization(localizationOptions);
        app.UseMiddleware<LocalizationCookiesMiddleware>();
        app.UseExceptionHandler();

        app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
        app.MapHub<ServerHub>(ISignalRHub.Url);


        app.MapAdditionalIdentityEndpoints();
        app.UseForwardedHeaders();
        app.UseWebSockets(new Microsoft.AspNetCore.Builder.WebSocketOptions()
        {
            KeepAliveInterval = TimeSpan.FromSeconds(30),
        });

        return app;
    }
}


