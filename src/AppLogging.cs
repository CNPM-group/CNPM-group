using Serilog;

namespace SmartDrone;

public static class AppLogging
{
    public static WebApplicationBuilder AddAppLogging(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((ctx, cfg) => cfg
            .ReadFrom.Configuration(ctx.Configuration)
            .Enrich.FromLogContext()
            .WriteTo.Console());

        return builder;
    }
}
