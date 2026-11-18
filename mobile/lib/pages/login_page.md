# login_page.dart

Màn đăng nhập / đăng ký của khách hàng.

- Mặc định là **Đăng nhập** (email + mật khẩu). Bấm chuyển sang **Đăng ký** thì có thêm ô họ tên; tài khoản tạo ra có role Customer.
- Kiểm tra ngay trên form: email phải có `@`, các ô không được trống.
- Gọi `AuthStore.login` / `register`. Thành công thì `Gate` (main.dart) tự chuyển sang danh sách đơn; lỗi thì hiện câu tiếng Việt từ `ApiException`.

Nhân viên (điều phối, trạm, quản lý) dùng web, không dùng app.
