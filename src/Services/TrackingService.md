# TrackingService.cs

Tính vị trí drone của một đơn đang giao (v0.9). Không gọi GPS thật: vị trí là **giả lập** dựa trên giờ cất cánh và ETA, đủ để demo theo dõi realtime.

## Công thức

```
% đã bay = (bây giờ − giờ cất cánh) × TimeScale ÷ (ETA − giờ cất cánh)   (kẹp trong 0–100 %)
vị trí   = trạm đi + (trạm đến − trạm đi) × % đã bay                      (nội suy thẳng theo vĩ độ, kinh độ)
```

| Trạng thái đơn | % đã bay |
|---|---|
| Chưa cất cánh (Pending → Scheduled), Failed, Cancelled, Rejected | 0 % – đứng ở trạm đi |
| InTransit | Theo công thức trên. Tới 100 % mà nhân viên trạm chưa bấm "tới trạm" thì đứng chờ ở 100 % |
| ArrivedAtStation, Completed | 100 % – ở trạm đến |

`TimeScale` (cấu hình `Simulator:TimeScale`, mặc định 1) cho bay nhanh hơn thật khi demo: để 10 thì chuyến 10 phút xong trong 1 phút.

## Hàm

| Hàm | Ai gọi | Làm gì |
|---|---|---|
| `GetAsync(customerScope, orderId)` | `GET /orders/{id}/tracking` | Lấy vị trí hiện tại. Khách chỉ xem được đơn của mình (giống `GET /orders/{id}`) |
| `MoveFlyingDronesAsync()` | `DroneSimulator` mỗi vài giây | Duyệt mọi đơn `InTransit` có chuyến bay đang `Flying`, tính vị trí, lưu vào `drone_flights` (`CurrentLatitude/Longitude`) và thêm 1 dòng `flight_telemetry`, trả danh sách để gửi qua SignalR |
| `Progress(...)` (static) | Bên trong | Công thức % ở trên, tách riêng để dễ kiểm |
| `Lerp(from, to, t)` | Bên trong | Nội suy tọa độ. 0 % và 100 % trả **đúng** tọa độ trạm (tính bằng `double` thì `a + (b − a) × 1` có thể lệch chữ số cuối, so sánh bằng nhau sẽ hỏng) |

Trả về `TrackingSnapshot` (Domain): tọa độ drone, %, tên và tọa độ 2 trạm, mã drone, giờ cất cánh, ETA.
