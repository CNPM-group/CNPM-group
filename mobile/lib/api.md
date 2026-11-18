# api.dart

Lớp `Api.instance` – chỗ duy nhất trong app gọi backend, dùng thư viện Dio.

## Chung cho mọi request

- Interceptor tự gắn `Authorization: Bearer <token>`.
- Gặp **401** ở API không phải đăng nhập → đăng xuất, về màn đăng nhập.
- Timeout: kết nối 10 giây, chờ trả lời 30 giây (riêng chatbot 60 giây).
- `_guard(...)` bọc mọi hàm: lỗi gì cũng đổi thành `ApiException` có câu tiếng Việt (lấy `errors` / `detail` trong ProblemDetails của server).

## Danh sách hàm

| Hàm | API | Thêm ở |
|---|---|---|
| `login`, `register` | `POST /auth/login`, `/auth/register` | v0.5 |
| `getStations` | `GET /stations` | v0.5 |
| `getOrders`, `getOrder`, `getHistory` | `GET /orders`, `/orders/{id}`, `/orders/{id}/history` | v0.5 |
| `createOrder` (trả id), `cancelOrder` | `POST /orders`, `/orders/{id}/cancel` | v0.5, sửa v0.7 |
| `uploadImage` | `POST /packages/images` (multipart) | v0.5, backend có từ v0.8 |
| `getNotifications`, `markRead` | `GET /notifications`, `POST /notifications/{id}/read` | v0.7 |
| `chat` | `POST /ai/chat` | v0.8 |
| `getAddresses`, `saveAddress`, `deleteAddress` | `/customers/me/addresses` | v0.8 |
| `estimateEta` | `GET /orders/eta` | v0.8 |
| `getTracking` | `GET /orders/{id}/tracking` | v0.9 |
| `getAiSummary` | `POST /orders/{id}/ai-summary`, chờ tối đa 60 giây | v0.10 |
| `getOrders(search:)` | thêm query `q` tìm theo mã đơn | v0.11 |
| `getMe`, `updateMe` | `GET` / `PUT /auth/me` (hồ sơ) | v0.11 |
| `changePassword` | `POST /auth/me/password` | v0.11 |
| `updateOrder` | `PUT /orders/{id}` (sửa đơn Pending) | v0.11 |
