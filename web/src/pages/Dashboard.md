# Dashboard.jsx

Thống kê cho quản lý, lấy từ `GET /reports/orders` (mặc định 30 ngày gần nhất, server tự tính bằng SQL). Riêng khung "Nhận xét của AI" ở cuối trang mới dùng AI.

| Khối | Lấy từ trường |
|---|---|
| Tổng đơn, Hoàn tất, Thất bại | `totalOrders`, `completedOrders`, `failedOrders` |
| Tỷ lệ thành công | `successRatePct`, làm tròn 1 chữ số |
| Thời gian giao trung bình | `avgDeliveryMinutes` (chưa có đơn hoàn tất thì hiện `-`) |
| Bảng theo trạng thái | `byStatus: [{ status, count }]` |
| Thanh số đơn theo ngày | `byDay: [{ day, count }]`, thanh dài nhất = ngày nhiều đơn nhất |

Biểu đồ thanh vẽ bằng CSS (`.bar` có `width` theo phần trăm), không cần thư viện.

**Tải CSV** gọi `GET /reports/orders.csv` kèm token, nhận file dạng `Blob` rồi tạo link tải tạm. File có BOM UTF-8 nên mở bằng Excel không lỗi font tiếng Việt.

## Lọc khoảng ngày (v0.11)

Hai ô **Từ / Đến** cạnh tiêu đề. Chọn xong thì cả trang (số liệu, bảng theo trạm, tải CSV, nhận xét AI) dùng khoảng đó, gửi `?from=yyyy-mm-dd&to=<ngày sau ngày Đến>` vì server tính `to` là mốc loại trừ. Bấm **30 ngày gần nhất** để bỏ lọc. Server giới hạn tối đa 366 ngày.

## Hiệu quả theo trạm (v0.11, FR11)

Bảng giữa trang, từ `GET /reports/stations`: mỗi trạm số đơn gửi đi, nhận về, hoàn tất, thất bại, tỷ lệ thành công, thời gian giao trung bình. 4 cột sau tính theo **trạm nhận** (nơi khách nhập OTP), ghi rõ dưới bảng. Trạm chưa có đơn kết thúc thì tỷ lệ hiện `-`.

## Nhận xét của AI (v0.10)

Khung dưới cùng có nút **Phân tích bằng AI** → `GET /reports/ai-insights`. Server đưa số liệu 30 ngày (tổng đơn, tỷ lệ thành công, lý do thất bại hay gặp, tuyến đông nhất, đơn còn chờ duyệt...) cho LLM viết 3–5 nhận xét kèm việc nên làm, hiện thành danh sách. Dưới danh sách là nhãn `AiSource`: "AI viết · model" hoặc "Tự ghép từ dữ liệu" khi chưa có API key.

Không tự gọi khi mở trang, vì mỗi lần là 1 lần gọi LLM (tốn phí); bấm lại thì đổi thành "Phân tích lại".

Trang tự tải lại khi có đơn đổi trạng thái, nên lúc demo nhập OTP xong mở Thống kê là thấy số Hoàn tất tăng.
