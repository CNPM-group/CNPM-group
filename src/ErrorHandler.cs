using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SmartDrone.Domain;

namespace SmartDrone;

public sealed class ErrorHandler(IProblemDetailsService problemDetails, ILogger<ErrorHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext http, Exception exception, CancellationToken ct)
    {
        var (status, title) = exception switch
        {
            NotFoundException => (StatusCodes.Status404NotFound, "Không tìm thấy"),
            ValidationException => (StatusCodes.Status400BadRequest, "Dữ liệu không hợp lệ"),
            UnauthorizedException => (StatusCodes.Status401Unauthorized, "Chưa xác thực"),
            ForbiddenException => (StatusCodes.Status403Forbidden, "Không có quyền"),
            ConflictException => (StatusCodes.Status409Conflict, "Xung đột trạng thái"),
            ExternalServiceException e => (e.StatusCode, "Lỗi dịch vụ bên ngoài"),
            _ => (StatusCodes.Status500InternalServerError, "Lỗi hệ thống")
        };

        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled exception at {Path}", http.Request.Path);

        http.Response.StatusCode = status;

        return await problemDetails.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = http,
            Exception = exception,
            ProblemDetails = new ProblemDetails
            {
                Status = status,
                Title = title,
                Detail = exception is DomainException ? exception.Message : null
            }
        });
    }
}
