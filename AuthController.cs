using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDrone.Application.Abstractions;
using SmartDrone.Application.Auth;

namespace SmartDrone.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public class AuthController(IAuthService auth) : ControllerBase
{
    /// <summary>Đăng ký tài khoản Customer.</summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        var result = await auth.RegisterAsync(request, ct);
        if (result.Succeeded) return Ok(result.Data);

        return BadRequest(new ValidationProblemDetails(
            new Dictionary<string, string[]> { ["auth"] = result.Errors.ToArray() }));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var result = await auth.LoginAsync(request, ct);
        if (result.Succeeded) return Ok(result.Data);

        return Unauthorized(new ProblemDetails
        {
            Status = StatusCodes.Status401Unauthorized,
            Title = "Đăng nhập thất bại",
            Detail = result.Errors.FirstOrDefault()
        });
    }

    /// <summary>Xem token đang đăng nhập là ai, role gì (dùng để debug phân quyền).</summary>
    [HttpGet("me")]
    [Authorize]
    public IActionResult Me() => Ok(new
    {
        Id = User.FindFirst("sub")?.Value,
        Email = User.FindFirst("email")?.Value,
        Name = User.FindFirst("name")?.Value,
        Roles = User.FindAll("role").Select(c => c.Value).ToArray()
    });
}
