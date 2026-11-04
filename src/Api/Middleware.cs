using Serilog;

namespace SmartDrone.Api;

public static class Middleware
{
    public static WebApplication UseAppMiddleware(this WebApplication app)
    {
        app.UseSerilogRequestLogging();

        app.Use(async (ctx, next) =>
        {
            ctx.Response.OnStarting(() =>
            {
                ctx.Response.Headers["X-Request-Id"] = ctx.TraceIdentifier;
                return Task.CompletedTask;
            });
            await next();
        });

        app.UseExceptionHandler();
        return app;
    }
}
