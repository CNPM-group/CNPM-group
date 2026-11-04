# PHẦN 3. PHÂN TÍCH YÊU CẦU HỆ THỐNG

## 3.1. Tổng quan yêu cầu

Hệ thống SmartDroneDelivery được xây dựng nhằm hỗ trợ quản lý toàn bộ quy trình giao hàng bằng Drone. Hệ thống cần đáp ứng các yêu cầu về quản lý người dùng, khách hàng, đơn hàng, kiện hàng, trạm Drone, theo dõi giao hàng và báo cáo thống kê.

## 3.2. Yêu cầu chức năng

### 3.2.1. Quản lý tài khoản
- Đăng ký tài khoản mới.
- Đăng nhập và đăng xuất.
- Cập nhật thông tin cá nhân.
- Quản lý vai trò và quyền truy cập.
- Bảo vệ thông tin tài khoản người dùng.

### 3.2.2. Quản lý khách hàng
- Thêm thông tin khách hàng.
- Xem và cập nhật thông tin khách hàng.
- Quản lý địa chỉ giao và nhận hàng.
- Tra cứu lịch sử đơn hàng.

### 3.2.3. Quản lý đơn hàng
- Tạo đơn hàng mới.
- Xem danh sách và chi tiết đơn hàng.
- Chỉnh sửa thông tin đơn hàng theo quyền hạn.
- Tìm kiếm và lọc đơn hàng.
- Hủy đơn hàng theo quy định.
- Theo dõi trạng thái xử lý đơn hàng.

### 3.2.4. Quản lý kiện hàng
- Lưu thông tin kiện hàng.
- Quản lý khối lượng và kích thước.
- Kiểm tra điều kiện vận chuyển.
- Liên kết kiện hàng với đơn hàng tương ứng.

### 3.2.5. Quản lý trạm Drone
- Thêm và cập nhật thông tin trạm.
- Xem danh sách các trạm.
- Quản lý trạng thái hoạt động của trạm.
- Theo dõi số lượng đơn hàng tại từng trạm.

### 3.2.6. Quản lý quá trình giao hàng
- Tiếp nhận và phân công đơn hàng.
- Cập nhật trạng thái giao hàng.
- Theo dõi tiến trình thực hiện.
- Ghi nhận thời gian bắt đầu và kết thúc giao hàng.
- Xử lý các trường hợp giao hàng thất bại.

### 3.2.7. Theo dõi và xác nhận giao hàng
- Hiển thị trạng thái đơn hàng.
- Cập nhật dữ liệu theo thời gian thực khi có tích hợp.
- Ghi nhận kết quả giao hàng.
- Lưu thời gian và thông tin xác nhận.
- Ghi nhận lý do khi giao hàng không thành công.

### 3.2.8. Báo cáo và thống kê
- Thống kê tổng số đơn hàng.
- Thống kê đơn hàng theo trạng thái.
- Tính tỷ lệ giao hàng thành công và thất bại.
- Thống kê thời gian giao hàng trung bình.
- Tổng hợp hiệu quả hoạt động theo trạm.

### 3.2.9. Chức năng hỗ trợ AI
- Hỗ trợ dự đoán thời gian giao hàng.
- Đề xuất phương án hoặc tuyến đường giao hàng.
- Phân tích dữ liệu hoạt động.
- Hỗ trợ người quản lý đưa ra quyết định dựa trên dữ liệu.

## 3.3. Yêu cầu phi chức năng

### 3.3.1. Bảo mật
- Xác thực người dùng khi đăng nhập.
- Phân quyền theo từng vai trò.
- Bảo vệ thông tin cá nhân.
- - Sử dụng HTTPS khi triển khai thực tế.

### 3.3.2. Hiệu năng
- Hỗ trợ xử lý nhiều yêu cầu từ người dùng.
- Tối ưu thời gian truy vấn dữ liệu.
- Hiển thị thông tin trong thời gian hợp lý.

### 3.3.3. Tính ổn định
- Hạn chế lỗi trong quá trình sử dụng.
- Đảm bảo dữ liệu được lưu trữ chính xác.
- Có cơ chế xử lý lỗi khi kết nối API hoặc cơ sở dữ liệu gặp sự cố.

### 3.3.4. Khả năng mở rộng
- Có thể bổ sung chức năng mới.
- Có thể tích hợp thêm dịch vụ bên ngoài.
- Hỗ trợ mở rộng số lượng người dùng và trạm giao nhận.

### 3.3.5. Tính dễ sử dụng
- Giao diện rõ ràng và dễ thao tác.
- Các chức năng được bố trí hợp lý.
- Có thông báo khi thao tác thành công hoặc thất bại.

## 3.4. Quy tắc nghiệp vụ

- Mỗi đơn hàng phải có khách hàng hợp lệ.
- Đơn hàng phải có thông tin người nhận và địa chỉ giao hàng.
- Kiện hàng phải đáp ứng điều kiện vận chuyển.
- Đơn hàng phải được kiểm tra trước khi phân công.
- Chỉ người dùng có quyền hạn mới được thực hiện các thao tác quản lý.
- Mọi thay đổi trạng thái quan trọng cần được ghi nhận.
- Kết quả giao hàng phải được lưu trữ để phục vụ tra cứu và thống kê.

## 3.5. Kết luận

Các yêu cầu trên là cơ sở để thiết kế kiến trúc, cơ sở dữ liệu, giao diện và xây dựng chức năng cho hệ thống SmartDroneDelivery. Việc xác định rõ yêu cầu giúp nhóm phát triển hệ thống đúng mục tiêu và phù hợp với nghiệp vụ thực tế.
