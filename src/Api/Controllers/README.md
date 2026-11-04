# Api/Controllers

Mỗi controller là một nhóm endpoint (tương đương một Blueprint của Flask). Prefix chung `/api/v1` lấy từ `ApiConstants.Prefix`. Controller chỉ làm 3 việc: đổi schema → model, gọi service, đổi model → response schema. Lỗi nghiệp vụ do service ném, `ErrorHandler` lo phần trả mã lỗi.

Tất cả endpoint cần JWT trừ `register` và `login`.

## AuthController.cs – `/api/v1/auth`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| POST | `/register` | Ai cũng được | Tạo tài khoản, luôn là role `Customer` (role khác do Admin cấp). Trả token luôn, khỏi login lại |
| POST | `/login` | Ai cũng được | Đúng email/mật khẩu thì trả `AuthResponse` (token, hạn, id, email, tên, roles). Sai → 401 |
| GET | `/me` | Đã đăng nhập | Đọc thông tin từ token đang gửi lên (debug phân quyền) |

## StationsController.cs – `/api/v1/stations`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/` | Đã đăng nhập | Danh sách trạm, sắp theo tên |
| GET | `/{id}` | Đã đăng nhập | Chi tiết trạm, không có → 404 |
| POST | `/` | Admin, Manager | Thêm trạm, trả 201 + header `Location` |
| PUT | `/{id}` | Admin, Manager | Sửa tên, tọa độ, trạng thái |
| DELETE | `/{id}` | Admin, Manager | Xóa trạm, trả 204 |

## OrdersController.cs – `/api/v1/orders`

Chi tiết từng endpoint: [OrdersController.md](OrdersController.md)

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/eta?originStationId=&destinationStationId=&weightKg=` | Đã đăng nhập | Ước tính khoảng cách (km) và thời gian giao (phút) trước khi tạo đơn |
| POST | `/` | Customer | Tạo đơn + kiện hàng, trạng thái `Pending` |
| GET | `/?page=&pageSize=&status=` | Đã đăng nhập | Phân trang. Khách thấy đơn của mình, nhân viên thấy tất cả |
| GET | `/{id}` | Đã đăng nhập | Chi tiết đơn |
| PUT | `/{id}` | Customer | Sửa đơn khi còn `Pending` |
| POST | `/{id}/cancel` | Customer | Hủy khi còn `Pending` |
| POST | `/{id}/approve` | Dispatcher, Manager, Admin | Duyệt đơn `Pending` → `Approved`, body `{ "note" }` không bắt buộc |
| POST | `/{id}/reject` | Dispatcher, Manager, Admin | Từ chối đơn `Pending` → `Rejected`, bắt buộc `{ "reason" }` |
| GET | `/{id}/history` | Đã đăng nhập | Lịch sử chuyển trạng thái (timeline) |
| POST | `/{id}/schedule` | Dispatcher, Manager, Admin | Gán drone + giờ bay: `Approved`/`Failed` → `Scheduled`. Body `{ "droneId", "scheduledAtUtc"? }` |
| POST | `/{id}/start` | Dispatcher, Manager, Admin | Cất cánh: `Scheduled` → `InTransit` |
| POST | `/{id}/arrive` | StationOperator, Dispatcher, Manager, Admin | Hạ cánh tại trạm nhận: `InTransit` → `ArrivedAtStation`, sinh OTP gửi vào thông báo của khách |
| POST | `/{id}/confirm` | StationOperator, Dispatcher, Manager, Admin | Nhập OTP khách đọc: `ArrivedAtStation` → `Completed`. Body `{ "otp": "123456" }` |
| POST | `/{id}/fail` | Dispatcher, Manager, Admin | Giao thất bại: `InTransit`/`ArrivedAtStation` → `Failed`. Body `{ "reason" }` |

Luồng giao trọn vẹn để demo: `POST /orders` (khách) → `approve` → `schedule` → `start` → `arrive` → khách xem OTP ở `GET /notifications` → `confirm`. Chi tiết: [../../Services/DeliveryService.md](../../Services/DeliveryService.md)

## DronesController.cs – `/api/v1/drones`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/` | Dispatcher, StationOperator, Manager, Admin | Danh sách drone, sắp theo mã |
| GET | `/{id}` | như trên | Chi tiết drone |
| POST | `/` | Admin, Manager | Thêm drone. Trùng mã → 409, trạm hiện tại không tồn tại → 400 |
| PUT | `/{id}` | Admin, Manager | Sửa drone |
| DELETE | `/{id}` | Admin, Manager | Xóa. Drone đang `Assigned`/`InFlight` → 409 |

## NotificationsController.cs – `/api/v1/notifications`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/?unreadOnly=` | Đã đăng nhập | Thông báo của chính mình, mới nhất trước (tối đa 100). Khách lấy mã OTP nhận hàng ở đây |
| POST | `/{id}/read` | Đã đăng nhập | Đánh dấu đã đọc |

## ReportsController.cs – `/api/v1/reports`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| GET | `/orders?from=&to=` | Admin, Manager | Tổng đơn, số hoàn thành/thất bại, tỷ lệ thành công, thời gian giao trung bình (phút), số đơn theo trạng thái và theo ngày. Mặc định 30 ngày gần nhất |
| GET | `/orders.csv?from=&to=` | Admin, Manager | Tải file CSV danh sách đơn (UTF-8, mở được bằng Excel) |

## UsersController.cs – `/api/v1/users`

Chỉ Admin.

| Method | Path | Mô tả |
|---|---|---|
| GET | `/` | Danh sách tài khoản kèm vai trò |
| POST | `/` | Tạo tài khoản nội bộ `{ email, password, fullName, role }` |
| PUT | `/{id}/roles` | Gán lại vai trò `{ "roles": ["Dispatcher"] }`. Không tự gỡ quyền Admin của mình |
| PUT | `/{id}/active` | Khóa/mở tài khoản `{ "isActive": false }`. Tài khoản bị khóa đăng nhập sẽ nhận 403. Không tự khóa mình |

## CustomerAddressesController.cs – `/api/v1/customers/me/addresses`

Chỉ role `Customer`, mọi thao tác chỉ trên địa chỉ của chính mình (`User.UserId()`).

| Method | Path | Mô tả |
|---|---|---|
| GET | `/` | Danh sách địa chỉ, mới nhất trước |
| GET | `/{id}` | Chi tiết |
| POST | `/` | Thêm. Đánh dấu mặc định khi đã có địa chỉ mặc định khác → 409 |
| PUT | `/{id}` | Sửa |
| DELETE | `/{id}` | Xóa, trả 204 |

## AiController.cs – `/api/v1/ai`

| Method | Path | Quyền | Mô tả |
|---|---|---|---|
| POST | `/feasibility-test` | Đã đăng nhập | Gửi 1 prompt tới LLM, trả câu trả lời + độ trễ + token usage (dùng cho báo cáo KAN-18) |
| POST | `/chat` | Đã đăng nhập | Chatbot hỗ trợ khách, được gọi tool tra đơn / trạm / ETA. Khách chỉ tra được đơn của mình |

Chưa cấu hình `Llm__ApiKey` thì hai endpoint này trả 503.
