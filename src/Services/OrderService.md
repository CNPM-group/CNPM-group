# OrderService.cs

Nghiệp vụ đơn giao hàng. Controller: `Api/Controllers/OrdersController.cs`.

## Luật kiểm tra khi tạo và sửa đơn (`ValidateAsync`)

| # | Luật | Sai thì |
|---|---|---|
| 1 | Trạm gửi ≠ trạm nhận | 400 "Trạm gửi và trạm nhận phải khác nhau." |
| 2 | Cả hai trạm tồn tại | 400 "Trạm gửi hoặc trạm nhận không tồn tại." |
| 3 | Nếu có `deliveryAddressId` thì địa chỉ phải của chính khách | 400 |
| 4 | Có ít nhất một drone có `MaxPayloadKg` ≥ cân nặng kiện | 400 "Kiện hàng vượt quá tải trọng tối đa của đội drone." |

Ngoài ra schema đã chặn cân nặng ngoài 0.01–50 kg, kích thước ngoài 0.1–99999.9 cm.

## Các hàm

| Hàm | Làm gì |
|---|---|
| `CreateAsync(customerId, input, package)` | Kiểm tra luật → tạo mã tra cứu `SDD-xxxxxxxxxxxxxxxx` (20 ký tự) → đơn `Pending` → một dòng lịch sử `null → Pending` → lưu đơn, kiện, lịch sử trong **một** lần `SaveChanges` |
| `UpdateAsync(customerId, orderId, input, package)` | Chỉ khi `Pending` (khác → 409). Kiểm tra luật, sửa kiện + trạm + địa chỉ + ghi chú |
| `CancelAsync(customerId, orderId)` | Chỉ khi `Pending` (khác → 409). Đổi sang `Cancelled`, ghi lịch sử |
| `ApproveAsync(staffId, orderId, note)` | `Pending` → `Approved` qua `OrderStateMachine`, ghi `ApprovedBy`, `ApprovedAtUtc` và một dòng lịch sử (ghi chú mặc định "Đơn đã được duyệt.") |
| `RejectAsync(staffId, orderId, reason)` | `Pending` → `Rejected`, bắt buộc lý do (lưu vào ghi chú của dòng lịch sử) |
| `PageAsync(scope, status, page, pageSize)` | Kiểm tra `status` hợp lệ (sai → 400), kẹp `page ≥ 1`, `1 ≤ pageSize ≤ 100` |
| `GetAsync(scope, orderId)` | `scope = null` (nhân viên) xem mọi đơn; `scope = customerId` chỉ đơn của khách đó; không thấy → 404 |
| `GetHistoryAsync(scope, orderId)` | Kiểm tra quyền xem như `GetAsync` rồi trả lịch sử cũ → mới |

## Trạng thái đơn

Danh sách trạng thái nằm ở `OrderStatus` (`Domain/Constants.cs`); bảng chuyển hợp lệ ở `Domain/Models/OrderStateMachine.cs` (khớp mục IV.3.1 của báo cáo). `CancelAsync` đã đi qua `OrderStateMachine.EnsureTransition`.

Hiện service có tạo/sửa/hủy/duyệt/từ chối. Lên lịch, gán drone, xác nhận giao làm ở các sprint sau (Jira KAN); bước nào đổi trạng thái cũng phải ghi một dòng `OrderStatusHistory` giống `CancelAsync`, và gọi `OrderStateMachine.EnsureTransition` trước khi đổi để chuyển sai trả 409 như `docs/openapi.yaml` quy định.

## Vì sao service không gọi `SaveChanges`

Service gọi `orders.AddAsync(order, package, history)`; repository thêm cả ba rồi lưu một lần. Nếu một bảng lỗi thì không bảng nào được ghi, không có đơn mồ côi thiếu kiện hoặc thiếu lịch sử.
