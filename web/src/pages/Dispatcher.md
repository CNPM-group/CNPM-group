# Dispatcher.jsx

Màn hình chính của điều phối viên: bảng đơn hàng lọc theo trạng thái, mỗi dòng có nút thao tác hợp với trạng thái đó.

## Tab lọc

Chờ duyệt · Đã duyệt · Đã gán drone · Đang bay · Tới trạm · Thất bại · Hoàn tất (thêm ở v0.10) · Tất cả. Mỗi tab gọi `GET /orders?status=<...>&pageSize=100`. Mở trang mặc định ở "Chờ duyệt".

## Tìm theo mã đơn (v0.11)

Ô **Tìm mã đơn** cạnh tiêu đề: gõ một phần mã (ví dụ `7f3a`), dừng 300 ms thì gọi `GET /orders?status=<tab>&q=<từ khóa>`. Tìm trong tab đang mở; muốn tìm khắp nơi thì chọn tab Tất cả. Không có kết quả thì báo "Không có đơn nào khớp ...".

Hộp thoại gán drone hiện thêm người nhận (tên, số điện thoại) ngay dưới tuyến bay.

## Nút theo trạng thái

| Trạng thái đơn | Nút | Gọi API | Kết quả |
|---|---|---|---|
| Pending | Duyệt | `POST /orders/{id}/approve` | Approved |
| Pending | Từ chối → hộp thoại nhập lý do | `POST .../reject { reason }` | Rejected |
| Approved | Gán drone → hộp thoại chọn drone | `POST .../schedule { droneId }` | Scheduled |
| Failed | Giao lại → hộp thoại chọn drone | `POST .../schedule { droneId }` | Scheduled |
| Failed | Hủy & hoàn → hộp thoại nhập lý do | `POST .../return { reason }` | Cancelled, khách nhận thông báo (KAN-57) |
| Scheduled | Cất cánh | `POST .../start` | InTransit |
| InTransit, ArrivedAtStation | Báo lỗi → hộp thoại nhập lý do | `POST .../fail { reason }` | Failed |

Trạng thái khác (Completed, Rejected, Cancelled) không có nút thao tác.

Mọi dòng có thêm nút **Tóm tắt AI** (v0.10, KAN-62): mở `AiSummaryDialog`, gọi `POST /orders/{id}/ai-summary`, hiện 3–4 câu tóm tắt chuyến giao và nhãn ai viết (AI hay server tự ghép). Hữu ích nhất ở đơn Hoàn tất và Thất bại.

Trong lúc một dòng đang gửi request thì các nút của dòng đó bị khóa (`busyId`), tránh bấm 2 lần.

## Hộp thoại gán drone (`ScheduleDialog`)

1. Gọi song song `GET /drones` và `GET /orders/{id}` (lấy `package.weightKg`).
2. Chỉ hiện drone có `status = Idle` và `maxPayloadKg ≥` cân nặng kiện.
3. Không có drone nào phù hợp → báo đỏ, nút Gán drone bị khóa.
4. Đơn có ảnh kiện (`package.imageKey`) → tải ảnh qua `GET /orders/{id}/package-image` (kèm token) và hiện trên hộp thoại, để điều phối viên nhìn kích cỡ kiện trước khi chọn drone.

Server vẫn tự kiểm tra lại (drone phải rảnh, đủ tải). Bước lọc ở web chỉ để điều phối viên khỏi chọn nhầm.

## Hộp thoại lý do (`ReasonDialog`)

Dùng chung cho Từ chối, Báo lỗi và Hủy & hoàn. Lý do bắt buộc (server cũng bắt buộc, tối đa 1000 ký tự).
