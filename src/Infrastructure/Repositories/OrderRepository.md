# OrderRepository.cs

Truy vấn đơn hàng, kiện hàng và lịch sử trạng thái.

| Hàm | SQL tương đương / ghi chú |
|---|---|
| `GetAsync(id, customerId)` | `WHERE order_id = @id AND (@customerId IS NULL OR customer_id = @customerId)`. Có tracking để service sửa được. |
| `GetPackageAsync(packageId)` | Lấy kiện của đơn (khi sửa đơn). |
| `PageAsync(customerId, status, page, pageSize)` | `COUNT(*)` để có `totalItems`, rồi `ORDER BY created_at DESC, order_id DESC OFFSET … LIMIT …`. Tên trạm gửi/nhận lấy bằng subquery trong cùng một câu SQL, không N+1. |
| `GetHistoryAsync(orderId)` | Lịch sử cũ → mới (`changed_at`, rồi `id`). |
| `AddAsync(order, package, history)` | Thêm 3 dòng rồi `SaveChangesAsync` **một lần** = một transaction. |
| `UpdateAsync(order, history?)` | Lưu thay đổi của đơn (và kiện nếu service đã sửa trong cùng request), thêm dòng lịch sử nếu có. |

`status` truyền vào đã được `OrderService` kiểm tra và chuẩn hóa (`"pending"` → `"Pending"`), repository không kiểm tra lại.

## Thêm ở v0.11

| Hàm | Làm gì |
|---|---|
| `PageAsync(..., search, ...)` | `search` khác rỗng → `UPPER(tracking_code) LIKE '%SEARCH%'`. Danh sách trả thêm người nhận |
| `StatsByCustomerAsync()` | Một truy vấn `GROUP BY customer_id`: tổng đơn, hoàn tất, đang xử lý (chưa Completed / Cancelled / Rejected), ngày đặt gần nhất. Dùng cho màn Khách hàng |
| `CountIncomingAsync(stationId, exceptOrderId)` | Đếm đơn `Scheduled`, `InTransit`, `ArrivedAtStation` có trạm nhận này, trừ chính đơn đang gán (giao lại đơn Failed không tự đếm mình) |

