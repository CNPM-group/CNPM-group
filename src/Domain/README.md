# Domain

Lõi nghiệp vụ (tương đương `src/domain` của repo mẫu). Chỉ có class C# thuần: **không** `using` EF Core, ASP.NET hay PostgreSQL. Tầng nào cũng được dùng Domain, Domain không dùng tầng nào.

| File / thư mục | Tác dụng |
|---|---|
| `Constants.cs` | Hằng số dùng chung, xem bảng dưới |
| `Exceptions.cs` | Lỗi nghiệp vụ. Service `throw`, `ErrorHandler.cs` đổi ra HTTP status (bảng ở [../ErrorHandler.md](../ErrorHandler.md)) |
| `Models/` | Model nghiệp vụ + interface repository/dịch vụ. Xem [Models/README.md](Models/README.md) |

## Constants.cs

| Tên | Giá trị | Dùng ở |
|---|---|---|
| `ApiConstants.Prefix` | `api/v1` | `[Route]` của mọi controller |
| `ApiConstants.DefaultPageSize` / `MaxPageSize` | 20 / 100 | Phân trang danh sách đơn |
| `ApiConstants.MaxPromptLength` | 2000 | Giới hạn độ dài câu hỏi gửi AI |
| `Roles.Customer` ... `Roles.Admin` | 5 role | `[Authorize(Roles = ...)]`, seed role |
| `Roles.AdminOrManager` | `"Admin,Manager"` | Endpoint quản lý trạm |
| `Roles.Staff` | Dispatcher, Manager, Admin | `User.IsStaff()`: được xem mọi đơn |
| `OrderStatus` | Pending, Approved, Rejected, Scheduled, InTransit, ArrivedAtStation, Completed, Failed, Cancelled | Trạng thái đơn (lưu dạng chuỗi, CSDL có check constraint cùng danh sách) |
| `StationStatus` | Available, Occupied, Maintenance | Trạng thái trạm |
| `DroneStatus` | Idle, Assigned, InFlight, Charging, Maintenance | Trạng thái drone |

Thêm trạng thái mới phải sửa **cả** enum ở đây **và** check constraint trong `Infrastructure/Models/*Configuration.cs`, rồi tạo migration.

## Exceptions.cs

`DomainException` là lớp cha. Các lớp con: `NotFoundException`, `ValidationException`, `UnauthorizedException`, `ForbiddenException`, `ConflictException`, `ExternalServiceException` (có thêm `StatusCode`). Message viết tiếng Việt vì được trả thẳng cho người dùng ở trường `detail`.
