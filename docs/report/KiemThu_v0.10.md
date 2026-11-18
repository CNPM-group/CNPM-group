# Kiểm thử v0.10 – test case bổ sung (AI)

Đồ án SmartDroneDelivery · Nhóm Mậu Huỳnh · Cập nhật 16/11/2026

Nối tiếp [KiemThu_v0.8.md](KiemThu_v0.8.md) (38 case) và [KiemThu_v0.9.md](KiemThu_v0.9.md) (18 case), vẫn giữ nguyên và vẫn phải chạy. File này chỉ ghi phần thêm ở v0.10, đánh số từ TC80. Tổng sau v0.10: **38 + 18 + 12 = 68 test case**.

Ký hiệu như các bản trước: **PM** = Postman (backend thật), **UI** = test giao diện web tự động với API giả lập, **FT** = `flutter test` (CI), **TC** = làm bằng tay.

## 1. Tóm tắt chuyến giao (KAN-62)

| ID | Kiểm gì | Mong đợi | Cách | Kết quả |
|---|---|---|---|---|
| TC80 | Điều phối gọi `POST /orders/{id}/ai-summary` cho đơn Hoàn tất | 200, `text` không rỗng, `source` là `llm` hoặc `rule` | PM | Chưa chạy |
| TC81 | Có API key: đối chiếu câu tóm tắt với dữ liệu đơn | Giờ cất cánh/hạ cánh, mã drone, số phút sớm/trễ khớp trang chi tiết đơn; không có chi tiết bịa | TC | Chưa chạy |
| TC82 | Không có API key | 200 (không phải 503), `source` = `rule`, `note` = "Chưa cấu hình LLM API...", câu có mã đơn | PM | Chưa chạy |
| TC83 | Khách tóm tắt đơn của khách khác | 404 | TC (2 tài khoản khách) | Chưa chạy |
| TC84 | Web: tab Hoàn tất → nút Tóm tắt AI | Hộp thoại hiện đoạn tóm tắt + nhãn "AI viết · model"; nút Viết lại, Đóng | UI | Đạt (giả lập) |
| TC85 | Web: đơn Thất bại, chưa có key | Nhãn "Tự ghép từ dữ liệu" + câu giải thích, nội dung có lý do thất bại | UI | Đạt (giả lập) |
| TC86 | App: chi tiết đơn Hoàn tất → Nhờ AI tóm tắt | Thẻ hiện đoạn tóm tắt và dòng nguồn; đơn đang bay không có thẻ này | TC | Chưa chạy |
| TC87 | App đọc JSON tóm tắt | `AiSummary.byLlm` đúng với `source` | FT | Chưa chạy (chạy khi CI chạy) |

## 2. Nhận xét vận hành

| ID | Kiểm gì | Mong đợi | Cách | Kết quả |
|---|---|---|---|---|
| TC88 | Khách gọi `GET /reports/ai-insights` | 403 | PM | Chưa chạy |
| TC89 | Quản lý gọi nhận xét; web bấm "Phân tích bằng AI" | API: 1–5 nhận xét không rỗng. Web: danh sách nhận xét + nhãn nguồn, nút đổi thành "Phân tích lại" | PM, UI | UI: Đạt (giả lập) · PM: Chưa chạy |

## 3. Sửa lỗi

| ID | Kiểm gì | Mong đợi | Cách | Kết quả |
|---|---|---|---|---|
| TC90 | Manager xóa trạm đã có đơn hàng (ví dụ trạm seed đã dùng trong Postman) | 409 "Trạm đã có đơn hàng hoặc drone dùng tới...", không phải 500 | TC (Swagger hoặc trang Drone & trạm) | Chưa chạy |
| TC91 | Manager xóa drone Idle đã từng bay | 409 "Drone đã có lịch sử chuyến bay...", không phải 500 | TC | Chưa chạy |

## Ghi kết quả

Không tự động kiểm được **nội dung** câu LLM viết (mỗi lần một khác), nên TC81 bắt buộc làm tay khi có API key: chọn ít nhất 1 đơn Hoàn tất và 1 đơn Thất bại, ghi lại câu AI viết và chỗ nào khớp/sai.
