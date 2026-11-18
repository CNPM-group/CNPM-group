# Customers.jsx

Trang **Khách hàng** (`/customers`, v0.11) cho Dispatcher, Manager, Admin. Đáp ứng FR03 trong đề: "thêm, xem, cập nhật thông tin khách hàng; quản lý địa chỉ; tra cứu lịch sử đơn".

## Danh sách

`GET /customers?q=`: họ tên, email, điện thoại, tổng đơn, hoàn tất, đang xử lý, ngày đặt gần nhất. Ô tìm lọc theo tên / email / số điện thoại, gõ xong 300 ms mới gọi API. Tài khoản khách bị khóa hiện mờ.

## Chi tiết khách (`CustomerDialog`)

Bấm **Chi tiết** → `GET /customers/{id}`:

- Họ tên, số điện thoại (ô sửa được nếu là Manager/Admin, Dispatcher chỉ xem).
- Địa chỉ đã lưu của khách, địa chỉ mặc định có nhãn.
- 20 đơn gần nhất: mã, tuyến, trạng thái, ngày tạo.

Manager/Admin bấm **Lưu** → `PUT /customers/{id} { fullName, phone }`. Server kiểm họ tên 2–100 ký tự, số điện thoại 8–15 chữ số; sai thì hiện câu lỗi trong hộp thoại.

## Không làm ở đây

- Tạo tài khoản khách: khách tự đăng ký trên app (`/auth/register`).
- Khóa tài khoản, đổi vai trò: trang Tài khoản của Admin.
- Sửa địa chỉ thay khách: địa chỉ là của khách, khách tự quản trên app.
