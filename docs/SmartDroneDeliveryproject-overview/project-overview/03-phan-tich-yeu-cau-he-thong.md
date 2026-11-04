# PHẦN 3. PHÂN TÍCH YÊU CẦU HỆ THỐNG

## 3.1. Tổng quan

Hệ thống SmartDroneDelivery cần đáp ứng các yêu cầu quản lý người dùng, khách hàng, đơn hàng, kiện hàng, trạm Drone, quá trình giao hàng, báo cáo và các chức năng hỗ trợ AI.

## 3.2. Yêu cầu chức năng

| Mã | Nhóm chức năng | Mô tả |
|---|---|---|
| FR01 | Tài khoản | Đăng ký, đăng nhập, đăng xuất |
| FR02 | Phân quyền | Quản lý vai trò và quyền truy cập |
| FR03 | Khách hàng | Quản lý thông tin khách hàng |
| FR04 | Địa chỉ | Quản lý địa chỉ giao nhận |
| FR05 | Đơn hàng | Tạo, xem, cập nhật và hủy đơn |
| FR06 | Kiện hàng | Quản lý khối lượng và kích thước |
| FR07 | Trạm Drone | Quản lý thông tin và trạng thái trạm |
| FR08 | Điều phối | Duyệt và phân công đơn hàng |
| FR09 | Theo dõi | Cập nhật trạng thái giao hàng |
| FR10 | Xác nhận | Ghi nhận kết quả giao hàng |
| FR11 | Báo cáo | Thống kê và tổng hợp dữ liệu |
| FR12 | AI | Hỗ trợ dự đoán và phân tích |

## 3.3. Mô tả yêu cầu chức năng

### 3.3.1. Quản lý tài khoản
- Đăng ký tài khoản.
- Đăng nhập và đăng xuất.
- Cập nhật thông tin cá nhân.
- Quản lý vai trò người dùng.
- Kiểm tra quyền truy cập.

### 3.3.2. Quản lý khách hàng
- Thêm, xem và cập nhật thông tin khách hàng.
- Quản lý địa chỉ giao nhận.
- Tra cứu lịch sử đơn hàng.

### 3.3.3. Quản lý đơn hàng
- Tạo đơn hàng mới.
- Xem danh sách và chi tiết đơn hàng.
- Chỉnh sửa thông tin theo quyền hạn.
- Tìm kiếm và lọc đơn hàng.
- Hủy đơn theo quy định.

### 3.3.4. Quản lý kiện hàng
- Quản lý mã kiện hàng.
- Lưu khối lượng và kích thước.
- Kiểm tra điều kiện vận chuyển.
- Liên kết kiện hàng với đơn hàng.

### 3.3.5. Quản lý trạm Drone
- Thêm và cập nhật thông tin trạm.
- Xem trạng thái hoạt động.
- Theo dõi số lượng đơn tại trạm.
- Quản lý khả năng tiếp nhận đơn.

### 3.3.6. Theo dõi và xác nhận giao hàng
- Cập nhật trạng thái giao hàng.
- Hiển thị tiến trình thực hiện.
- Ghi nhận thời gian giao hàng.
- Xác nhận giao thành công hoặc thất bại.
- Lưu lịch sử xử lý.

### 3.3.7. Báo cáo và AI
- Thống kê đơn theo trạng thái.
- Tính tỷ lệ giao hàng thành công.
- Thống kê thời gian giao hàng trung bình.
- Phân tích hiệu quả hoạt động theo trạm.
- Hỗ trợ dự đoán thời gian giao hàng khi có dữ liệu phù hợp.

## 3.4. Yêu cầu phi chức năng

| Mã | Yêu cầu | Mô tả |
|---|---|---|
| NFR01 | Bảo mật | Xác thực và phân quyền người dùng |
| NFR02 | Hiệu năng | Truy vấn và xử lý trong thời gian hợp lý |
| NFR03 | Ổn định | Đảm bảo dữ liệu nhất quán |
| NFR04 | Mở rộng | Có thể bổ sung chức năng mới |
| NFR05 | Dễ sử dụng | Giao diện rõ ràng |
| NFR06 | Bảo trì | Mã nguồn có cấu trúc và tài liệu |
| NFR07 | Tương thích | Hỗ trợ môi trường triển khai mục tiêu |

## 3.5. Ma trận quyền hạn đề xuất

| Chức năng | Customer | Dispatcher | Station Operator | Manager | Admin |
|---|---|---|---|---|---|
| Tạo đơn | Có | Không | Không | Có thể | Có thể |
| Xem đơn của mình | Có | Có | Theo phân công | Có | Có |
| Duyệt đơn | Không | Có | Không | Có thể | Có thể |
| Phân công đơn | Không | Có | Không | Có | Có |
| Cập nhật tiến trình | Không | Theo quyền | Có | Có thể | Có |
| Xác nhận giao | Theo quy trình | Theo quyền | Có | Có thể | Có |
| Xem báo cáo | Không | Theo quyền | Theo quyền | Có | Có |
| Quản lý tài khoản | Không | Không | Không | Theo quyền | Có |

## 3.6. Quy tắc nghiệp vụ

1. Mỗi đơn hàng phải gắn với khách hàng hợp lệ.
2. Đơn hàng phải có thông tin người nhận và địa chỉ giao.
3. Kiện hàng phải đáp ứng điều kiện vận chuyển.
4. Đơn hàng cần được kiểm tra trước khi phân công.
5. Người dùng chỉ được thực hiện chức năng phù hợp với quyền hạn.
6. Những thay đổi trạng thái quan trọng cần được lưu lịch sử.
7. Đơn hàng hoàn tất phải có thông tin xác nhận theo quy trình.

## 3.7. Điều kiện nghiệm thu đề xuất

| Mã | Tiêu chí | Kết quả mong đợi |
|---|---|---|
| AC01 | Đăng nhập | Xác thực đúng tài khoản |
| AC02 | Tạo đơn | Lưu được đơn hợp lệ |
| AC03 | Kiểm tra dữ liệu | Từ chối dữ liệu không hợp lệ |
| AC04 | Phân công | Ghi nhận thông tin phân công |
| AC05 | Theo dõi | Hiển thị trạng thái mới |
| AC06 | Xác nhận | Lưu kết quả giao hàng |
| AC07 | Báo cáo | Tính toán đúng trên dữ liệu kiểm thử |

## 3.8. Kết luận

Các yêu cầu chức năng, phi chức năng và quy tắc nghiệp vụ là căn cứ để thiết kế hệ thống và xây dựng kế hoạch kiểm thử SmartDroneDelivery.
