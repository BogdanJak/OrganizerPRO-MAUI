using Microsoft.Extensions.DependencyInjection;

namespace OrganizerPRO.Application;

public static class ApplicationServices
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddAutoMapper(config => {
            config.AddMaps(Assembly.GetExecutingAssembly());
            config.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzg0MzMyODAwIiwiaWF0IjoiMTc1MjgxNzc5NiIsImFjY291bnRfaWQiOiIwMTk4MWMxNDU0ZTk3YTc0YjIwYzRhZTk0YzU0OWU0MyIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazBlMWExanlrcjViNnExM2d0cDM5Yzk1Iiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.YE489y8lgE96pGZ1hp_rrau4dJaEHkI_5mit8Pv-8HmlB47eAMDHhHVBxd_k1yGDN89r8dqneWKd3xL9SV73cBfMMrSLqnPiArCPDG9Q7fUONcU6Xq807xszDIsOQjVZ0a3Q2qBs_7qEcVclhG7ir8AUGSjEURbJKz9qSvX6UkWsUi6En7sfSSSXpE_pc7c9aZA2dXVI8n_lSa_8V8zCpxN8YQl3NvUpf_6RLOSBe6J6Ehch3-w_ooACIh6gZ8GVDKiB_7qISUS8PkNnYq_ougm3ce_0MXUGSMS5d_ZhDLQtLb6v36unu-rnZNBGTrvb6FE1j1NcDqCc0WB5Rwfjdw";
        });
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            config.NotificationPublisherType = typeof(ChannelBasedNoWaitPublisher);
            config.AddRequestPreProcessor(typeof(IRequestPreProcessor<>), typeof(ValidationPreProcessor<>));
            config.AddOpenBehavior(typeof(PerformanceBehaviour<,>));
            config.AddOpenBehavior(typeof(FusionCacheBehaviour<,>));
            config.AddOpenBehavior(typeof(CacheInvalidationBehaviour<,>));
            config.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxNzg0MzMyODAwIiwiaWF0IjoiMTc1MjgxNzc5NiIsImFjY291bnRfaWQiOiIwMTk4MWMxNDU0ZTk3YTc0YjIwYzRhZTk0YzU0OWU0MyIsImN1c3RvbWVyX2lkIjoiY3RtXzAxazBlMWExanlrcjViNnExM2d0cDM5Yzk1Iiwic3ViX2lkIjoiLSIsImVkaXRpb24iOiIwIiwidHlwZSI6IjIifQ.YE489y8lgE96pGZ1hp_rrau4dJaEHkI_5mit8Pv-8HmlB47eAMDHhHVBxd_k1yGDN89r8dqneWKd3xL9SV73cBfMMrSLqnPiArCPDG9Q7fUONcU6Xq807xszDIsOQjVZ0a3Q2qBs_7qEcVclhG7ir8AUGSjEURbJKz9qSvX6UkWsUi6En7sfSSSXpE_pc7c9aZA2dXVI8n_lSa_8V8zCpxN8YQl3NvUpf_6RLOSBe6J6Ehch3-w_ooACIh6gZ8GVDKiB_7qISUS8PkNnYq_ougm3ce_0MXUGSMS5d_ZhDLQtLb6v36unu-rnZNBGTrvb6FE1j1NcDqCc0WB5Rwfjdw";
        });

        return services;
    }
}
