# App.jsx

Khai báo đường dẫn của web.

| Đường dẫn | Trang | Ghi chú |
|---|---|---|
| `/login` | `Login` | Đã đăng nhập thì tự chuyển về trang chính |
| `/dispatch` | `Dispatcher` | Bọc trong `Guard` |
| `/station` | `Station` | Bọc trong `Guard` |
| `/dashboard` | `Dashboard` | Bọc trong `Guard` |
| `/fleet` | `Fleet` | Bọc trong `Guard` |
| `/users` | `Users` | Bọc trong `Guard` |
| `/customers` | `Customers` | Bọc trong `Guard` (v0.11) |
| `/audit` | `Audit` | Bọc trong `Guard` (v0.11) |
| Còn lại | – | Đã đăng nhập → trang chính theo role, chưa → `/login` |

## Guard

Chạy trước khi vẽ trang:

1. Chưa đăng nhập → `/login`.
2. Role không được mở trang này (ví dụ nhân viên trạm gõ `/dashboard`) → về trang chính của role đó.
3. Được phép → vẽ trang bên trong `Layout` (thanh menu trên cùng).

Đây chỉ là chặn ở giao diện. Nếu ai đó gọi API trực tiếp, server vẫn chặn bằng `[Authorize(Roles = ...)]`.
