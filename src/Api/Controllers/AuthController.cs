using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDrone.Api.Schemas;
using SmartDrone.Domain;
using SmartDrone.Services;

namespace SmartDrone.Api.Controllers;

[ApiController]
[Route(ApiConstants.Prefix + "/auth")]
public class AuthController(AuthService auth) : ControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request) =>
        Ok(AuthResponse.From(await auth.RegisterAsync(request.Email, request.Password, request.FullName)));

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request) =>
        Ok(AuthResponse.From(await auth.LoginAsync(request.Email, request.Password)));

    [HttpGet("me")]
    [Authorize]
    public ActionResult<MeResponse> Me() => Ok(new MeResponse(
        User.FindFirst("sub")?.Value,
        User.FindFirst("email")?.Value,
        User.FindFirst("name")?.Value,
        User.FindAll("role").Select(c => c.Value).ToList()));
}
