namespace SmartDrone;

public static class Cors
{
    public const string Policy = "web";

    public static IServiceCollection AddAppCors(this IServiceCollection services, IConfiguration config)
    {
        var origins = config.GetSection("Cors:Origins").Get<string[]>() ?? new[] { "http://localhost:5173" };

        return services.AddCors(o => o.AddPolicy(Policy, p => p
            .WithOrigins(origins)
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()));
    }
}
