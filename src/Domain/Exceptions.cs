namespace SmartDrone.Domain;

public abstract class DomainException(string message) : Exception(message);

public sealed class NotFoundException(string message = "Không tìm thấy dữ liệu.") : DomainException(message);

public sealed class ValidationException(string message = "Dữ liệu không hợp lệ.") : DomainException(message);

public sealed class UnauthorizedException(string message = "Chưa đăng nhập hoặc phiên đã hết hạn.") : DomainException(message);

public sealed class ForbiddenException(string message = "Không có quyền thực hiện.") : DomainException(message);

public sealed class ConflictException(string message = "Trạng thái hiện tại không cho phép thao tác này.") : DomainException(message);

public sealed class ExternalServiceException(string message, int statusCode = 502) : DomainException(message)
{
    public int StatusCode { get; } = statusCode;
}
