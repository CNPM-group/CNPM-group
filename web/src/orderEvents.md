# orderEvents.js

Hook `useOrderEvents(onChange)` cho trang biết "có đơn vừa đổi trạng thái, tải lại đi". Trả về `live`: `true` khi đang nối SignalR.

## Cách chạy

1. Mở kết nối tới hub `http://localhost:5080/hubs/delivery`, gửi token qua `accessTokenFactory` (SignalR tự gắn vào query `access_token`).
2. Nghe event `OrderStatusUpdated`, payload theo contract cả nhóm đã chốt:

   ```json
   { "orderId": "guid", "trackingCode": "SDD-...", "status": "InTransit", "updatedAt": "2026-11-13T08:00:00Z" }
   ```

3. Có event → gọi `onChange(event)`. Truyền thêm tham số thứ 2 `onDrone` thì nhận cả event `DroneLocation` (vị trí drone, v0.9) – trang Trạm dùng để chạy thanh tiến độ.
4. **Không nối được** (hub chưa có, backend tắt, mất mạng quá lâu) → chuyển sang hỏi lại server mỗi 10 giây, gọi `onChange(null)`. Nối lại được thì tắt chế độ này.
5. Rời trang → đóng kết nối, xóa bộ đếm giờ.

Từ v0.8 backend đã có hub, nên bình thường nhãn sẽ là "Realtime". Bước 4 vẫn giữ để web không chết khi hub trục trặc, chỉ chậm hơn tối đa 10 giây.

Góc phải tiêu đề mỗi trang có nhãn `LiveDot`: xanh "Realtime" là đang nối hub, xám "Tự làm mới 10s" là đang dùng chế độ dự phòng.

## Vì sao dùng `useRef` cho `onChange`

Kết nối chỉ tạo **một lần** khi mở trang (`useEffect(..., [])`). Nếu dùng thẳng `onChange` thì kết nối sẽ giữ bản `onChange` cũ, ví dụ trang Điều phối đã đổi tab nhưng vẫn tải lại tab cũ. Lưu vào `handler.current` và cập nhật mỗi lần render thì lúc nào cũng gọi bản mới nhất.

## Phía backend

Đã có từ v0.8, xem `src/Infrastructure/Realtime/README.md`: hub `/hubs/delivery`, gửi event sau khi lưu DB, đọc token từ query `access_token`. Nhân viên nhận event của mọi đơn.
