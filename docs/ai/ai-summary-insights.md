# KAN-62 + nhận xét vận hành: prompt và luật

Tài liệu cho 2 tính năng AI thêm ở v0.10. Code: `src/Services/AiInsightService.cs` (giải thích chi tiết ở `AiInsightService.md` cùng thư mục).

## 1. Khác gì chatbot

| | Chatbot (`/ai/chat`) | Tóm tắt + nhận xét (v0.10) |
|---|---|---|
| Ai tra dữ liệu | LLM tự gọi tool | Server lấy dữ liệu trước, đưa sẵn cho LLM |
| LLM làm gì | Hiểu câu hỏi, chọn tool, trả lời | Chỉ diễn đạt dữ liệu thành câu |
| Không có API key | 503 | Server tự ghép câu theo luật (`source = "rule"`) |

Cách "đưa dữ liệu sẵn" an toàn hơn cho 2 việc này: LLM không thể tra nhầm đơn của người khác, và số liệu trong câu luôn lấy từ DB.

## 2. Prompt tóm tắt chuyến giao

```
Bạn viết tóm tắt một chuyến giao hàng bằng drone cho nhân viên và khách của SmartDroneDelivery.
Viết tiếng Việt, 3 đến 4 câu, văn xuôi, không dùng markdown, không gạch đầu dòng.
Chỉ dùng dữ liệu trong JSON người dùng gửi; trường nào không có thì không nhắc tới, không đoán.
Mọi mốc giờ trong dữ liệu đã là giờ Việt Nam.
Nếu giao trễ so với dự kiến hoặc thất bại thì nói rõ, kèm lý do nếu dữ liệu có.
```

Dữ liệu gửi kèm (ví dụ, rút gọn):

```json
{ "TrackingCode": "SDD-9912ff03d18a4c6e", "Status": "Completed", "Origin": "Trạm Thư viện", "Destination": "Trạm KTX Khu A",
  "WeightKg": 1.0, "DepartedAt": "09:12 16/11", "ArrivedAt": "09:24 16/11", "EtaAt": "09:26 16/11", "LateMinutes": -2,
  "CompletedAt": "09:31 16/11", "TotalMinutes": 19, "DroneCode": "DRONE-002", "Attempts": 1,
  "Timeline": ["09:12 16/11: chờ duyệt", "..."] }
```

Không gửi: OTP, id khách, email, số điện thoại.

## 3. Prompt nhận xét vận hành

```
Bạn là chuyên viên phân tích vận hành của SmartDroneDelivery (giao hàng bằng drone giữa các trạm).
Từ số liệu JSON người dùng gửi, viết 3 đến 5 nhận xét bằng tiếng Việt cho quản lý.
Mỗi nhận xét 1 dòng, bắt đầu bằng "- ", gồm điều thấy được trong số liệu và 1 việc nên làm.
Chỉ dựa vào số liệu được gửi, không bịa con số. Ít dữ liệu thì nói thẳng là chưa đủ để kết luận.
```

Số liệu gửi kèm: tổng đơn, hoàn tất, thất bại, chờ duyệt, tỷ lệ thành công, thời gian giao trung bình, ngày đông nhất, số đơn theo trạng thái, 3 lý do thất bại hay gặp, 3 tuyến đông nhất. Server tách câu trả lời theo dòng thành danh sách `points`.

## 4. Khi không gọi được LLM

| Tình huống | Kết quả |
|---|---|
| Chưa có `LLM_API_KEY` | `source = "rule"`, `note` = "Chưa cấu hình LLM API, nội dung do server tự ghép từ dữ liệu." |
| LLM lỗi (sai key, hết hạn mức, timeout, mất mạng) | `source = "rule"`, `note` ghi lỗi |
| LLM trả rỗng | `source = "rule"` |

Web và app luôn hiện nhãn nguồn ("AI viết · model" hoặc "Tự ghép từ dữ liệu"), không giả là AI viết.

## 5. Kiểm thử

- Không key: Postman nhóm 9 kiểm `source` là `llm` hoặc `rule`, `text` không rỗng và có mã đơn.
- Có key: gọi tay vài đơn Hoàn tất / Thất bại, đối chiếu từng con số trong câu với trang chi tiết đơn (giờ, mã drone, số phút trễ). Ghi kết quả vào `docs/report/KiemThu_v0.10.md` (TC80–TC83).
