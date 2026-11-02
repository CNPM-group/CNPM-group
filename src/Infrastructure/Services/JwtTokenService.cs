using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using SmartDrone.Domain.Models;

namespace SmartDrone.Infrastructure.Services;

public sealed class JwtTokenService(IOptions<JwtOptions> options) : ITokenService
{
    private readonly JwtOptions _opt = options.Value;

    public (string Token, DateTime ExpiresAtUtc) Create(User user, IEnumerable<string> roles)
    {
        var expires = DateTime.UtcNow.AddMinutes(_opt.ExpiryMinutes);

        var claims = new List<Claim>
        {
            new("sub", user.Id.ToString()),
            new("email", user.Email),
            new("name", user.FullName)
        };
        claims.AddRange(roles.Select(r => new Claim("role", r)));

        var descriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Issuer = _opt.Issuer,
            Audience = _opt.Audience,
            Expires = expires,
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_opt.Key)),
                SecurityAlgorithms.HmacSha256)
        };

        return (new JsonWebTokenHandler().CreateToken(descriptor), expires);
    }
}
