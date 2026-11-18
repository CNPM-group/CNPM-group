# signalr_config.dart

Hằng số kết nối SignalR, phải khớp với backend và web:

| Hằng | Giá trị |
|---|---|
| `hubUrl` | `<gốc của apiBaseUrl>/hubs/delivery` (bỏ đuôi `/api/v1`) |
| `eventName` | `OrderStatusUpdated` – đơn đổi trạng thái |
| `droneEventName` | `DroneLocation` – vị trí drone đang bay (v0.9) |

Tên event phải trùng `SignalROrderEvents.EventName` / `DroneEventName` ở backend (`src/Infrastructure/Realtime/SignalROrderEvents.cs`).
