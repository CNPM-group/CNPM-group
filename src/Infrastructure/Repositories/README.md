# Infrastructure/Repositories

Cài đặt các interface `I*Repository` của `Domain/Models` bằng EF Core (tương đương `infrastructure/repositories` bên Flask, ví dụ `TodoRepository(ITodoRepository)`). Repository là nơi **duy nhất** viết câu truy vấn LINQ/SQL.

Mọi repository nhận cùng một `AppDbContext` (scoped theo request). Hàm `Add/Update/Delete` tự `SaveChangesAsync`.

| File | Interface | Ghi chú |
|---|---|---|
| `UserRepository.cs` | `IUserRepository` | Bọc `UserManager<AppUser>` của Identity. Có `ToDomain` đổi `AppUser` → `User` (ánh xạ tay giống repo mẫu). Tạo user lỗi (email trùng, mật khẩu yếu) → `ValidationException` gom lỗi của Identity. v0.11: `ListInRoleAsync` dùng `GetUsersInRoleAsync`, `UpdateProfileAsync` đổi `FullName` + `PhoneNumber`. |
| `StationRepository.cs` | `IStationRepository` | List dùng `AsNoTracking` (chỉ đọc, nhanh hơn); `GetByIdAsync` để tracking nên sửa xong chỉ cần `UpdateAsync`. `DeleteAsync` bắt `DbUpdateException` (khóa ngoại) → `ConflictException` 409 (v0.10). `LoadAsync` (v0.11): 3 truy vấn `GROUP BY` đếm đơn đang tới, chờ nhận, sắp đi theo trạm. |
| `OrderRepository.cs` | `IOrderRepository` | Chi tiết: [OrderRepository.md](OrderRepository.md) |
| `CustomerAddressRepository.cs` | `ICustomerAddressRepository` | Mọi truy vấn lọc theo `customerId`. `HasOtherDefaultAsync(customerId, exceptId)` bỏ qua chính địa chỉ đang sửa. |
| `AuditLogRepository.cs` | `IAuditLogRepository` | (v0.11) Phân trang `audit_logs` mới nhất trước, lọc theo `entity_name`, lấy kèm email người làm bằng truy vấn con vào bảng `users`. |
| `DeliveryRepository.cs` | `IDeliveryRepository` | `AddFlight`/`AddConfirmation` chỉ đưa vào DbContext, **không** tự lưu: `DeliveryService` gọi `IOrderRepository.UpdateAsync` ở cuối để đơn, lịch sử, chuyến bay, OTP, thông báo được lưu chung một transaction. |
| `NotificationRepository.cs` | `INotificationRepository` | `Add` cũng chỉ đưa vào DbContext; `SaveAsync` để lưu khi đánh dấu đã đọc. |
| `ReportRepository.cs` | `IReportRepository` | Truy vấn thống kê chỉ đọc (`AsNoTracking`); tên trạm và cân nặng lấy bằng subquery trong cùng câu SQL. |
| `DroneRepository.cs` | `IDroneRepository` | CRUD drone, `CodeExistsAsync(code, exceptId)` kiểm tra trùng mã (bỏ qua chính drone đang sửa), `AnyCanCarryAsync(kg)`: có drone nào `MaxPayloadKg ≥ kg`. `DeleteAsync`: drone đã có chuyến bay trong lịch sử → 409 thay vì 500 (v0.10). |

## Viết repository mới

1. Khai báo interface ở `Domain/Models/IXxxRepository.cs` (chỉ dùng kiểu của Domain, không lộ `IQueryable` hay `DbSet`).
2. Tạo `XxxRepository(AppDbContext db) : IXxxRepository` ở đây.
3. Đăng ký `services.AddScoped<IXxxRepository, XxxRepository>()` trong `src/DependencyContainer.cs`.
