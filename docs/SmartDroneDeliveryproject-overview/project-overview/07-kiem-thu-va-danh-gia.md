
# PHẦN 7. KIỂM THỬ VÀ ĐÁNH GIÁ

## 7.1. Mục tiêu kiểm thử

Kiểm thử nhằm xác định hệ thống SmartDroneDelivery có hoạt động đúng theo yêu cầu, xử lý dữ liệu chính xác và đáp ứng các chức năng đã đề ra hay không.

Các mục tiêu:
- Kiểm tra từng chức năng.
- Kiểm tra kết nối giữa Frontend và Backend.
- Kiểm tra khả năng lưu trữ và truy xuất dữ liệu.
- Kiểm tra phân quyền và bảo mật.
- Phát hiện và sửa lỗi trước khi triển khai.

## 7.2. Phương pháp kiểm thử

| Loại kiểm thử | Nội dung |
|---|---|
| Unit Testing | Kiểm thử từng thành phần nhỏ |
| Integration Testing | Kiểm thử kết nối giữa các thành phần |
| API Testing | Kiểm tra các API |
| UI Testing | Kiểm tra giao diện và thao tác |
| System Testing | Kiểm thử toàn bộ hệ thống |
| Security Testing | Kiểm tra xác thực và phân quyền |

## 7.3. Môi trường kiểm thử

| Thành phần | Môi trường dự kiến |
|---|---|
| Web | Trình duyệt máy tính |
| Mobile | Thiết bị Android hoặc iOS |
| Backend | ASP.NET Core Web API |
| Database | PostgreSQL |
| API Testing | Postman |
| Source Code | GitHub |

## 7.4. Test case chức năng

| Mã | Chức năng | Dữ liệu kiểm thử | Kết quả mong đợi |
|---|---|---|---|
| TC01 | Đăng ký | Thông tin hợp lệ | Tạo tài khoản thành công |
| TC02 | Đăng ký | Email đã tồn tại | Thông báo lỗi |
| TC03 | Đăng nhập | Email và mật khẩu đúng | Đăng nhập thành công |
| TC04 | Đăng nhập | Mật khẩu sai | Từ chối đăng nhập |
| TC05 | Tạo đơn | Thông tin hợp lệ | Lưu đơn thành công |
| TC06 | Tạo đơn | Thiếu địa chỉ | Thông báo lỗi |
| TC07 | Cập nhật đơn | Dữ liệu hợp lệ | Cập nhật thành công |
| TC08 | Theo dõi đơn | Mã đơn hợp lệ | Hiển thị trạng thái |
| TC09 | Xác nhận giao | Đơn đủ điều kiện | Lưu xác nhận |
| TC10 | Phân quyền | Tài khoản không đủ quyền | Từ chối truy cập |

## 7.5. Kiểm thử API

Các nội dung cần kiểm tra:
- Endpoint và phương thức HTTP.
- Dữ liệu gửi lên.
- Mã phản hồi HTTP.
- Nội dung phản hồi.
- Trường hợp dữ liệu hợp lệ và không hợp lệ.
- Xác thực và quyền truy cập.

## 7.6. Kiểm thử giao diện

- Kiểm tra bố cục và hiển thị.
- Kiểm tra các nút chức năng.
- Kiểm tra biểu mẫu nhập liệu.
- Kiểm tra thông báo lỗi và thành công.
- Kiểm tra trên nhiều kích thước màn hình.

## 7.7. Kiểm thử bảo mật

- Kiểm tra đăng nhập.
- Kiểm tra phân quyền.
- Kiểm tra truy cập API khi chưa xác thực.
- Kiểm tra dữ liệu đầu vào.
- Kiểm tra bảo vệ thông tin nhạy cảm.

## 7.8. Bảng ghi nhận kết quả kiểm thử

| Mã test case | Kết quả thực tế | Trạng thái | Ghi chú |
|---|---|---|---|
| TC01 | Chưa kiểm thử | Pending | Cập nhật sau |
| TC02 | Chưa kiểm thử | Pending | Cập nhật sau |
| TC03 | Chưa kiểm thử | Pending | Cập nhật sau |
| TC04 | Chưa kiểm thử | Pending | Cập nhật sau |
| TC05 | Chưa kiểm thử | Pending | Cập nhật sau |
| TC06 | Chưa kiểm thử | Pending | Cập nhật sau |
| TC07 | Chưa kiểm thử | Pending | Cập nhật sau |
| TC08 | Chưa kiểm thử | Pending | Cập nhật sau |
| TC09 | Chưa kiểm thử | Pending | Cập nhật sau |
| TC10 | Chưa kiểm thử | Pending | Cập nhật sau |

## 7.9. Đánh giá hệ thống

Việc đánh giá dựa trên:
- Mức độ hoàn thành chức năng.
- Độ chính xác của dữ liệu.
- Khả năng kết nối giữa các thành phần.
- Mức độ dễ sử dụng.
- Khả năng đáp ứng yêu cầu bảo mật.
- Các lỗi còn tồn tại.

## 7.10. Kết luận

Kiểm thử là bước quan trọng nhằm đảm bảo SmartDroneDelivery hoạt động đúng yêu cầu. Kết quả thực tế cần được cập nhật sau khi nhóm thực hiện kiểm thử.
