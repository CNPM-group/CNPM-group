# docs

Tài liệu của dự án. Code nằm ở `src/`, ở đây chỉ có mô tả.

| File | Nội dung | Dùng khi nào |
|---|---|---|
| `clean-architecture.md` | 4 tầng, bảng đối chiếu Flask ↔ C#, sequence diagram PlantUML, ví dụ tạo đơn đi qua từng tầng, các bước thêm chức năng | Đọc đầu tiên trước khi code |
| `CONVENTIONS.md` | Quy ước Git (nhánh, commit, PR), bí mật, API, code | Trước khi tạo nhánh / mở PR |
| `openapi.yaml` | Hợp đồng API v1 (endpoint, request, response, mã lỗi) | Web/mobile code theo file này; mở bằng https://editor.swagger.io |
| `schema.sql` | Thiết kế CSDL PostgreSQL (bảng, khóa, constraint) | Đối chiếu với `src/Infrastructure/Models` khi sửa bảng |
| `use-case.md` | Tác nhân và use case chính | Viết báo cáo, làm UI |
| `figma-link.md` | Link Figma và danh sách màn hình | Làm giao diện |
| `ai/ai-chatbot-prompt.md` | Prompt và luật của chatbot (chỉ dùng dữ liệu từ tool, không bịa) | Sửa `src/Services/AiService.cs` |
| `ai/KAN18_Feasibility_Report.md` | Mẫu báo cáo thử nghiệm LLM API (KAN-18) | Điền kết quả gọi `POST /api/v1/ai/feasibility-test` |
| `ai/KAN18_README.md` | Ghi chú cũ của KAN-18 (bản đầu, trước khi tách tầng) | Tham khảo lịch sử |
| `reports/KAN16_KAN17_SmartDroneDelivery.docx` | Báo cáo KAN-16, KAN-17 | Nộp / tham khảo |

Báo cáo chính (SPMP/SRS/SDD/Test/Release) nằm trên Confluence của nhóm.
