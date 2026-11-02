# Infrastructure/Repositories

Cài đặt các interface `I*Repository` của `Domain/Models` bằng EF Core (tương đương `infrastructure/repositories` bên Flask, ví dụ `TodoRepository(ITodoRepository)`). Repository là nơi **duy nhất** viết câu truy vấn LINQ/SQL.

Mọi repository nhận cùng một `AppDbContext` (scoped theo request). Hàm `Add/Update/Delete` tự `SaveChangesAsync`.

| File | Interface | Ghi chú |
|---|---|---|
| `UserRepository.cs` | `IUserRepository` | Bọc `UserManager<AppUser>` của Identity. Có `ToDomain` đổi `AppUser` → `User` (ánh xạ tay giống repo mẫu). Tạo user lỗi (email trùng, mật khẩu yếu) → `ValidationException` gom lỗi của Identity. |
| `StationRepository.cs` | `IStationRepository` | List dùng `AsNoTracking` (chỉ đọc, nhanh hơn); `GetByIdAsync` để tracking nên sửa xong chỉ cần `UpdateAsync`. |
| `OrderRepository.cs` | `IOrderRepository` | Chi tiết: [OrderRepository.md](OrderRepository.md) |
| `CustomerAddressRepository.cs` | `ICustomerAddressRepository` | Mọi truy vấn lọc theo `customerId`. `HasOtherDefaultAsync(customerId, exceptId)` bỏ qua chính địa chỉ đang sửa. |
| `DroneRepository.cs` | `IDroneRepository` | `AnyCanCarryAsync(kg)`: có drone nào `MaxPayloadKg ≥ kg`. |

## Viết repository mới

1. Khai báo interface ở `Domain/Models/IXxxRepository.cs` (chỉ dùng kiểu của Domain, không lộ `IQueryable` hay `DbSet`).
2. Tạo `XxxRepository(AppDbContext db) : IXxxRepository` ở đây.
3. Đăng ký `services.AddScoped<IXxxRepository, XxxRepository>()` trong `src/DependencyContainer.cs`.
