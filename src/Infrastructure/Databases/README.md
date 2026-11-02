# Infrastructure/Databases

| File | Tác dụng |
|---|---|
| `AppDbContext.cs` | "Cổng" vào PostgreSQL. Kế thừa `IdentityDbContext` nên có sẵn bảng user/role. Khai báo `DbSet` cho 15 bảng nghiệp vụ, đổi tên bảng Identity sang snake_case (`asp_net_roles`...), map `AppUser` vào bảng `users` theo `docs/schema.sql`, rồi nạp toàn bộ cấu hình bảng trong `Infrastructure/Models` bằng `ApplyConfigurationsFromAssembly`. Chi tiết: [AppDbContext.md](AppDbContext.md) |
| `DbSeeder.cs` | Chạy khi API khởi động ở Development: áp migration (`MigrateAsync`), tạo 5 role, 5 trạm quanh khuôn viên trường, 20 tuyến bay có hướng giữa 5 trạm (mỗi tuyến 1 km), 3 drone DJI Matrice 350 RTK (tải 2.5 kg, tầm 15 km, 40 km/h), 5 tài khoản demo `<role>@drone.local` / `Seed:DemoPassword`. Chạy lại nhiều lần không tạo trùng. |

Connection string lấy từ `ConnectionStrings:Default` (đăng ký ở `src/DependencyContainer.cs`). Muốn đổi sang SQL Server / MySQL: đổi `UseNpgsql` sang provider khác + tạo lại migration, không phải sửa Services.

Muốn dữ liệu sạch: `docker compose down -v` (xóa volume) rồi chạy lại API.
