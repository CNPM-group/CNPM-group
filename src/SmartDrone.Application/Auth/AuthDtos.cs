
namespace SmartDrone.Application.Auth;

// Dữ liệu đăng ký tài khoản
public record RegisterRequest(
    string Email,
    string Password,
    string FullName
);

// Dữ liệu đăng nhập
public record LoginRequest(
    string Email,
    string Password
);

// Kết quả xác thực thành công
public record AuthResponse(
    string Token,
    DateTime ExpiresAtUtc,
    Guid UserId,
    string Email,
    string FullName,
    List<string> Roles
);

// Kết quả xử lý đăng ký / đăng nhập
public class AuthResult
{
    public bool Succeeded { get; init; }

    public AuthResponse? Data { get; init; }

    public List<string> Errors { get; init; } = new();

    public static AuthResult Ok(AuthResponse data)
    {
        return new AuthResult
        {
            Succeeded = true,
            Data = data
        };
    }

    public static AuthResult Fail(string error)
    {
        return Fail(new[] { error });
    }

    public static AuthResult Fail(IEnumerable<string> errors)
    {
        return new AuthResult
        {
            Succeeded = false,
            Errors = errors.ToList()
        };
    }
}
