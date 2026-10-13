# Quy ước chung

## Git
- `main` bị khóa, mọi thứ qua Pull Request, tối thiểu 1 người review, CI xanh mới merge.
- Tên nhánh: `feature/<module>-<mô-tả>`, `fix/<mô-tả>`, `docs/<mô-tả>`.
- Commit: `feat: ...`, `fix: ...`, `docs: ...`, `refactor: ...`, `test: ...`.
- Ai commit thì phải giải thích được từng dòng mình commit (kể cả code do AI sinh).

## Bí mật
- Không commit `.env`, password, API key, JWT key. Prod đặt qua biến môi trường (`Jwt__Key`, `ConnectionStrings__Default`).

## API
- Prefix `/api/v1/...`, tên resource số nhiều, chữ thường: `/stations`, `/orders`.
- Lỗi trả về dạng ProblemDetails (RFC 7807).
- Enum trả về dạng chuỗi (`"Available"`), thời gian dạng UTC ISO-8601.
- Phân quyền bằng `[Authorize(Roles = ...)]`, hằng số role nằm trong `Roles.cs`.

## Thêm một module mới (làm theo mẫu Stations)
1. Entity trong `Domain/Entities`, enum trong `Domain/Enums`.
2. Thêm `DbSet` vào `IAppDbContext` và `AppDbContext`, tạo `IEntityTypeConfiguration` trong `Persistence/Configurations`.
3. DTO + `IXxxService` + `XxxService` trong `Application/Xxx`, đăng ký trong `Application/DependencyInjection.cs`.
4. `XxxController` trong `Api/Controllers`.
5. `dotnet ef migrations add <Tên>` rồi commit migration.
