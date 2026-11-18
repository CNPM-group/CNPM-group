# Login.jsx

Form email + mật khẩu.

1. Bấm Đăng nhập → `useAuth().login()` → `POST /auth/login`.
2. Thành công → chuyển tới `homePath(roles)`: Manager/Admin vào Thống kê, Dispatcher vào Điều phối, Station Operator vào Trạm.
3. Sai mật khẩu → hiện câu lỗi server trả.
4. Tài khoản Customer → báo "Tài khoản khách hàng dùng app mobile" và không lưu token.

Đang đăng nhập rồi mà mở `/login` thì tự chuyển về trang chính.

Hàng nút "Tài khoản demo" điền sẵn email và mật khẩu `Demo@12345` của 4 tài khoản nhân viên (thêm Admin từ v0.9) do `DbSeeder` tạo, để lúc demo không phải gõ. Khi đưa lên môi trường thật thì xóa mảng `DEMO_ACCOUNTS`.
