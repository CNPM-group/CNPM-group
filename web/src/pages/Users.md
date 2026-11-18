# Users.jsx

Trang **Tài khoản** (`/users`) – chỉ **Admin** mở được (v0.9). Gọi các API `UsersController` (đều yêu cầu role Admin).

| Thao tác | API | Ghi chú |
|---|---|---|
| Xem danh sách | `GET /users` | Email, họ tên, vai trò (nhãn tiếng Việt), ngày tạo, đang dùng / đã khóa. Ô tìm lọc theo email hoặc tên ngay trên trình duyệt |
| + Tạo tài khoản | `POST /users { email, password, fullName, role }` | Mật khẩu ≥ 8 ký tự, chọn 1 vai trò. Dùng để tạo tài khoản nhân viên (khách tự đăng ký trên app) |
| Đổi vai trò | `PUT /users/{id}/roles { roles: [...] }` | Tick nhiều vai trò, phải còn ít nhất 1 |
| Khóa / Mở khóa | `PUT /users/{id}/active { isActive }` | Tài khoản bị khóa không đăng nhập được |

Dòng của chính Admin đang đăng nhập bị khóa 2 nút (không tự khóa, không tự đổi vai trò). Server chặn 2 việc nguy hiểm nhất: tự khóa mình và tự gỡ quyền Admin của mình (`UserService` trả 409). Web khóa luôn cả nút đổi vai trò ở hàng mình cho chắc.
