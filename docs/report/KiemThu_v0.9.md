# Kiểm thử v0.9 – test case bổ sung

Đồ án SmartDroneDelivery · Nhóm Mậu Huỳnh · Cập nhật 15/11/2026

Nối tiếp bảng 38 test case của [KiemThu_v0.8.md](KiemThu_v0.8.md) (TC01–TC55, vẫn giữ nguyên, vẫn phải chạy). File này chỉ ghi phần **thêm ở v0.9**: theo dõi drone và quản lý tài khoản, đánh số từ TC60. Tổng sau v0.9: **38 + 18 = 56 test case**.

Ký hiệu cách kiểm như v0.8: **PM** = Postman collection (chạy trên backend thật), **UI** = test giao diện web tự động với API giả lập, **FT** = `flutter test` (chạy trong CI), **TC** = làm bằng tay.

## 1. Bộ giả lập và API theo dõi

| ID | Kiểm gì | Mong đợi | Cách | Kết quả |
|---|---|---|---|---|
| TC60 | `GET /orders/{id}/tracking` lúc đơn vừa cất cánh | 200, `status` = `InTransit`, có `droneCode`, `progressPct` trong 0–100, có tọa độ 2 trạm | PM | Chưa chạy |
| TC61 | Tracking sau khi xác nhận OTP | `progressPct` = 100, tọa độ drone **bằng đúng** tọa độ trạm đến | PM | Chưa chạy |
| TC62 | Khách B gọi tracking đơn của khách A | 404 (không lộ đơn tồn tại) | TC (2 tài khoản khách) | Chưa chạy |
| TC63 | Đơn chưa cất cánh (`Approved`) | `progressPct` = 0, drone ở trạm đi | TC | Chưa chạy |
| TC64 | Bộ giả lập chạy khi có đơn đang bay | Bảng `flight_telemetry` có dòng mới khoảng 3 giây/lần; `drone_flights.current_latitude` đổi | TC (xem DB bằng `docker compose exec db psql`) | Chưa chạy |
| TC65 | Drone bay quá ETA mà trạm chưa bấm "tới trạm" | Đứng ở 100 %, đơn vẫn `InTransit` (không tự đổi trạng thái) | TC (để `Simulator__TimeScale=10`) | Chưa chạy |
| TC66 | `Simulator__Enabled=false` | Không có dòng `flight_telemetry` mới; API tracking vẫn trả % tính theo giờ | TC | Chưa chạy |

## 2. Hiển thị theo dõi

| ID | Kiểm gì | Mong đợi | Cách | Kết quả |
|---|---|---|---|---|
| TC67 | Trang Trạm có đơn đang bay | Thanh tiến độ "DRONE-001 đã bay 43%" | UI | Đạt (giả lập) |
| TC68 | Nhận event `DroneLocation` | Thanh % trên web tăng, không cần F5 | TC | Chưa chạy |
| TC69 | App: chi tiết đơn đang bay | Thẻ "Theo dõi drone", chấm drone di chuyển dần sang trạm đến | TC | Chưa chạy |
| TC70 | Đọc JSON tracking + áp event trên app | `TrackingInfo.fromJson` đúng; `moved()` đổi vị trí và %, giữ trạm và mã drone | FT | Chưa chạy (chạy khi CI chạy) |

## 3. Quản lý tài khoản (Admin)

| ID | Kiểm gì | Mong đợi | Cách | Kết quả |
|---|---|---|---|---|
| TC71 | Manager gọi `GET /users` | 403 | PM | Chưa chạy |
| TC72 | Menu "Tài khoản" | Chỉ Admin thấy; Manager gõ thẳng `/users` bị đưa về trang chính | UI | Đạt (giả lập) |
| TC73 | Tạo tài khoản nhân viên mới (PM: Điều phối, UI: Trạm) | Hiện trong bảng, đúng vai trò, đang hoạt động | PM, UI | UI: Đạt (giả lập) · PM: Chưa chạy |
| TC74 | Thêm vai trò thứ 2 (PM: Điều phối + Trạm, UI: Điều phối + Quản lý) | Có 2 chip vai trò; đăng nhập lại thấy trang của cả 2 vai trò (kiểm tay) | PM, UI | UI: Đạt (giả lập) · PM: Chưa chạy |
| TC75 | Khóa tài khoản | Hàng mờ (class `inactive`), chữ "Đã khóa", nút đổi thành "Mở khóa"; đăng nhập bị từ chối (403 "Tài khoản đã bị khóa.") | PM, UI | UI: Đạt (giả lập) · PM: Chưa chạy |
| TC76 | Admin tự khóa / tự gỡ quyền Admin | Web: nút bị vô hiệu ở hàng của mình; API: 409 | PM, UI | UI: Đạt (giả lập) · PM: Chưa chạy |
| TC77 | Nút demo `admin@drone.local` ở trang đăng nhập | Điền sẵn, đăng nhập vào Thống kê, menu có "Tài khoản" | UI | Đạt (giả lập) |

## Ghi kết quả

"Đạt (giả lập)" = test giao diện web với API giả, chứng minh web gọi đúng API và hiện đúng, **chưa** chứng minh backend chạy đúng. Trước buổi báo cáo: chạy `npx newman run docs/postman/SmartDrone.postman_collection.json` trên máy có Docker, điền lại cột Kết quả các dòng PM, rồi làm tay các dòng TC.
