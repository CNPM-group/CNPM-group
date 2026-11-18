using System.Text.RegularExpressions;
using SmartDrone.Domain;

namespace SmartDrone.Services;

// Luật chung khi sửa họ tên / số điện thoại (hồ sơ cá nhân và nhân viên sửa thông tin khách).
internal static partial class ProfileRules
{
    public static string Name(string? fullName)
    {
        var name = fullName?.Trim() ?? "";
        if (name.Length is < 2 or > 100)
            throw new ValidationException("Họ tên phải từ 2 đến 100 ký tự.");
        return name;
    }

    public static string? Phone(string? phone)
    {
        var value = phone?.Trim();
        if (string.IsNullOrEmpty(value))
            return null;
        if (!PhonePattern().IsMatch(value))
            throw new ValidationException("Số điện thoại không hợp lệ (8–15 chữ số, có thể bắt đầu bằng +).");
        return value;
    }

    [GeneratedRegex(@"^\+?[0-9 .]{8,15}$")]
    private static partial Regex PhonePattern();
}
