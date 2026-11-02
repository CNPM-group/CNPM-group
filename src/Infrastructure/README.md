# Infrastructure

Phần nói chuyện với thế giới bên ngoài (tương đương `src/infrastructure` của repo mẫu): PostgreSQL qua EF Core, ASP.NET Identity, JWT, LLM API. Tầng này **cài đặt** các interface khai báo ở `Domain/Models`. Đổi CSDL hay đổi nhà cung cấp LLM chỉ sửa ở đây, Services không phải đổi gì.

| Thư mục | Tương đương Flask | Tác dụng | Xem |
|---|---|---|---|
| `Databases/` | `infrastructure/databases` | `AppDbContext` (kết nối PostgreSQL), `DbSeeder` (migrate + dữ liệu mẫu) | [Databases/README.md](Databases/README.md) |
| `Models/` | `infrastructure/models` (SQLAlchemy) | Ánh xạ model Domain ↔ bảng (Fluent API), `AppUser`/`AppRole` của Identity | [Models/README.md](Models/README.md) |
| `Repositories/` | `infrastructure/repositories` | Cài đặt `I*Repository` bằng EF Core | [Repositories/README.md](Repositories/README.md) |
| `Services/` | `infrastructure/services` | Dịch vụ dùng thư viện/bên thứ ba: tạo JWT, gọi LLM | [Services/README.md](Services/README.md) |
