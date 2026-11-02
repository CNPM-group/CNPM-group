# AuthService.cs

Đăng ký, đăng nhập. Không đụng trực tiếp ASP.NET Identity: mọi thao tác user đi qua `IUserRepository`, tạo token qua `ITokenService`.

| Hàm | Luồng | Lỗi |
|---|---|---|
| `RegisterAsync(email, password, fullName)` | `users.CreateAsync(..., Roles.Customer)` → tạo phiên đăng nhập luôn | Email trùng, mật khẩu < 8 ký tự... → 400 (`ValidationException` do `UserRepository` ném, nội dung là lỗi của Identity) |
| `LoginAsync(email, password)` | Tìm theo email → kiểm tra mật khẩu → kiểm tra `IsActive` → tạo phiên | Sai email **hoặc** mật khẩu đều báo chung "Email hoặc mật khẩu không đúng." (401) để không lộ email nào đã đăng ký; tài khoản khóa → 403 |

`CreateSessionAsync` lấy roles, gọi `ITokenService.Create` → `AuthSession` (token, hạn, user, roles). Controller đổi sang `AuthResponse`.

Tự đăng ký chỉ ra **Customer**. Tài khoản nhân viên lấy từ seed (`Infrastructure/Databases/DbSeeder.cs`) hoặc do Admin cấp (sprint sau).

Token chứa claim `sub` (id), `email`, `name`, `role` (mỗi role một claim), hạn `Jwt:ExpiryMinutes` phút (mặc định 60).
