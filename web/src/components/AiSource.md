# AiSource.jsx

Dòng nhỏ dưới mọi nội dung AI trên web, cho biết đoạn văn do ai viết:

| `source` từ API | Hiện |
|---|---|
| `llm` | Nhãn xanh "AI viết · <tên model>" |
| `rule` | Nhãn xám "Tự ghép từ dữ liệu" + câu `note` (ví dụ "Chưa cấu hình LLM API...") |

Lý do có component này: khi chưa có API key, server vẫn trả nội dung (tự ghép theo luật) để demo không chết. Ghi rõ nguồn để không ai tưởng đó là AI viết.

Dùng ở `AiSummaryDialog.jsx` và trang `Dashboard.jsx`.
