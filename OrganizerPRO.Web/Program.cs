var builder = WebApplication.CreateBuilder(args);

builder.RegisterSerilog();

builder.Services
            .AddApplicationServices()
            .AddInfrastructureServices(builder.Configuration)
            .AddOrganizerPROServices(builder.Configuration);

builder.Services.AddSingleton<IFormFactor, FormFactor>();

var app = builder.Build();

app.ConfigureServer(builder.Configuration);

await app.InitializeDatabaseAsync().ConfigureAwait(false);

app.InitializeCacheFactory();

await app.RunAsync().ConfigureAwait(false);
