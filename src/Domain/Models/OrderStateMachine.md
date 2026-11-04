# OrderStateMachine.cs

Là gì: bảng luật "đơn đang ở trạng thái này thì được chuyển sang trạng thái nào". Cả hệ thống chỉ có **một** chỗ giữ luật này.

Tác dụng: mọi service đổi trạng thái đơn (`OrderService`, `DeliveryService`) đều gọi `EnsureTransition` trước khi đổi. Chuyển sai luật → `ConflictException` → API trả **409**. Nhờ vậy không có chuyện đơn đang `Pending` mà bị bấm "cất cánh", hay đơn đã `Completed` bị hủy.

## Bảng chuyển

| Đang ở | Được sang | Do ai / API |
|---|---|---|
| `Pending` | `Approved` | Dispatcher – `POST /orders/{id}/approve` |
| `Pending` | `Rejected` | Dispatcher – `POST /orders/{id}/reject` |
| `Pending` | `Cancelled` | Customer – `POST /orders/{id}/cancel` |
| `Approved` | `Scheduled` | Dispatcher – `POST /orders/{id}/schedule` |
| `Scheduled` | `InTransit` | Dispatcher – `POST /orders/{id}/start` |
| `InTransit` | `ArrivedAtStation` | Nhân viên trạm – `POST /orders/{id}/arrive` |
| `InTransit` | `Failed` | Dispatcher – `POST /orders/{id}/fail` |
| `ArrivedAtStation` | `Completed` | Nhân viên trạm – `POST /orders/{id}/confirm` (đúng OTP) |
| `ArrivedAtStation` | `Failed` | Dispatcher – `POST /orders/{id}/fail` |
| `Failed` | `Scheduled` | Dispatcher – `POST /orders/{id}/schedule` (giao lại) |
| `Failed` | `Cancelled` | Hoàn trả – **chưa có API** |

`Rejected`, `Cancelled`, `Completed` là trạng thái cuối, không đi tiếp được.

```
Pending ──> Approved ──> Scheduled ──> InTransit ──> ArrivedAtStation ──> Completed
   │                        ▲              │                 │
   ├──> Rejected            └─── Failed <──┴─────────────────┘
   └──> Cancelled                  └──> Cancelled
```

## Hàm

| Hàm | Làm gì |
|---|---|
| `CanTransition(from, to)` | Trả `true`/`false`, không ném lỗi. Dùng khi chỉ cần hỏi (ví dụ ẩn/hiện nút trên giao diện) |
| `EnsureTransition(from, to)` | Sai thì ném `ConflictException("Không thể chuyển đơn từ X sang Y.")` |

`from` là chuỗi vì cột `status` trong CSDL lưu dạng text; chuỗi lạ (không thuộc `OrderStatus`) coi như không chuyển được.

## Muốn thêm trạng thái / luật mới

1. Thêm giá trị vào enum `OrderStatus` trong `Domain/Constants.cs`.
2. Thêm dòng vào `Transitions` ở file này.
3. Sửa CHECK constraint cột `status` của bảng `delivery_orders` (migration mới, xem `src/Migrations/README.md`).
4. Cập nhật bảng trên, `docs/openapi.yaml` và mục IV.3.1 của báo cáo.
