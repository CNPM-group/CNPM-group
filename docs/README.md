# docs

Tài liệu của dự án. Code nằm ở `src/`, ở đây chỉ có mô tả.

| File | Nội dung | Dùng khi nào |
|---|---|---|
| `report/BaoCao_SmartDroneDelivery.md` | Báo cáo đồ án v0.6 (SPMP/SRS/SDD/Test/Release), đọc thẳng trên GitHub | Xem báo cáo, ôn trước khi thầy hỏi |
| `diagrams/` | 14 sơ đồ PNG + mã Mermaid `.mmd` (ngữ cảnh, use case, kiến trúc, ERD, state machine, lớp, sequence, BPMN) – xem [diagrams/README.md](diagrams/README.md) | Chèn vào báo cáo, slide |
| `clean-architecture.md` | 4 tầng, bảng đối chiếu Flask ↔ C#, sequence diagram PlantUML, ví dụ tạo đơn đi qua từng tầng, các bước thêm chức năng | Đọc đầu tiên trước khi code |
| `CONVENTIONS.md` | Quy ước Git (nhánh, commit, PR), bí mật, API, code | Trước khi tạo nhánh / mở PR |
| `openapi.yaml` | Hợp đồng API v1 (endpoint, request, response, mã lỗi) | Web/mobile code theo file này; mở bằng https://editor.swagger.io |
| `schema.sql` | Thiết kế CSDL PostgreSQL (bảng, khóa, constraint) | Đối chiếu với `src/Infrastructure/Models` khi sửa bảng |
| `use-case.md` | Tác nhân và use case chính | Viết báo cáo, làm UI |
| `figma-link.md` | Link Figma và danh sách màn hình | Làm giao diện |
| `SmartDroneDeliveryproject-overview/project-overview/01-gioi-thieu-de-tai.md` | Giới thiệu đề tài: lý do, mục tiêu, đối tượng | Viết báo cáo phần I |
| `SmartDroneDeliveryproject-overview/project-overview/02-khao-sat-va-phan-tich-nghiep-vu.md` | Khảo sát và phân tích nghiệp vụ | Viết báo cáo phần I–III |
| `SmartDroneDeliveryproject-overview/project-overview/03-phan-tich-yeu-cau-he-thong.md` | Phân tích yêu cầu hệ thống | Viết báo cáo phần III |
| `SmartDroneDeliveryproject-overview/project-overview/04-thiet-ke-he-thong.md` | Thiết kế hệ thống | Viết báo cáo phần IV |
| `SmartDroneDeliveryproject-overview/project-overview/05-cong-nghe-va-cong-cu.md` | Công nghệ và công cụ | Viết báo cáo phần IV |
| `SmartDroneDeliveryproject-overview/project-overview/06-xay-dung-va-trien-khai.md` | Xây dựng và triển khai, phân công, kết luận | Viết báo cáo phần V–VI |
| `SmartDroneDeliveryproject-overview/project-overview/07-kiem-thu-va-danh-gia.md` | Kiểm thử và đánh giá | Viết báo cáo phần V |
| `SmartDroneDeliveryproject-overview/project-overview/08-ket-qua-dat-duoc.md` | Kết quả đạt được | Viết báo cáo phần cuối |
| `SmartDroneDeliveryproject-overview/project-overview/09-ket-luan-va-huong-phat-trien.md` | Kết luận và hướng phát triển | Viết báo cáo phần cuối |
| `analytics/KPI_Definitions.md` | Định nghĩa các KPI (tỷ lệ thành công, thời gian giao...) | Làm dashboard |
| `analytics/analytics_queries.sql` | Câu SQL thống kê đơn theo trạng thái, tỷ lệ thành công | Làm dashboard, kiểm tra số liệu |
| `ai/ai-chatbot-prompt.md` | Prompt và luật của chatbot (chỉ dùng dữ liệu từ tool, không bịa) | Sửa `src/Services/AiService.cs` |
| `ai/KAN18_Feasibility_Report.md` | Mẫu báo cáo thử nghiệm LLM API (KAN-18) | Điền kết quả gọi `POST /api/v1/ai/feasibility-test` |
| `reports/KAN16_KAN17_SmartDroneDelivery.docx` | Báo cáo KAN-16, KAN-17 | Nộp / tham khảo |

Báo cáo chính để chỉnh sửa nằm trên Confluence của nhóm; `report/` là bản chép để đọc trên GitHub. Thay đổi qua từng lần cập nhật code: [../CHANGELOG.md](../CHANGELOG.md).
