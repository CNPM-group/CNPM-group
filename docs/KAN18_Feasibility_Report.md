# KAN-18 — Báo cáo thử nghiệm tính khả thi của LLM API

**Dự án:** SmartDroneDelivery — AI-powered Drone Delivery Management Platform  
**Work item:** KAN-18 — Thử gọi LLM API 1 lần và báo cáo khả thi  
**Người thực hiện:** [Điền tên]  
**Ngày kiểm thử:** [Điền ngày]

## 1. Mục tiêu
Thực hiện một lời gọi LLM API từ backend C# để đánh giá sơ bộ khả năng tích hợp AI. KAN-18 tập trung vào thử nghiệm tính khả thi, chưa phải triển khai đầy đủ chatbot/function calling hoặc analytics.

## 2. Phương pháp
- Backend: ASP.NET Core Web API, C# và `IHttpClientFactory`.
- Endpoint: `POST /api/llm-feasibility/test`.
- Prompt tiếng Việt: yêu cầu nêu một lợi ích và một hạn chế khi dùng AI để ước tính thời gian giao hàng drone.
- Dữ liệu ghi nhận: HTTP status, độ trễ, model, số token nếu provider trả về, và nội dung phản hồi.
- Bảo mật: API key lấy từ biến môi trường `LLM_API_KEY`, không lưu trong mã nguồn.

## 3. Kết quả kiểm thử thực tế
**Điền sau khi chạy endpoint; không ghi số liệu giả định như kết quả thật.**

| Chỉ tiêu | Kết quả |
|---|---|
| Provider / endpoint | [Điền] |
| Model | [Điền] |
| HTTP status | [Điền] |
| Độ trễ (ms) | [Điền] |
| Input tokens | [Điền nếu có] |
| Output tokens | [Điền nếu có] |
| Chất lượng tiếng Việt | [Có/Không + nhận xét] |
| Lỗi phát sinh | [Không có / mô tả] |

## 4. Đánh giá

### Chi phí
Tính theo bảng giá hiện hành của provider:
`Chi phí = (input tokens / 1.000.000 × giá input/1M tokens) + (output tokens / 1.000.000 × giá output/1M tokens)`

Ghi rõ model, URL bảng giá và ngày kiểm tra. Nếu chưa có token usage hoặc bảng giá, kết luận chi phí chính xác chưa xác định.

### Độ trễ
Sử dụng `latencyMs` từ kết quả thực tế. Một lần gọi chỉ phản ánh thời điểm kiểm thử; cần nhiều lần đo để đánh giá trung bình và p95.

### Tiếng Việt
Đánh giá câu trả lời theo độ đúng trọng tâm, ngữ pháp, độ rõ ràng và việc tuân thủ yêu cầu. Đính kèm câu trả lời thật trong kết quả.

### Rủi ro
Phụ thuộc mạng, giới hạn tốc độ, trạng thái nhà cung cấp và giá. Cần timeout, xác thực, giới hạn tần suất, theo dõi chi phí và không gửi dữ liệu cá nhân nhạy cảm.

## 5. Kết luận
**Kết luận sau kiểm thử:** [Điền dựa trên kết quả thực tế.]

Tiêu chí đạt đề xuất:
1. Backend gửi request và nhận HTTP success.
2. Ghi nhận được độ trễ.
3. Mô hình trả lời prompt tiếng Việt phù hợp.
4. API key không bị lộ trong repository.
5. Có nguồn giá/token usage để ước tính chi phí.

## 6. Tài liệu tham khảo
- Tài liệu API chính thức: [Điền URL]
- Bảng giá chính thức: [Điền URL]
- Ngày truy cập: [Điền ngày]
