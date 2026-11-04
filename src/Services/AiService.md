# AiService.cs

Trợ lý AI. Gọi LLM qua `ILlmClient` (cài đặt `Infrastructure/Services/LlmClient.cs`, chuẩn OpenAI Chat Completions).

## AskAsync(prompt) – `POST /api/v1/ai/feasibility-test`

Gửi system prompt "trợ lý logistics" + câu hỏi, trả `AiAnswer` (model, câu trả lời, độ trễ ms, token usage). Dùng để đo khả thi cho KAN-18.

## ChatAsync(message, customerScope) – `POST /api/v1/ai/chat`

Chatbot hỗ trợ khách dùng **function calling**. LLM không tự biết dữ liệu; nó chỉ được gọi 3 tool:

| Tool | Tham số | Service gọi | Trả về |
|---|---|---|---|
| `get_order_status` | `orderId` | `OrderService.GetAsync(customerScope, id)` | mã đơn, trạng thái, trạm, ETA, ngày tạo |
| `get_delivery_eta` | `orderId` | `OrderService.GetAsync(customerScope, id)` | ETA + trạng thái |
| `get_station_status` | `stationId` | `StationService.GetAsync(id)` | tên, tọa độ, trạng thái trạm |

Luồng:
1. Gửi system prompt + tin nhắn + danh sách tool.
2. LLM trả lời thẳng → xong. LLM đòi gọi tool → chạy tool, gửi kết quả lại cho LLM.
3. Tối đa 3 vòng, quá thì báo lỗi 502 (chặn LLM gọi tool vô hạn).

An toàn:
- Tool chạy **cùng quyền với người hỏi**: `customerScope` là id khách, nên khách hỏi đơn của người khác sẽ nhận "Không tìm thấy dữ liệu hoặc người dùng không có quyền truy cập." Nhân viên (`scope = null`) tra được mọi đơn.
- Id không phải UUID, tool lạ, JSON tham số hỏng → trả lỗi cho LLM thay vì làm sập request.
- Câu hỏi rỗng hoặc dài hơn 2000 ký tự → 400.

Bản cũ gọi vòng lại chính API qua HTTP; bản này gọi thẳng service nên nhanh hơn và không phụ thuộc token trong header.

Prompt và luật chatbot: `docs/ai/ai-chatbot-prompt.md`.
