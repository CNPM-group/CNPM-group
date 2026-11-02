using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using SmartDrone.Api;
using SmartDrone.Infrastructure.Databases;

namespace SmartDrone;

public static class CreateApp
{
    public static async Task<WebApplication> BuildAsync(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.AddAppLogging();
        var jwt = builder.AddAppConfig();

        builder.Services.AddDependencies(builder.Configuration);
        builder.Services.AddJwtAuthentication(jwt);
        builder.Services.AddAppCors(builder.Configuration);
        builder.Services.AddControllers().AddApiResponses();
        builder.Services.AddProblemDetails();
        builder.Services.AddExceptionHandler<ErrorHandler>();
        builder.Services.AddHealthChecks().AddDbContextCheck<AppDbContext>();
        builder.Services.AddAppSwagger();

        var app = builder.Build();

        app.UseAppMiddleware();

        if (app.Environment.IsDevelopment())
        {
            app.UseAppSwagger();

            var isEfTooling = Assembly.GetEntryAssembly()?.GetName().Name == "ef";
            if (!isEfTooling)
                await DbSeeder.SeedAsync(app.Services, app.Configuration);
        }

        app.UseCors(Cors.Policy);
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();
        app.MapHealthChecks("/health");

        return app;
    }

    private static IServiceCollection AddJwtAuthentication(this IServiceCollection services, JwtOptions jwt)
    {
        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(o =>
            {
                o.MapInboundClaims = false;
                o.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidIssuer = jwt.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwt.Audience,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.Key)),
                    RoleClaimType = "role",
                    NameClaimType = "name",
                    ClockSkew = TimeSpan.FromSeconds(30)
                };
                o.Events = new JwtBearerEvents
                {
                    OnMessageReceived = ctx =>
                    {
                        if (ctx.Request.Cookies.TryGetValue(Config.AuthCookie, out var token))
                            ctx.Token = token;
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();
        return services;
    }
}
