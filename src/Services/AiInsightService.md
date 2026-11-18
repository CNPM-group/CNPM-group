# AiInsightService.cs

Hai tính năng AI mới của v0.10, tách khỏi `AiService.cs` (chatbot) cho dễ đọc:

| Hàm | Ai gọi | Làm gì |
|---|---|---|
| `SummarizeOrderAsync(customerScope, orderId)` | `POST /orders/{id}/ai-summary` | **KAN-62**: viết 3–4 câu tóm tắt chuyến giao của 1 đơn |
| `InsightsAsync(from, to)` | `GET /reports/ai-insights` | Viết 3–5 nhận xét vận hành + việc nên làm cho quản lý |

## Luồng chung

```
DB (đơn, lịch sử, chuyến bay, drone / số liệu thống kê)
  → gom thành "facts" (record SummaryFacts / InsightFacts, giờ đã đổi sang giờ Việt Nam)
  → có LLM?  có  → gửi facts dạng JSON + prompt cho LLM → LLM viết thành câu      (source = "llm")
             không / LLM lỗi → server tự ghép câu theo luật từ chính facts đó     (source = "rule")
```

Điểm chính để giải thích khi thầy hỏi:

1. **LLM không tự tra gì**: server lấy dữ liệu trước rồi mới đưa, LLM chỉ diễn đạt lại. Prompt dặn "chỉ dùng dữ liệu trong JSON, không có thì không nhắc". Khác chatbot (`AiService`) là chatbot tự gọi tool.
2. **Không lộ dữ liệu nhạy cảm**: facts không có OTP, không có id người dùng. Lịch sử trạng thái không chứa OTP (xem câu ghi chú trong `DeliveryService`).
3. **Khách chỉ tóm tắt được đơn của mình**: dùng `OrderService.GetDetailAsync(customerScope, ...)`, đơn của người khác → 404 như các API khác.
4. **Không có API key vẫn chạy**: hàm `WriteAsync` kiểm `llm.IsConfigured`; chưa cấu hình hoặc LLM lỗi (`ExternalServiceException`: timeout, sai key, mạng) thì trả bản tự ghép, kèm `note` nói rõ lý do. Giao diện ghi "Tự ghép từ dữ liệu" chứ không giả là AI viết.
5. `temperature = 0.2` để câu chữ ổn định, `max_tokens` 400/600 để chặn trả lời dài và tốn phí.

## Bản tự ghép (luật)

`RuleSummary(facts)` ghép lần lượt: kiện gì, nặng bao nhiêu, đi từ đâu đến đâu, trạng thái hiện tại → bị từ chối thì lý do → drone nào cất cánh lúc nào, hạ cánh lúc nào, **sớm/trễ bao nhiêu phút so với ETA** → giao mấy lần → lý do thất bại → giờ khách nhập OTP và tổng thời gian.

`RuleInsights(facts)` lấy tối đa 5 ý theo thứ tự: tổng quan (số đơn, tỷ lệ thành công) → cảnh báo nếu tỷ lệ < 90 % (khi đã có ≥ 5 đơn kết thúc) kèm lý do thất bại phổ biến nhất → tuyến nhiều đơn nhất → số đơn còn chờ duyệt → thời gian giao trung bình → ngày đông nhất. Không có đơn nào thì nói thẳng "chưa đủ dữ liệu".

## Các record trong file

| Record | Là gì |
|---|---|
| `SummaryFacts` | Dữ liệu 1 đơn gửi LLM: mã đơn, trạng thái, 2 trạm, cân nặng, mô tả, các mốc giờ (tạo, duyệt, cất cánh, hạ cánh, ETA, hoàn tất), số phút trễ, tổng phút, mã drone, số lần giao, lý do thất bại, timeline |
| `InsightFacts` | Số liệu khoảng ngày: tổng/hoàn tất/thất bại/chờ duyệt, tỷ lệ, thời gian TB, ngày đông nhất, top 3 lý do thất bại, top 3 tuyến |
| `ReasonCount`, `RouteCount` | Cặp (lý do / tuyến, số lần) |

Kết quả trả ra là `AiSummary` / `AiInsights` (ở `Domain/Models/AiInsight.cs`).

## Hàm phụ

| Hàm | Làm gì |
|---|---|
| `WriteAsync(prompt, facts, maxTokens, rule)` | Gọi LLM, lỗi thì chạy `rule()`. Trả `(text, source, note)` |
| `ReadContent(doc)` | Lấy `choices[0].message.content` từ phản hồi chuẩn OpenAI |
| `Vn(utc)` | Đổi giờ UTC sang giờ Việt Nam (UTC+7) dạng `HH:mm dd/MM` |
| `StatusText(status)` | `Completed` → "hoàn tất"... |
| `FactsJson` | Tùy chọn JSON giữ nguyên chữ tiếng Việt (mặc định bị đổi thành `\uXXXX`, tốn token) |

## Thử nhanh

```bash
# token của dispatcher
curl -X POST http://localhost:5080/api/v1/orders/<id>/ai-summary -H "Authorization: Bearer <token>"
# token của manager
curl "http://localhost:5080/api/v1/reports/ai-insights" -H "Authorization: Bearer <token>"
```

Không có `LLM_API_KEY` thì `source` là `"rule"`, có key đúng thì `"llm"`.
