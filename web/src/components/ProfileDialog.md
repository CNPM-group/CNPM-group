# ProfileDialog.jsx

Hộp thoại **Hồ sơ của tôi** (v0.11, FR01). Mở bằng cách bấm tên mình trên thanh menu (`Layout.jsx`).

1. Mở lên gọi `GET /auth/me` lấy email, họ tên, số điện thoại mới nhất.
2. Sửa họ tên, số điện thoại (email không đổi được, ô bị khóa).
3. **Lưu** → `PUT /auth/me { fullName, phone }`. Số điện thoại để trống gửi `null`.
4. **Đổi mật khẩu** (UC-03, không bắt buộc): điền mật khẩu hiện tại + mật khẩu mới (≥ 8 ký tự). Có mật khẩu mới thì sau khi lưu hồ sơ gọi thêm `POST /auth/me/password`.
5. Thành công → `useAuth().updateProfile(...)` đổi tên trên thanh menu ngay, đóng hộp thoại. Lỗi (số điện thoại sai dạng...) → hiện câu lỗi server trả, không đóng.
