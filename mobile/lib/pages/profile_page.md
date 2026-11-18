# profile_page.dart

Màn **Hồ sơ của tôi** (v0.11, FR01). Mở từ menu ⋮ ở màn Đơn của tôi → "Hồ sơ của tôi".

- Mở lên gọi `Api.getMe()` (`GET /auth/me`), điền họ tên và số điện thoại. Email hiện ở trên, không sửa được.
- Kiểm tra ngay trên máy: họ tên ít nhất 2 ký tự, số điện thoại trống hoặc 8–15 chữ số (có thể có `+`). Server kiểm lại lần nữa.
- **Lưu** → `Api.updateMe()` (`PUT /auth/me`), báo "Đã lưu hồ sơ".
- **Đổi mật khẩu** (UC-03): nhập mật khẩu hiện tại + mật khẩu mới (≥ 8 ký tự) rồi bấm Lưu → thêm `Api.changePassword()` (`POST /auth/me/password`). Bỏ trống mật khẩu mới thì không đổi. Sai mật khẩu cũ → báo "Mật khẩu hiện tại không đúng."

Số điện thoại ở đây cũng là số mặc định của người nhận: tạo đơn mà bỏ trống ô người nhận thì server lấy tên và số trong hồ sơ.
