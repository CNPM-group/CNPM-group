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
- Phân quyền bằng `[Authorize(Roles = ...)]`, hằng số role nằm trong `src/Domain/Constants.cs`.

## Code
- Làm đúng 4 tầng: `Api` → `Services` → `Domain` (interface) ← `Infrastructure`. Xem [clean-architecture.md](clean-architecture.md).
- Controller không chứa nghiệp vụ, không gọi `AppDbContext`. Service không gọi `AppDbContext`, chỉ gọi interface `I*Repository`.
- Sai luật nghiệp vụ thì ném exception trong `Domain/Exceptions.cs`, không tự `return BadRequest(...)` trong controller.
- Comment trong code ít thôi; giải thích để ở `README.md` của thư mục hoặc file `.md` cùng tên.
- Sửa/thêm file nào thì cập nhật `README.md` của thư mục đó trong cùng PR.

## Thêm một module mới
Làm theo mục 6 của [clean-architecture.md](clean-architecture.md).
