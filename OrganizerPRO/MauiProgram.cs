using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using OrganizerPRO.Application;
using OrganizerPRO.Infrastructure;
using OrganizerPRO.Services;
using OrganizerPRO.Shared.Services;

namespace OrganizerPRO;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        
        builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
        
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        builder.Services.AddControllers();

        builder.Services
            .AddApplicationServices()
            .AddInfrastructureServices(builder.Configuration)
            .AddOrganizerPROServices(builder.Configuration);

        builder.Services.AddSingleton<IFormFactor, FormFactor>();

        builder.Services.AddMauiBlazorWebView();

#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif

        builder.Services.AddAuthorizationCore();

        return builder.Build();
    }
}
