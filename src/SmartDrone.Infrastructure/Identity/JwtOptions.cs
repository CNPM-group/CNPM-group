namespace SmartDrone.Infrastructure.Identity;

public sealed class JwtOptions
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    /// <summary>Tối thiểu 32 ký tự. Prod đặt qua biến môi trường Jwt__Key, KHÔNG commit.</summary>
    public string Key { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
}
