# chat_page.dart

Màn **Trợ lý AI**: khách hỏi đáp với chatbot. Mở từ menu ⋮ trên trang "Đơn của tôi".

## Cách chạy

1. Khách gõ câu hỏi (hoặc bấm 1 trong 3 câu gợi ý lúc mới mở) → tin nhắn hiện bên phải, màu chủ đạo.
2. Gọi `Api.chat(text)` → `POST /api/v1/ai/chat { message }`. Thời gian chờ nâng lên **60 giây** cho riêng request này vì LLM trả lời chậm hơn API thường (mặc định 30 giây).
3. Câu trả lời hiện bên trái. Lỗi (chưa có API key → 503, quá giờ → 504...) hiện bằng bong bóng màu đỏ nhạt với câu lỗi server trả.
4. Đang chờ thì hiện "Đang trả lời..." và khóa nút gửi, tránh gửi dồn.

Chatbot phía server (`src/Services/AiService.cs`) tự gọi tool tra đơn, trạm, ETA của **chính khách đang đăng nhập** nên hỏi "đơn gần nhất của tôi tới đâu rồi" là trả lời được bằng dữ liệu thật.

## Lưu ý

- Lịch sử chat chỉ nằm trong màn hình, thoát ra là mất. Server không lưu theo phiên.
- Backend phải có `Llm__ApiKey` (biến `LLM_API_KEY` trong `.env`), không có thì mọi câu hỏi đều báo lỗi 503.
- Danh sách tin nhắn tự cuộn xuống cuối sau mỗi tin (`_scrollToEnd`, chạy sau khi vẽ xong khung hình).
