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
| POST | `/` | Customer | Tạo đơn + kiện hàng, trạng thái `Pending` |
| GET | `/?page=&pageSize=&status=` | Đã đăng nhập | Phân trang. Khách thấy đơn của mình, nhân viên thấy tất cả |
| GET | `/{id}` | Đã đăng nhập | Chi tiết đơn |
| PUT | `/{id}` | Customer | Sửa đơn khi còn `Pending` |
| POST | `/{id}/cancel` | Customer | Hủy khi còn `Pending` |
| GET | `/{id}/history` | Đã đăng nhập | Lịch sử chuyển trạng thái (timeline) |

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
