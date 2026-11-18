# addresses_page.dart

Màn **Sổ địa chỉ** của khách (KAN-67). Mở từ menu ⋮ trên trang "Đơn của tôi".

## Chức năng

| Thao tác | API |
|---|---|
| Xem danh sách (địa chỉ mặc định có ngôi sao vàng) | `GET /api/v1/customers/me/addresses` |
| Nút + → hộp thoại thêm | `POST /api/v1/customers/me/addresses` |
| Bấm 1 dòng → hộp thoại sửa | `PUT /api/v1/customers/me/addresses/{id}` |
| Menu ⋮ → Đặt làm mặc định | `PUT` với `isDefault: true`; server tự bỏ mặc định ở địa chỉ cũ |
| Menu ⋮ → Xóa (hỏi lại trước) | `DELETE /api/v1/customers/me/addresses/{id}` |

Hộp thoại (`_AddressDialog`) có 2 ô bắt buộc: tên gợi nhớ (≤ 50 ký tự, vd "Phòng trọ Bình Thạnh") và địa chỉ (≤ 255 ký tự), thêm ô tick "Đặt làm mặc định". Lỗi từ server hiện ngay trong hộp thoại, không đóng.

Tọa độ (`latitude`, `longitude`) chưa nhập trên app; khi sửa thì giữ nguyên giá trị cũ.

## Dùng ở đâu

Màn **Tạo đơn** (`create_order_page.dart`) có thêm ô chọn "Địa chỉ nhận (không bắt buộc)", tự chọn sẵn địa chỉ mặc định, gửi lên dạng `deliveryAddressId`. Server kiểm tra địa chỉ phải thuộc khách đang đăng nhập. Khách chưa có địa chỉ nào thì ô này không hiện.
