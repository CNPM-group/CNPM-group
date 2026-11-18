# AiSummaryDialog.jsx

Hộp thoại **Tóm tắt chuyến giao** (v0.10, KAN-62), mở từ nút "Tóm tắt AI" ở mỗi dòng của trang Điều phối.

- Mở lên là gọi `api.aiSummary(order.id)` → `POST /orders/{id}/ai-summary`.
- Đang chờ: hiện "AI đang đọc dữ liệu đơn và viết tóm tắt...", nút "Viết lại" khóa lại.
- Xong: hiện đoạn văn trong khung xanh nhạt, bên dưới là dòng nguồn (`AiSource`).
- Nút **Viết lại** gọi API lần nữa (LLM có thể viết câu khác); nút **Đóng** tắt hộp thoại.
- Lỗi (mất mạng, 404...) hiện khung đỏ trong hộp thoại.

Dùng lại `Dialog.jsx`: "Viết lại" là nút submit, "Đóng" là nút hủy (`cancelText`).
