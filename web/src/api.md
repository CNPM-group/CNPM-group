# api.js

Chỗ duy nhất trong web gọi backend. Trang nào cần dữ liệu thì `import { api } from '../api'` rồi gọi `api.xxx()`, không tự viết `fetch`.

## Token

- Đăng nhập xong, cả object trả về từ `/auth/login` (`token`, `expiresAtUtc`, `fullName`, `roles`...) được lưu vào `localStorage` khóa `sd_auth` (`saveAuth`).
- `loadAuth()` đọc lại và tự xóa nếu token đã hết hạn, nên F5 không bị văng ra nếu token còn hạn.
- Mỗi request gắn header `Authorization: Bearer <token>`.
- Server trả **401** (token sai hoặc hết hạn) ở bất kỳ API nào trừ `/auth/...` → xóa token, gọi `onUnauthorized` để `auth.jsx` đưa về trang đăng nhập.

## Lỗi

`request()` ném `ApiError` có `message` đọc được ngay:

| Server trả | Câu hiện ra |
|---|---|
| 400 có `errors` (sai dữ liệu) | Gộp các câu lỗi của từng trường |
| 4xx/5xx có `detail` (ProblemDetails) | `detail`, ví dụ "Mã nhận hàng không đúng." |
| Chỉ có `title` | `title`, ví dụ "Xung đột trạng thái" |
| 403 không có body | "Bạn không có quyền thực hiện thao tác này." |
| Không kết nối được | "Không kết nối được máy chủ. Backend đã chạy chưa?" |

## Danh sách hàm

| Hàm | API | Ai gọi được |
|---|---|---|
| `login(email, password)` | `POST /auth/login` | Mọi người |
| `orders({ status, page, pageSize })` | `GET /orders?...` | Nhân viên thấy mọi đơn |
| `order(id)` | `GET /orders/{id}` (có kiện hàng, tên trạm) | Nhân viên |
| `history(id)` | `GET /orders/{id}/history` | Nhân viên |
| `approve(id, note)` / `reject(id, reason)` | `POST .../approve`, `.../reject` | Dispatcher, Manager, Admin |
| `schedule(id, droneId)` | `POST .../schedule` | Dispatcher, Manager, Admin |
| `start(id)` / `fail(id, reason)` | `POST .../start`, `.../fail` | Dispatcher, Manager, Admin |
| `returnOrder(id, reason)` | `POST .../return` (hủy và hoàn đơn Failed) | Dispatcher, Manager, Admin |
| `packageImage(id)` | `GET .../package-image`, trả `Blob` | Nhân viên |
| `arrive(id)` / `confirm(id, otp)` | `POST .../arrive`, `.../confirm` | + Station Operator |
| `drones()` | `GET /drones` | Nhân viên |
| `createDrone`, `updateDrone`, `deleteDrone` | `POST/PUT/DELETE /drones` | Manager, Admin |
| `stations()`, `createStation`, `updateStation`, `deleteStation` | `GET/POST/PUT/DELETE /stations` | Xem: mọi người; sửa: Manager, Admin |
| `tracking(id)` | `GET /orders/{id}/tracking` (vị trí drone, % đã bay) | Nhân viên |
| `aiSummary(id)` | `POST /orders/{id}/ai-summary` (AI tóm tắt chuyến giao, v0.10) | Nhân viên |
| `users`, `createUser`, `setRoles`, `setActive` | `GET/POST /users`, `PUT /users/{id}/roles`, `PUT /users/{id}/active` | Admin |
| `report()` / `reportCsv()` | `GET /reports/orders`, `.../orders.csv` | Manager, Admin |
| `reportStations()` | `GET /reports/stations` (hiệu quả theo trạm, v0.11) | Manager, Admin |
| `aiInsights()` | `GET /reports/ai-insights` (AI nhận xét vận hành, v0.10) | Manager, Admin |

`reportCsv()` trả `Blob` vì file CSV cần gửi kèm token, không mở bằng link thường được.

## Địa chỉ API

`API_ROOT` lấy từ biến `VITE_API_URL` lúc build, mặc định `http://localhost:5080`. Vite chỉ đưa biến bắt đầu bằng `VITE_` vào code chạy trên trình duyệt.

## Thêm ở v0.11

| Hàm | Gọi | Ai dùng |
|---|---|---|
| `me()`, `updateMe(body)` | `GET` / `PUT /auth/me` | Mọi nhân viên (hồ sơ) |
| `changePassword(current, next)` | `POST /auth/me/password` | Mọi nhân viên |
| `auditLogs({ entity, page, pageSize })` | `GET /audit-logs` | Admin |
| `customers(q)`, `customer(id)`, `updateCustomer(id, body)` | `/customers` | Dispatcher, Manager, Admin (sửa: Manager, Admin) |
| `stationLoad()` | `GET /stations/load` | Trang Drone & trạm |
| `orders({ status, q, pageSize })` | `q` = tìm theo mã đơn | Trang Điều phối |

