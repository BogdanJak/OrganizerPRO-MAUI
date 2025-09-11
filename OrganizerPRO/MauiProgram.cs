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
       
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });


        builder.Services.AddSingleton<IFormFactor, FormFactor>();


#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
        builder.Logging.AddDebug();
#endif
        builder.Services.AddMauiBlazorWebView();

        return builder.Build();
    }
}
