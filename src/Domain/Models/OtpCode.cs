using System.Security.Cryptography;
using System.Text;

namespace SmartDrone.Domain.Models;

public static class OtpCode
{
    public const int MaxFailedAttempts = 5;
    public static readonly TimeSpan Lifetime = TimeSpan.FromHours(24);

    public static string Generate() =>
        RandomNumberGenerator.GetInt32(0, 1_000_000).ToString("D6");

    public static string Hash(Guid orderId, string code) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes($"{orderId:N}:{code.Trim()}")));
}
