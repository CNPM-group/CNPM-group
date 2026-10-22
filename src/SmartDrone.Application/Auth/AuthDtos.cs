
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDrone.Application.Abstractions;
using SmartDrone.Application.Auth;

namespace SmartDrone.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(IAuthService auth) : ControllerBase
{
    private const string TokenCookieName = "smartdrone_auth";

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        RegisterRequest request,
        CancellationToken ct)
    {
        var result = await auth.RegisterAsync(request, ct);

        if (!result.Succeeded || result.Data is null)
        {
            return BadRequest(new
            {
                errors = result.Errors
            });
        }

        SetAuthCookie(result.Data);

        // Không trả JWT trong JSON khi dùng cookie HttpOnly.
        return Ok(new
        {
            result.Data.UserId,
            result.Data.Email,
            result.Data.FullName,
            result.Data.Roles,
            result.Data.ExpiresAtUtc
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        LoginRequest request,
        CancellationToken ct)
    {
        var result = await auth.LoginAsync(request, ct);

        if (!result.Succeeded || result.Data is null)
        {
            return Unauthorized(new
            {
                message = "Email hoặc mật khẩu không đúng."
            });
        }

        SetAuthCookie(result.Data);

        return Ok(new
        {
            result.Data.UserId,
            result.Data.Email,
            result.Data.FullName,
            result.Data.Roles,
            result.Data.ExpiresAtUtc
        });
    }

    [HttpGet("me")]
    [Authorize]
    public IActionResult Me() => Ok(new
    {
        Id = User.FindFirst("sub")?.Value,
        Email = User.FindFirst("email")?.Value,
        Name = User.FindFirst("name")?.Value,
        Roles = User.FindAll("role").Select(c => c.Value).ToArray()
    });

    [HttpPost("logout")]
    [AllowAnonymous]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(TokenCookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = !HttpContext.Request.Host.Host.Equals(
                "localhost", StringComparison.OrdinalIgnoreCase),
            SameSite = SameSiteMode.Lax,
            Path = "/"
        });

        return Ok(new { message = "Đã đăng xuất." });
    }

    private void SetAuthCookie(AuthResponse data)
    {
        Response.Cookies.Append(TokenCookieName, data.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = !HttpContext.Request.Host.Host.Equals(
                "localhost", StringComparison.OrdinalIgnoreCase),
            SameSite = SameSiteMode.Lax,
            Expires = new DateTimeOffset(data.ExpiresAtUtc),
            IsEssential = true,
            Path = "/"
        });
    }
}
