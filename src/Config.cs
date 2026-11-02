namespace SmartDrone;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string Key { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
}

public sealed class LlmOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int TimeoutSeconds { get; set; } = 30;
}

public static class Config
{
    public const string AuthCookie = "smartdrone_auth";

    public static JwtOptions AddAppConfig(this WebApplicationBuilder builder)
    {
        var config = builder.Configuration;

        builder.Services.Configure<JwtOptions>(config.GetSection("Jwt"));
        builder.Services.Configure<LlmOptions>(config.GetSection("Llm"));

        var jwt = config.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
        if (string.IsNullOrWhiteSpace(jwt.Key) || jwt.Key.Length < 32)
            throw new InvalidOperationException("Jwt:Key phải >= 32 ký tự (đặt qua biến môi trường Jwt__Key).");

        if (string.IsNullOrWhiteSpace(config.GetConnectionString("Default")))
            throw new InvalidOperationException("Thiếu ConnectionStrings:Default (đặt qua biến môi trường ConnectionStrings__Default).");

        return jwt;
    }
}
