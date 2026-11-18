# web/src/pages

Mỗi file là một màn hình, mỗi màn có file `.md` riêng giải thích.

| File | Màn | Ai dùng | Giải thích |
|---|---|---|---|
| `Login.jsx` | Đăng nhập | Mọi nhân viên | [Login.md](Login.md) |
| `Dispatcher.jsx` | Điều phối đơn hàng | Dispatcher, Manager, Admin | [Dispatcher.md](Dispatcher.md) |
| `Station.jsx` | Nhân viên trạm | Station Operator (+ Dispatcher, Manager, Admin) | [Station.md](Station.md) |
| `Dashboard.jsx` | Thống kê vận hành | Manager, Admin | [Dashboard.md](Dashboard.md) |
| `Fleet.jsx` | Quản lý drone & trạm | Manager, Admin | [Fleet.md](Fleet.md) |
| `Users.jsx` | Quản lý tài khoản | Admin | [Users.md](Users.md) |
| `Customers.jsx` | Khách hàng (v0.11) | Dispatcher, Manager, Admin | [Customers.md](Customers.md) |
| `Audit.jsx` | Nhật ký thao tác (v0.11) | Admin | [Audit.md](Audit.md) |

Khuôn chung của 3 trang sau đăng nhập:

```js
const load = useCallback(async () => { /* gọi api, setState */ }, [/* bộ lọc */]);
useEffect(() => { load(); }, [load]);   // mở trang
const live = useOrderEvents(() => load()); // có đơn đổi trạng thái
```

Thao tác thành công → hiện thông báo xanh góc phải (`useToast().ok`) rồi `load()` lại. Lỗi → thông báo đỏ với câu lỗi server trả về.
