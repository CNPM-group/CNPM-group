# OrderService.cs

Nghiệp vụ đơn giao hàng. Controller: `Api/Controllers/OrdersController.cs`.

## Luật kiểm tra khi tạo và sửa đơn (`ValidateAsync`)

| # | Luật | Sai thì |
|---|---|---|
| 1 | Trạm gửi ≠ trạm nhận | 400 "Trạm gửi và trạm nhận phải khác nhau." |
| 2 | Cả hai trạm tồn tại | 400 "Trạm gửi hoặc trạm nhận không tồn tại." |
| 2b | Không trạm nào đang **Bảo trì** (v0.11) | 400 "<Tên trạm> đang bảo trì, chọn trạm khác." |
| 3 | Nếu có `deliveryAddressId` thì địa chỉ phải của chính khách | 400 |
| 4 | Có ít nhất một drone có `MaxPayloadKg` ≥ cân nặng kiện | 400 "Kiện hàng vượt quá tải trọng tối đa của đội drone." |

## Người nhận (v0.11, quy tắc nghiệp vụ số 2 "đơn phải có thông tin người nhận")

`RecipientAsync` chạy khi tạo và sửa đơn:

- Khách nhập `recipientName`, `recipientPhone` → lưu đúng như vậy (số điện thoại kiểm bằng `ProfileRules.Phone`: 8–15 chữ số, có thể có `+`).
- Bỏ trống ô nào → lấy họ tên / số điện thoại trong hồ sơ của chính khách (người nhận là khách tự ra trạm nhận).
- Hồ sơ cũng trống tên → "Khách hàng". Số điện thoại có thể trống.

Nhân viên trạm thấy người nhận trên thẻ đơn (trang Trạm) để gọi khi kiện tới.

Ngoài ra schema đã chặn cân nặng ngoài 0.01–50 kg, kích thước mỗi chiều ngoài 0,1–100 cm (v0.11, khớp BR-04 của báo cáo; trước đó cho tới 99 999,9 cm).

## Các hàm

| Hàm | Làm gì |
|---|---|
| `CreateAsync(customerId, input, package)` | Kiểm tra luật → tạo mã tra cứu `SDD-xxxxxxxxxxxxxxxx` (20 ký tự) → đơn `Pending` → một dòng lịch sử `null → Pending` → lưu đơn, kiện, lịch sử trong **một** lần `SaveChanges` |
| `UpdateAsync(customerId, orderId, input, package)` | Chỉ khi `Pending` (khác → 409). Kiểm tra luật, sửa kiện + trạm + địa chỉ + ghi chú |
| `CancelAsync(customerId, orderId)` | Chỉ khi `Pending` (khác → 409). Đổi sang `Cancelled`, ghi lịch sử |
| `ApproveAsync(staffId, orderId, note)` | `Pending` → `Approved` qua `OrderStateMachine`, ghi `ApprovedBy`, `ApprovedAtUtc` và một dòng lịch sử (ghi chú mặc định "Đơn đã được duyệt.") |
| `RejectAsync(staffId, orderId, reason)` | `Pending` → `Rejected`, bắt buộc lý do (lưu vào ghi chú của dòng lịch sử) |
| `PageAsync(scope, status, search, page, pageSize)` | Kiểm tra `status` hợp lệ (sai → 400), `search` tối đa 50 ký tự (tìm theo mã đơn, không phân biệt hoa thường, v0.11), kẹp `page ≥ 1`, `1 ≤ pageSize ≤ 100` |
| `GetAsync(scope, orderId)` | `scope = null` (nhân viên) xem mọi đơn; `scope = customerId` chỉ đơn của khách đó; không thấy → 404 |
| `GetDetailAsync(scope, orderId)` | Như `GetAsync`, lấy thêm kiện hàng và 2 trạm, trả `OrderDetail` để controller dựng `OrderDetailResponse` |
| `GetHistoryAsync(scope, orderId)` | Kiểm tra quyền xem như `GetAsync` rồi trả lịch sử cũ → mới |

## Trạng thái đơn

Danh sách trạng thái nằm ở `OrderStatus` (`Domain/Constants.cs`); bảng chuyển hợp lệ ở `Domain/Models/OrderStateMachine.cs` (khớp mục IV.3.1 của báo cáo). `CancelAsync` đã đi qua `OrderStateMachine.EnsureTransition`.

Service này lo tạo / sửa / hủy / duyệt / từ chối. Các bước giao (gán drone, cất cánh, tới trạm, OTP, thất bại, hủy và hoàn) nằm ở `DeliveryService.cs`. Bước nào đổi trạng thái cũng ghi một dòng `OrderStatusHistory` và gọi `OrderStateMachine.EnsureTransition` trước khi đổi, chuyển sai trả 409.

## Vì sao service không gọi `SaveChanges`

Service gọi `orders.AddAsync(order, package, history)`; repository thêm cả ba rồi lưu một lần. Nếu một bảng lỗi thì không bảng nào được ghi, không có đơn mồ côi thiếu kiện hoặc thiếu lịch sử.
