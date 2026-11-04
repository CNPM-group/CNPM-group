# ErrorHandler.cs

Tương đương `error_handler.py`. Service chỉ việc `throw`, không cần controller bọc `try/catch`.

## Bảng đổi exception → HTTP

| Exception (`Domain/Exceptions.cs`) | HTTP | Ví dụ |
|---|---|---|
| `NotFoundException` | 404 | Đơn không tồn tại hoặc không phải của khách đang đăng nhập |
| `ValidationException` | 400 | Trạm gửi trùng trạm nhận; kiện vượt tải drone; email đã dùng |
| `UnauthorizedException` | 401 | Sai email/mật khẩu |
| `ForbiddenException` | 403 | Tài khoản bị khóa |
| `ConflictException` | 409 | Hủy/sửa đơn không còn `Pending`; đã có địa chỉ mặc định |
| `ExternalServiceException` | 502/503/504 | LLM API lỗi, chưa cấu hình, timeout |
| exception khác | 500 | Lỗi chưa lường trước: ghi log đầy đủ, **không** trả chi tiết ra ngoài |

## Response mẫu

```json
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.10",
  "title": "Xung đột trạng thái",
  "status": 409,
  "detail": "Chỉ được hủy đơn hàng đang ở trạng thái Pending."
}
```

Web/mobile hiển thị trường `detail`. Lỗi validate tự động của `[ApiController]` (thiếu trường, sai `[Range]`...) có thêm `errors` theo từng trường, format ở `Api/Responses.cs`.
