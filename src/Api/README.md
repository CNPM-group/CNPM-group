# Api

Tầng giao tiếp HTTP (tương đương `src/api` của repo mẫu). Nhận request, kiểm tra dữ liệu, gọi service, trả JSON. **Không chứa nghiệp vụ, không gọi CSDL.**

| File / thư mục | Tương đương Flask | Tác dụng |
|---|---|---|
| `Controllers/` | `api/controllers` | Endpoint REST. Xem [Controllers/README.md](Controllers/README.md) |
| `Schemas/` | `api/schemas` (Marshmallow) | Request/response, validate bằng attribute. Xem [Schemas/README.md](Schemas/README.md) |
| `Middleware.cs` | `api/middleware.py` | `UseAppMiddleware()`: (1) Serilog ghi mỗi request một dòng: method, path, status, thời gian; (2) gắn header `X-Request-Id` vào mọi response để tra log khi có lỗi; (3) `UseExceptionHandler()` chuyển exception cho `ErrorHandler.cs`. |
| `Requests.cs` | `api/requests.py` | Hàm mở rộng trên `User` (người đang đăng nhập): `User.UserId()` lấy id từ claim `sub` (không có thì 401); `User.IsStaff()` là Dispatcher/Manager/Admin; `User.CustomerScope()` trả `null` cho nhân viên (xem mọi đơn) hoặc id khách (chỉ xem đơn của mình). |
| `Responses.cs` | `api/responses.py` | `AddApiResponses()`: enum trả dạng chuỗi (`"Pending"` chứ không phải `0`); lỗi validate trả `400` dạng `ValidationProblemDetails` với title "Dữ liệu không hợp lệ" và `errors` theo từng trường. |
| `Swagger.cs` | `api/swagger.py` | `AddAppSwagger()` tạo tài liệu OpenAPI từ controller + nút **Authorize** để dán JWT; `UseAppSwagger()` mở UI ở `/swagger` (chỉ bật ở Development). |

## Thử API bằng Swagger

1. Chạy API, mở http://localhost:5080/swagger
2. `POST /api/v1/auth/login` với `{"email":"customer@drone.local","password":"Demo@12345"}`
3. Copy `token`, bấm **Authorize**, dán vào (không gõ chữ `Bearer`)
4. Gọi các endpoint khác
