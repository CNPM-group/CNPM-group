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

    // v0.11: đọc từ CSDL (có số điện thoại, tên mới nhất) thay vì đọc lại claim trong token.
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ProfileResponse>> Me() =>
        Ok(ProfileResponse.From(await auth.GetProfileAsync(User.UserId())));

    // FR01: tự sửa họ tên, số điện thoại. Token cũ vẫn mang tên cũ tới khi đăng nhập lại.
    [HttpPut("me")]
    [Authorize]
    public async Task<ActionResult<ProfileResponse>> UpdateMe(ProfileRequest request) =>
        Ok(ProfileResponse.From(await auth.UpdateProfileAsync(User.UserId(), request.FullName, request.Phone)));

    // UC-03 (v0.11): đổi mật khẩu.
    [HttpPost("me/password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
    {
        await auth.ChangePasswordAsync(User.UserId(), request.CurrentPassword, request.NewPassword);
        return NoContent();
    }
}
