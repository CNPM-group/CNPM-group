# tracking_map.dart

Widget `TrackingMap` – bản đồ mini theo dõi drone ở màn chi tiết đơn (v0.9).

**Tự vẽ bằng `CustomPaint`**, không dùng gói bản đồ (Google Maps, flutter_map) nên không cần API key, không cần mạng tải ô bản đồ, không thêm thư viện.

## Vẽ gì

- Nền xám nhạt, đường thẳng nối trạm đi (chấm xanh) và trạm đến (chấm cam), tên 2 trạm.
- Đoạn đã bay tô đậm, chấm drone có quầng mờ.
- Dưới bản đồ: thanh tiến độ và dòng "DRONE-001 đã bay 45%" (100 % thì thêm "đang chờ hạ cánh"; đã tới trạm thì "Drone đã tới ...").

## Đổi tọa độ thành điểm trên màn

Lấy khung bao 2 trạm, phóng to **cùng một tỉ lệ cho cả vĩ độ và kinh độ** (để tuyến không bị méo), đặt tâm khung vào giữa ô 180 px, chừa lề 36 px. Vĩ độ tăng thì điểm đi lên (trục y màn hình ngược chiều).

Chỉ vẽ lại khi tọa độ drone hoặc trạng thái đổi (`shouldRepaint`).
