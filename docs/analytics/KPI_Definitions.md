# ĐỊNH NGHĨA CÁC CHỈ SỐ KPI ANALYTICS

## 1. Tổng quan

Tài liệu này định nghĩa các chỉ số đánh giá hiệu suất chính (KPI) cho dự án SmartDroneDelivery. Các chỉ số giúp theo dõi, thống kê và đánh giá hiệu quả hoạt động giao hàng bằng drone.

## 2. Danh sách các chỉ số KPI

| Chỉ số KPI | Mô tả | Công thức tính |
|---|---|---|
| Tổng số đơn hàng | Tổng số đơn giao hàng trong hệ thống | COUNT(order_id) |
| Đơn hàng hoàn thành | Số đơn hàng được giao thành công | COUNT(status = 'Completed') |
| Đơn hàng thất bại | Số đơn hàng giao không thành công | COUNT(status = 'Failed') |
| Đơn hàng đang chờ | Số đơn hàng đang chờ xử lý | COUNT(status = 'Pending') |
| Tỷ lệ giao hàng thành công | Tỷ lệ phần trăm đơn hàng giao thành công | Đơn hoàn thành / Tổng đơn hàng * 100 |
| Thời gian giao hàng trung bình | Thời gian trung bình để hoàn thành một đơn hàng | AVG(completed_at - created_at) |
| Tổng số drone | Tổng số drone được đăng ký trong hệ thống | COUNT(drone_id) |

## 3. Nguồn dữ liệu

Các chỉ số KPI được tổng hợp từ những nguồn dữ liệu sau:

- Thông tin đơn hàng giao hàng.
- Thông tin drone.
- Thông tin khách hàng.
- Thông tin chuyến bay của drone.

## 4. Ghi chú

- Các chỉ số KPI phải được tính toán dựa trên dữ liệu thực tế trong cơ sở dữ liệu.
- Sử dụng dữ liệu mẫu (Seed Data) của dự án để kiểm tra kết quả.
- Đảm bảo các phép tính chính xác và phản ánh đúng tình trạng hoạt động của hệ thống SmartDroneDelivery.

## 5. Mục đích sử dụng

Các chỉ số KPI hỗ trợ nhóm phát triển:

- Theo dõi số lượng và tình trạng đơn hàng.
- Đánh giá tỷ lệ giao hàng thành công.
- Kiểm soát thời gian giao hàng.
- Theo dõi số lượng drone trong hệ thống.
- Hỗ trợ phân tích và cải thiện hiệu quả vận hành.
