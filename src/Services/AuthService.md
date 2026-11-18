# AuthService.cs

Đăng ký, đăng nhập, xem và sửa hồ sơ cá nhân (v0.11). Không đụng trực tiếp ASP.NET Identity: mọi thao tác user đi qua `IUserRepository`, tạo token qua `ITokenService`.

| Hàm | Luồng | Lỗi |
|---|---|---|
| `RegisterAsync(email, password, fullName)` | `users.CreateAsync(..., Roles.Customer)` → tạo phiên đăng nhập luôn | Email trùng, mật khẩu < 8 ký tự... → 400 (`ValidationException` do `UserRepository` ném, nội dung là lỗi của Identity) |
| `LoginAsync(email, password)` | Tìm theo email → kiểm tra mật khẩu → kiểm tra `IsActive` → tạo phiên | Sai email **hoặc** mật khẩu đều báo chung "Email hoặc mật khẩu không đúng." (401) để không lộ email nào đã đăng ký; tài khoản khóa → 403 |
| `GetProfileAsync(userId)` (v0.11) | Đọc hồ sơ từ CSDL (có số điện thoại, tên mới nhất) + roles. Dùng cho `GET /auth/me` | Không có → 404 |
| `ChangePasswordAsync(userId, current, new)` (v0.11, UC-03) | Mật khẩu mới ≥ 8 ký tự và khác mật khẩu cũ → `UserManager.ChangePasswordAsync` (kiểm mật khẩu cũ, băm mật khẩu mới) | Sai mật khẩu cũ → 400 "Mật khẩu hiện tại không đúng." |
| `UpdateProfileAsync(userId, fullName, phone)` (v0.11, FR01) | Kiểm tra qua `ProfileRules` → `users.UpdateProfileAsync`. Chỉ đổi họ tên, số điện thoại; email và vai trò không tự đổi được | Họ tên < 2 hoặc > 100 ký tự, số điện thoại sai dạng → 400 |

`CreateSessionAsync` lấy roles, gọi `ITokenService.Create` → `AuthSession` (token, hạn, user, roles). Controller đổi sang `AuthResponse`.

Tự đăng ký chỉ ra **Customer**. Tài khoản nhân viên lấy từ seed (`Infrastructure/Databases/DbSeeder.cs`) hoặc do Admin tạo ở trang Tài khoản (`UsersController`, có từ v0.9).

Token chứa claim `sub` (id), `email`, `name`, `role` (mỗi role một claim), hạn `Jwt:ExpiryMinutes` phút (mặc định 60). Sửa hồ sơ không cấp token mới, nên claim `name` giữ tên cũ tới lần đăng nhập sau; web và app hiển thị tên lấy từ `GET /auth/me` hoặc kết quả `PUT /auth/me`.
