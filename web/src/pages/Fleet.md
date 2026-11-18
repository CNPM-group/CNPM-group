# Fleet.jsx

Trang **Drone & trạm** (`/fleet`) cho Manager, Admin: xem, thêm, sửa, xóa drone và trạm hạ cánh (KAN-59).

## Drone

Bảng từ `GET /drones`: mã, model, tải tối đa, tầm bay, pin, trạng thái, trạm đang đậu.

| Nút | API | Ghi chú |
|---|---|---|
| + Thêm drone | `POST /drones` | Form: mã (≤ 30 ký tự, không trùng), model, tải 0,1–50 kg, tầm bay 0,1–500 km, tốc độ 1–200 km/h, pin 0–100 %, trạng thái, trạm đang đậu |
| Sửa | `PUT /drones/{id}` | Cùng form, điền sẵn giá trị cũ |
| Xóa | `DELETE /drones/{id}` | Hỏi lại trước khi xóa. Server chặn xóa drone đang gán/đang bay (409), lỗi hiện thông báo đỏ |

Trạng thái drone: Rảnh (Idle) · Đã gán đơn (Assigned) · Đang bay (InFlight) · Đang sạc (Charging) · Bảo trì (Maintenance). Chỉ drone **Rảnh** mới hiện trong hộp thoại gán drone ở trang Điều phối, nên chuyển drone sang Bảo trì là cách tạm ngưng dùng nó.

## Trạm

Bảng từ `GET /stations`: tên, vĩ độ, kinh độ, trạng thái (Hoạt động / Đang bận / Bảo trì). Thêm, sửa, xóa giống drone (`POST/PUT/DELETE /stations`). Tọa độ dùng để `EtaService` tính khoảng cách và ETA.

**Sức chứa và tải trạm (v0.11, FR07):**

- Form trạm có ô **Sức chứa** (1–50 kiện cùng lúc), gửi kèm `capacity` khi thêm và sửa.
- Cột **Đang tới + chờ / sức chứa** lấy từ `GET /stations/load`: số đơn đang bay tới + kiện chờ khách nhận, trên sức chứa. Bằng hoặc vượt thì chữ đỏ "· đầy": lúc này điều phối gán drone tới trạm đó sẽ bị server trả 409.
- Cột **Sắp đi**: đơn Chờ duyệt / Đã duyệt / Đã gán drone sẽ cất cánh từ trạm này.
- Xóa trạm đã có đơn → server trả 409 "chuyển sang Bảo trì thay vì xóa" (v0.10). Trạm Bảo trì không nhận đơn mới và không nhận drone.

## Cách viết

- `useForm(initial)` là hook nhỏ trong file: giữ object form, hàm `bind('tên')` trả `{ value, onChange }` để gắn vào `<input>` cho gọn.
- Ô số trong form là chuỗi; trước khi gửi đổi bằng `Number(...)`, trạm đang đậu để trống thì gửi `null`.
- Giới hạn `min`/`max` trên ô nhập lấy đúng từ `[Range]` của `DroneRequest`/`StationRequest` phía server.
