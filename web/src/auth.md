# auth.jsx

Giữ trạng thái đăng nhập cho cả web bằng React Context, và quy định role nào mở được trang nào.

## Role → trang

| Trang | Đường dẫn | Role |
|---|---|---|
| Điều phối | `/dispatch` | Dispatcher, Manager, Admin |
| Trạm | `/station` | StationOperator, Dispatcher, Manager, Admin |
| Khách hàng | `/customers` | Dispatcher, Manager, Admin (v0.11) |
| Nhật ký | `/audit` | Admin (v0.11) |
| Thống kê | `/dashboard` | Manager, Admin |
| Drone & trạm | `/fleet` | Manager, Admin |
| Tài khoản | `/users` | Admin |

Bảng này khớp với `[Authorize(Roles = ...)]` trong `OrdersController` và `ReportsController`: web chỉ ẩn trang cho gọn, chặn thật vẫn là server (gọi sai role → 403).

- `pagesFor(roles)`: các trang tài khoản được mở, dùng để vẽ menu.
- `canOpen(path, roles)`: `App.jsx` dùng để chặn khi gõ thẳng URL.
- `homePath(roles)`: trang mở đầu tiên sau đăng nhập. Manager/Admin vào Thống kê, còn lại vào trang đầu tiên được phép.

## AuthProvider

| Giá trị | Ý nghĩa |
|---|---|
| `auth` | Thông tin trả về từ `/auth/login` hoặc `null` nếu chưa đăng nhập. Giá trị đầu lấy từ `localStorage` nên F5 không mất |
| `login(email, password)` | Gọi API; tài khoản không có trang nào (Customer) thì ném lỗi "dùng app mobile" và **không** lưu token |
| `logout()` | Xóa token, `auth = null`, các trang tự đá về `/login` |

Khi `api.js` gặp 401 nó gọi lại handler đăng ký ở đây để set `auth = null`.

Trang nào cần biết ai đang đăng nhập thì gọi `const { auth } = useAuth()`.

## updateProfile (v0.11)

`updateProfile(profile)` trong context: sau khi `PUT /auth/me` thành công, ghi tên và số điện thoại mới vào `auth` (và `localStorage`) để thanh menu đổi ngay. Token giữ nguyên, nên claim `name` trong token vẫn là tên cũ tới lần đăng nhập sau; web không đọc claim đó nên không ảnh hưởng.

