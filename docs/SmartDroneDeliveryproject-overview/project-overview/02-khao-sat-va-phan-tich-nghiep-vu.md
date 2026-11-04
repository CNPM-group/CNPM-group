
# PHẦN 2. KHẢO SÁT VÀ PHÂN TÍCH NGHIỆP VỤ

## 2.1. Khảo sát hiện trạng

Hiện nay, hoạt động giao nhận hàng hóa chủ yếu được thực hiện bằng các phương tiện vận chuyển truyền thống như xe máy, ô tô và xe tải. Khi số lượng đơn hàng tăng, việc quản lý và theo dõi quá trình giao hàng có thể gặp nhiều khó khăn.

Một số vấn đề thường gặp:
- Khó quản lý nhiều đơn hàng cùng lúc.
- Khó theo dõi trạng thái giao hàng liên tục.
- Việc phân công đơn hàng và phương tiện chưa tối ưu.
- Khó tổng hợp dữ liệu để đánh giá hiệu quả giao hàng.
- Thông tin giữa khách hàng, nhân viên điều phối và trạm giao nhận có thể chưa đồng bộ.

## 2.2. Nhu cầu xây dựng hệ thống

Từ những vấn đề trên, hệ thống SmartDroneDelivery được đề xuất nhằm:
- Quản lý tập trung thông tin khách hàng và đơn hàng.
- Quản lý thông tin kiện hàng và trạm Drone.
- Hỗ trợ nhân viên điều phối xử lý và phân công đơn hàng.
- Theo dõi tiến trình giao hàng.
- Ghi nhận kết quả giao hàng.
- Tổng hợp dữ liệu và hỗ trợ phân tích bằng AI.

## 2.3. Các đối tượng tham gia nghiệp vụ

| Đối tượng | Vai trò |
|---|---|
| Customer | Tạo yêu cầu và theo dõi đơn hàng |
| Dispatcher | Tiếp nhận, duyệt và phân công đơn hàng |
| Station Operator | Quản lý hoạt động tại trạm Drone |
| Logistics Manager | Theo dõi và đánh giá hoạt động giao hàng |
| System Administrator | Quản lý tài khoản và phân quyền hệ thống |

## 2.4. Quy trình nghiệp vụ tổng quát

Quy trình giao hàng bằng Drone được đề xuất theo các bước:

1. Khách hàng đăng nhập vào hệ thống.
2. Khách hàng nhập thông tin người nhận, địa chỉ và kiện hàng.
3. Hệ thống tiếp nhận và lưu yêu cầu giao hàng.
4. Nhân viên điều phối kiểm tra thông tin đơn hàng.
5. Đơn hàng được duyệt và phân công trạm, phương án giao hàng phù hợp.
6. Nhân viên trạm tiếp nhận và cập nhật quá trình thực hiện.
7. Hệ thống cập nhật trạng thái để khách hàng theo dõi.
8. Khi giao hàng hoàn tất, hệ thống ghi nhận kết quả xác nhận.
9. Dữ liệu đơn hàng được lưu lại để phục vụ thống kê và báo cáo.

## 2.5. Quy trình xử lý đơn hàng

### a. Tiếp nhận đơn hàng
- Khách hàng tạo yêu cầu giao hàng.
- Nhập thông tin người gửi, người nhận và địa chỉ.
- Nhập thông tin kiện hàng như khối lượng và kích thước.
- Hệ thống kiểm tra dữ liệu đầu vào.

### b. Kiểm tra và duyệt đơn
- Nhân viên điều phối xem thông tin đơn hàng.
- Kiểm tra điều kiện vận chuyển.
- Phê duyệt hoặc từ chối đơn hàng.
- Thông báo kết quả xử lý cho khách hàng.

### c. Phân công giao hàng
- Xác định trạm phù hợp.
- Phân công đơn hàng cho phương án giao hàng.
- Cập nhật thông tin phân công trên hệ thống.

### d. Theo dõi giao hàng
- Cập nhật trạng thái đơn hàng.
- Hiển thị tiến trình giao hàng cho khách hàng.
- Ghi nhận thông tin vị trí Drone nếu có dữ liệu tích hợp.

### e. Xác nhận kết quả
- Ghi nhận đơn hàng đã giao thành công.
- Lưu thời gian và thông tin xác nhận.
- Nếu giao thất bại, ghi nhận lý do và chuyển sang quy trình xử lý phù hợp.

## 2.6. Các trạng thái đơn hàng

| Trạng thái | Ý nghĩa |
|---|---|
| Pending | Đơn hàng đang chờ duyệt |
| Approved | Đơn hàng đã được phê duyệt |
| Assigned | Đơn hàng đã được phân công |
| InTransit | Đơn hàng đang vận chuyển |
| Delivered | Đơn hàng đã giao thành công |
| Failed | Giao hàng thất bại |
| Cancelled | Đơn hàng đã bị hủy |

## 2.7. Các vấn đề cần giải quyết

- Đảm bảo thông tin đơn hàng được quản lý chính xác.
- Hạn chế sai sót khi tiếp nhận và xử lý đơn.
- Cập nhật trạng thái giao hàng kịp thời.
- Đảm bảo dữ liệu giữa các bộ phận được đồng bộ.
- Hỗ trợ quản lý theo dõi hiệu quả hoạt động tại các trạm.
- Tận dụng dữ liệu để hỗ trợ dự đoán và phân tích bằng AI.

## 2.8. Kết luận

Qua quá trình khảo sát và phân tích nghiệp vụ, có thể thấy việc xây dựng SmartDroneDelivery giúp chuẩn hóa quy trình quản lý giao hàng bằng Drone. Hệ thống hướng đến việc kết nối khách hàng, nhân viên điều phối, nhân viên trạm và nhà quản lý trên cùng một nền tảng, đồng thời hỗ trợ theo dõi và phân tích dữ liệu giao hàng.
