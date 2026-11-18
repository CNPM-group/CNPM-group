# notifications_page.dart

Màn Thông báo của khách. Mở từ icon chuông trên trang "Đơn của tôi" (chuông có số = số thông báo chưa đọc).

## Hiện gì

- Danh sách `GET /notifications`, mới nhất trên cùng.
- Chưa đọc: nền hơi xanh, chấm xanh, tiêu đề in đậm.
- Thông báo **"Hàng đã tới trạm"** có mã nhận hàng 6 số → hiện thêm ô to giữa màn, chữ cỡ 34, cách chữ rộng, để khách đọc cho nhân viên trạm. Mã chọn/copy được (`SelectableText`).

Mã lấy bằng getter `AppNotification.otp` trong `models.dart`: tìm dãy đúng 6 chữ số đứng riêng trong nội dung. Server không gửi mã ở trường riêng, mã chỉ nằm trong câu thông báo; DB chỉ lưu bản băm của mã nên không có API nào trả lại mã lần nữa.

## Thao tác

| Thao tác | Kết quả |
|---|---|
| Bấm 1 thông báo | `POST /notifications/{id}/read` (nếu chưa đọc), rồi mở chi tiết đơn liên quan |
| Kéo xuống | Tải lại |
| Có event SignalR / tới lượt làm mới 10 giây | Tự tải lại, nên đứng sẵn ở màn này lúc nhân viên bấm "Tới trạm" là mã hiện ra luôn |

Đánh dấu đã đọc bị lỗi thì vẫn cho mở đơn, không chặn khách.
