# Station.jsx

Màn hình của nhân viên trạm, chia 2 cột thẻ:

## Tình trạng các trạm (v0.11, UC-18)

Dải thẻ trên cùng, mỗi trạm 1 thẻ: `đang tới + chờ nhận / sức chứa` (từ `GET /stations` và `GET /stations/load`). Đầy thì thẻ đỏ "· đầy": điều phối sẽ không gán thêm drone tới trạm đó được. Trạm bảo trì ghi "Bảo trì". Tự làm mới cùng lúc với danh sách đơn.

## Đang bay tới

Các đơn `InTransit` (`GET /orders?status=InTransit`). Mỗi thẻ có giờ dự kiến tới và nút **Drone đã tới trạm** → `POST /orders/{id}/arrive`.

Lúc này server:
- chuyển đơn sang `ArrivedAtStation`, drone về `Idle`;
- sinh mã OTP 6 số (chỉ lưu bản băm SHA-256, hạn 24 giờ);
- gửi thông báo có mã OTP cho khách. Khách xem mã trong màn Thông báo của app mobile.

**Tiến độ bay (v0.9):** mỗi thẻ có thanh tiến độ và dòng "DRONE-001 đã bay 43%". Lúc tải trang lấy bằng `GET /orders/{id}/tracking` cho từng đơn đang bay, sau đó cập nhật theo event SignalR `DroneLocation` (vài giây một lần) mà không gọi lại API. Tới 100 % thì ghi "đang chờ hạ cánh": vị trí là giả lập theo ETA, drone chỉ thật sự tới trạm khi nhân viên bấm nút.

## Chờ khách nhận

Các đơn `ArrivedAtStation`. Mỗi thẻ có ô nhập OTP:

- Chỉ nhận chữ số, tự cắt còn 6 số (dán "482 913" vẫn thành `482913`).
- Đủ 6 số mới bấm được **Xác nhận giao** → `POST /orders/{id}/confirm { otp }`.
- Đúng → đơn `Completed`, thẻ biến mất.
- Sai → thông báo đỏ với câu lỗi server trả. Sai quá 5 lần server khóa mã.

**Người nhận (v0.11):** thẻ hiện tên và số điện thoại người nhận (bấm số để gọi trên máy có hỗ trợ), lấy từ danh sách đơn. Khách tạo đơn bỏ trống thì là chính khách.

Mỗi thẻ giữ ô OTP riêng (`otp` là object `{ [orderId]: "..." }`) nên nhập ở thẻ này không lẫn sang thẻ khác.

## Quyền

Station Operator chỉ có nút Tới trạm và Xác nhận. Nút Báo lỗi nằm ở trang Điều phối vì API `/fail` chỉ cho Dispatcher, Manager, Admin.
