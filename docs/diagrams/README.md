# docs/diagrams

Ảnh sơ đồ dùng trong báo cáo (`../report/`) và Confluence. File `.mmd` là mã Mermaid gốc: sửa file `.mmd` rồi xuất lại PNG bằng `npx @mermaid-js/mermaid-cli -i 08-class.mmd -o 08-class.png`, hoặc dán mã vào https://mermaid.live.

| File | Sơ đồ | Mục báo cáo |
|---|---|---|
| `01-context-diagram.png` | Sơ đồ ngữ cảnh (DFD mức 0): 7 tác nhân, các luồng dữ liệu vào/ra hệ thống | III.1.1 |
| `02-usecase-tong-quat.png` | Use case gom theo 6 module | III.2.2 |
| `03-usecase-uml.png` | Use case diagram chuẩn UML: 7 actor, 31 use case | III.2.2 |
| `04-kien-truc.png` | Kiến trúc hệ thống: web, mobile, API, PostgreSQL, MinIO, LLM | IV.1.1 |
| `05-erd.png` | ERD logic 16 bảng, 3 cụm | IV.2 |
| `06-state-machine.png` | Vòng đời đơn: 9 trạng thái (khớp `src/Domain/Models/OrderStateMachine.cs`) | IV.3.1 |
| `07-sequence.png` | Sequence luồng giao trọn vẹn (bản thiết kế ban đầu) | IV.3.2 |
| `08-class.png` / `.mmd` | Sơ đồ lớp luồng đơn hàng theo 4 tầng (vẽ từ code thật) | IV.3.3 a |
| `09-seq-login.png` / `.mmd` | Sequence đăng nhập | IV.3.3 b |
| `10-seq-create-order.png` / `.mmd` | Sequence tạo đơn | IV.3.3 c |
| `11-seq-delivery.png` / `.mmd` | Sequence giao hàng: approve → schedule → start → arrive → confirm | IV.3.3 d |
| `12-seq-fail.png` / `.mmd` | Sequence giao thất bại và giao lại | IV.3.3 e |
| `13-bpmn.png` / `.mmd` | Lưu đồ nghiệp vụ 3 làn: Customer, Dispatcher, Station Operator | IV.3.3 f |
| `14-erd.png` / `.mmd` | ERD đầy đủ ký hiệu crow's foot, có PK/FK/UK | IV.2 |
