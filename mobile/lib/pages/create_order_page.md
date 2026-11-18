# create_order_page.dart

Màn **Tạo đơn**.

1. Chọn trạm đi, trạm đến (`GET /stations`), không được trùng nhau.
2. Chọn **địa chỉ nhận** từ sổ địa chỉ (không bắt buộc, tự chọn sẵn địa chỉ mặc định) – v0.8.
3. Nhập cân nặng (≤ 50 kg) và 3 kích thước. Chọn đủ 2 trạm + cân nặng là hiện ngay ô **"Bay khoảng X phút · Y km"** (`GET /orders/eta`, chờ 0,5 giây sau lần gõ cuối mới gọi) – v0.8.
4. Mô tả, ghi chú, **ảnh kiện** (chụp hoặc chọn trong máy, `image_picker`).
5. Bấm Đặt đơn: upload ảnh trước (`POST /packages/images`) lấy `imageKey`, rồi `POST /orders`. Upload ảnh lỗi thì vẫn tạo đơn không kèm ảnh và báo cho khách (v0.7).

Thành công thì đóng màn, danh sách đơn tự tải lại.

## Người nhận (v0.11)

Thêm 2 ô **Tên người nhận**, **SĐT người nhận** (không bắt buộc) trước ô ghi chú. Gửi kèm `recipientName`, `recipientPhone`. Bỏ trống thì server lấy tên và số điện thoại trong hồ sơ của khách (quy tắc nghiệp vụ "đơn phải có người nhận"). Số điện thoại được kiểm 8–15 chữ số ngay trên máy.

## Sửa đơn (v0.11)

Cùng màn này dùng để sửa đơn còn Chờ duyệt: `CreateOrderPage(editing: order)`.

- `_prefill` điền trạm đi / đến, địa chỉ, kiện hàng, ghi chú, người nhận từ đơn cũ.
- Không chọn ảnh mới thì giữ `imageKey` cũ; chọn ảnh mới thì upload rồi thay.
- Bấm **Lưu thay đổi** → `Api.updateOrder` (`PUT /orders/{id}`). Đơn đã được duyệt trong lúc đang sửa → server trả 409, màn hiện câu lỗi.

