# Audit.jsx

Trang **Nhật ký** (`/audit`, v0.11, UC-29) chỉ cho Admin: ai đã thêm / sửa / xóa dữ liệu gì, lúc nào.

- Lấy từ `GET /audit-logs?entity=&page=&pageSize=50`, mới nhất trước, có nút trang trước / sau.
- Ô chọn lọc theo loại dữ liệu: Đơn hàng, Kiện hàng, Drone, Chuyến bay, Trạm, Xác nhận OTP, Địa chỉ, Tài khoản, Vai trò tài khoản.
- Cột **Thay đổi**:
  - Sửa: mỗi trường đổi 1 dòng `Trường: cũ → mới` (ví dụ `Status: Pending → Approved`).
  - Thêm / xóa: 4 trường đầu cho gọn (bỏ `Id` và các cột giờ).
  - Giá trị dài hơn 60 ký tự bị cắt, trống hiện `∅`.

Nhật ký do server tự ghi khi lưu (xem `src/Infrastructure/Databases/AuditInterceptor.cs`), web chỉ đọc. Không có cột mật khẩu hay mã OTP trong nhật ký: interceptor bỏ các cột băm trước khi ghi.
