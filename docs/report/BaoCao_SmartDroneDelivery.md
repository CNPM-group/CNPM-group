# Báo cáo đồ án SmartDroneDelivery

Bản v1.0 · cập nhật 16/11/2026 · Tiến

## Mục lục

- Thông tin chung
  - Thuật ngữ và viết tắt
- I. Giới thiệu dự án
  - 1. Tổng quan
  - 2. Bối cảnh sản phẩm
  - 3. Hệ thống hiện có
  - 4. Cơ hội
  - 5. Tầm nhìn sản phẩm
  - 6. Phạm vi và giới hạn
- II. Kế hoạch quản lý dự án
  - 1. Tổng quan
  - 2. Phương pháp quản lý
  - 3. Sản phẩm bàn giao
  - 4. Phân công trách nhiệm
  - 5. Giao tiếp dự án
  - 6. Quản lý cấu hình
- III. Đặc tả yêu cầu phần mềm (SRS)
  - 1. Tổng quan sản phẩm
  - 2. Yêu cầu người dùng
  - 3. Yêu cầu chức năng
  - 4. Yêu cầu phi chức năng
  - 5. Phụ lục yêu cầu
- IV. Mô tả thiết kế phần mềm (SDD)
  - 1. Thiết kế hệ thống
  - 2. Thiết kế cơ sở dữ liệu
  - 3. Thiết kế chi tiết
- V. Tài liệu kiểm thử
  - 1. Phạm vi kiểm thử
  - 2. Chiến lược kiểm thử
  - 3. Kế hoạch kiểm thử
  - 4. Test case chính
  - 5. Báo cáo kiểm thử
- VI. Gói bàn giao và hướng dẫn
  - 1. Gói bàn giao
  - 2. Hướng dẫn cài đặt
  - 3. Hướng dẫn sử dụng
- Lịch sử phiên bản
- Tình trạng theo yêu cầu đề
- Nguồn tham khảo

## Thông tin chung

Đây là bản v1.0 của báo cáo: đủ 6 phần theo mẫu SPMP/SRS/SDD/Test/Release, nội dung **khớp với code bản v0.11** (16/11/2026). Chức năng nào trong đặc tả chưa làm đều ghi rõ "chưa làm" ngay tại chỗ và gom lại ở mục *Tình trạng theo yêu cầu đề* cuối báo cáo. Chi tiết từng đợt cập nhật: các báo cáo `BaoCao_v0.7` … `BaoCao_v0.11` và `CHANGELOG.md`.

| Mục | Nội dung |
| --- | --- |
| Tên đề tài | AI-powered Drone Delivery Management Platform (Nền tảng quản lý giao hàng bằng drone tích hợp AI) |
| Tên sản phẩm | SmartDroneDelivery |
| Trường | Đại học Giao thông vận tải TP.HCM (UTH) |
| Giảng viên hướng dẫn | Nguyễn Văn Chiến |
| Nhóm | Nguyễn Ngọc Trần Hoàng Tiến (trưởng nhóm), Nguyễn Thiện Vinh, Nguyễn Mậu Huỳnh, Lê Công Anh Đức, Lê Công Anh Tài |
| Thời gian | 12/10/2026 – 29/11/2026, 7 tuần (ngày bảo vệ cần xác nhận) |
| Mã nguồn | github.com/nguyenmauhuynh/CNPM-group |
| Quản lý công việc | Jira, project KAN (cnpm-group.atlassian.net) |

### Thuật ngữ và viết tắt

| Viết tắt | Đầy đủ | Ý nghĩa trong báo cáo |
| --- | --- | --- |
| API | Application Programming Interface | Giao diện để web, app và hệ thống ngoài gọi vào backend |
| BR | Business Rule | Quy tắc nghiệp vụ, đánh số BR-xx |
| DTO | Data Transfer Object | Đối tượng dữ liệu trao đổi qua API |
| ERD | Entity Relationship Diagram | Sơ đồ quan hệ thực thể của cơ sở dữ liệu |
| ETA | Estimated Time of Arrival | Thời gian giao dự kiến |
| JWT | JSON Web Token | Token xác thực người dùng |
| LLM | Large Language Model | Mô hình ngôn ngữ lớn dùng cho tóm tắt và trợ lý AI |
| OTP | One-Time Password | Mã 6 số khách dùng để nhận hàng tại trạm |
| RBAC | Role-Based Access Control | Phân quyền theo vai trò |
| SDD | Software Design Description | Tài liệu thiết kế phần mềm (phần IV) |
| SPMP | Software Project Management Plan | Kế hoạch quản lý dự án (phần II) |
| SRS | Software Requirement Specification | Đặc tả yêu cầu phần mềm (phần III) |
| UC | Use Case | Ca sử dụng, đánh số UC-xx |
| UAT | User Acceptance Test | Kiểm thử chấp nhận với người dùng |

## I. Giới thiệu dự án

SmartDroneDelivery là nền tảng quản lý nghiệp vụ giao hàng bằng drone: quản lý đơn, gói hàng, trạm hạ cánh, theo dõi và xác nhận giao, không phải phần mềm điều khiển drone.

### 1. Tổng quan

#### 1.1 Thông tin dự án

- Tên dự án: AI-powered Drone Delivery Management Platform
- Tên sản phẩm: SmartDroneDelivery
- Loại phần mềm: Web App + Mobile App + RESTful API
- Mã project Jira: KAN

#### 1.2 Nhóm dự án

| Họ tên | Vai trò | Phụ trách chính |
| --- | --- | --- |
| Nguyễn Văn Chiến | Giảng viên hướng dẫn |  |
| Nguyễn Ngọc Trần Hoàng Tiến | Trưởng nhóm | Kiến trúc, API spec, state machine, Docker, quản lý dự án |
| Nguyễn Thiện Vinh | Thành viên | Backend, cơ sở dữ liệu, SignalR |
| Nguyễn Mậu Huỳnh | Thành viên | Web React |
| Lê Công Anh Đức | Thành viên | Mobile Flutter |
| Lê Công Anh Tài | Thành viên | Dịch vụ AI, kiểm thử |

### 2. Bối cảnh sản phẩm

Thương mại điện tử và logistics thông minh làm tăng nhu cầu giao hàng chặng cuối nhanh và tự động. Drone phù hợp với gói nhẹ trong khuôn viên trường, khu công nghiệp, khu dân cư và vùng xa.

Vận hành giao hàng bằng drone cần nhiều hơn việc bay: phải quản lý khách hàng, yêu cầu giao, thông tin gói hàng, trạm hạ cánh, theo dõi và xác nhận giao. Các hệ thống logistics hiện có thiếu luồng nghiệp vụ riêng cho drone nên khó điều phối và giám sát.

### 3. Hệ thống hiện có

#### 3.1 Zipline

- Mô tả: công ty giao hàng bằng drone lớn nhất; tháng 01/2026 công bố đã vượt 2 triệu lượt giao thương mại và gọi thêm hơn 600 triệu USD ([Zipline](https://www.zipline.com/newsroom/zipline-surpasses-2-million-deliveries-raises-more-than-600m-to-power-next-phase-of-growth-and-expands-operations-to-houston-and-phoenix)).
- Chức năng: khách đặt qua app Zipline; giao đồ ăn, hàng bán lẻ, hàng y tế tới nhà; thời gian bay trung vị 3 phút.
- Ưu điểm: quy mô lớn, giao nhanh, dịch vụ hoàn chỉnh.
- Hạn chế với bài toán của nhóm: hệ thống đóng, gắn với drone và hạ tầng riêng; không cung cấp nền tảng quản lý để đơn vị khác tự vận hành.

#### 3.2 Wing (Alphabet)

- Mô tả: đầu năm 2026 cùng Walmart mở rộng giao hàng bằng drone thêm 150 cửa hàng tại Mỹ, kế hoạch hơn 270 điểm vào năm 2027 ([Axios](https://www.axios.com/2026/01/11/walmart-drone-delivery-wing-los-angeles-miami)).
- Chức năng: đặt qua app Wing; hàng được nạp lên drone tại khu vực rào riêng ở bãi xe cửa hàng; drone bay tự động, phi công giám sát từ trung tâm điều hành.
- Ưu điểm: tách rõ điểm xuất phát cố định và trung tâm giám sát, gần với mô hình trạm + Dispatcher của nhóm.
- Hạn chế: gắn chặt với đối tác bán lẻ, không mở cho mục đích học tập hay tùy biến quy trình.

#### 3.3 Matternet

- Mô tả: giải pháp giao hàng y tế giữa các cơ sở bệnh viện gồm drone M2, trạm Matternet Station và nền tảng đám mây ([New Atlas](https://newatlas.com/drones/matternet-station-drone-delivery-hub-medical)).
- Chức năng: nhân viên lên lịch giao và gửi gói tại trạm; trạm nhận chứa tối đa 4 gói; người nhận được báo và quét mã nhân viên để lấy gói; điều hành viên theo dõi chuyến và tình trạng drone từ xa.
- Ưu điểm: mô hình trạm + xác thực người nhận rất giống luồng trạm hạ cánh + OTP của đề.
- Hạn chế: chuyên cho y tế, thuê bao theo năm, không có trợ lý AI cho khách.

### 4. Cơ hội

Các giải pháp trên đều là hệ thống đóng của doanh nghiệp vận hành drone. Một nền tảng quản lý nghiệp vụ độc lập với loại drone, có trạm hạ cánh, xác nhận nhận hàng và dịch vụ AI hỗ trợ điều phối phù hợp cho khuôn viên trường, khu công nghiệp hoặc doanh nghiệp logistics nhỏ muốn thử nghiệm giao hàng bằng drone.

### 5. Tầm nhìn sản phẩm

Dành cho đơn vị muốn vận hành giao hàng bằng drone trong một khu vực có trạm cố định, SmartDroneDelivery là nền tảng web + mobile quản lý trọn vòng đời đơn từ yêu cầu đến xác nhận nhận hàng. Khác với các hệ thống đóng, sản phẩm không phụ thuộc loại drone, có API mở cho tích hợp và có dịch vụ AI ước tính ETA, tóm tắt chuyến, trợ lý khách hàng và phân tích vận hành.

### 6. Phạm vi và giới hạn

#### 6.1 Tính năng chính

- FE01. Xác thực và phân quyền: đăng ký, đăng nhập JWT, 5 vai trò, quản lý tài khoản.
- FE02. Khách hàng và đơn giao: địa chỉ giao, tạo, sửa, hủy, tra cứu đơn.
- FE03. Gói hàng: khối lượng, kích thước, ảnh gói lưu MinIO, kiểm tra tải trọng.
- FE04. Quy trình giao: duyệt, từ chối, lên lịch, gán drone, bắt đầu chuyến, xử lý giao thất bại.
- FE05. Trạm hạ cánh: quản lý trạm, sức chứa, tình trạng, xác nhận gói tới trạm.
- FE06. Theo dõi và xác nhận giao: vị trí drone realtime (giả lập), OTP nhận hàng.
- FE07. Dashboard và báo cáo: thống kê đơn, tỷ lệ thành công, thời gian giao, xuất CSV.
- FE08. Dịch vụ AI: ước tính ETA, tóm tắt chuyến, trợ lý AI cho khách, phân tích vận hành.
- FE09. Quản trị hệ thống: quản lý tài khoản, nhật ký thao tác (audit log), sao lưu bằng script, kiểm tra sức khỏe `/health`. API key cho hệ thống ngoài để hướng phát triển (bảng `api_keys` đã có trong CSDL).

#### 6.2 Giới hạn và loại trừ

- LI-01: Không điều khiển drone thật; vị trí, pin, tốc độ do bộ giả lập trên server phát ra.
- LI-02: Drone bay đường thẳng giữa hai trạm; không tính vùng cấm bay, thời tiết, không phận thật.
- LI-03: Không có thanh toán trực tuyến.
- LI-04: Mỗi đơn chứa một gói hàng.
- LI-05: Chất lượng tóm tắt và trợ lý AI phụ thuộc dịch vụ LLM bên ngoài. Khi không có API key hoặc LLM lỗi: tóm tắt chuyến và nhận xét vận hành trả nội dung server tự ghép từ dữ liệu (ghi rõ nguồn), chatbot trả 503.
- LI-06: ETA tính bằng công thức (khoảng cách Haversine ÷ vận tốc + thời gian theo cân nặng), chưa học từ dữ liệu lịch sử.
- LI-07: Giao diện chỉ có tiếng Việt; demo chạy trên một máy bằng Docker Compose.

## II. Kế hoạch quản lý dự án

Tổng khối lượng ước tính khoảng 96 man-day cho 5 người trong 7 tuần (12/10–29/11), làm theo Scrum với sprint 1 tuần và demo nội bộ mỗi Chủ nhật.

### 1. Tổng quan

#### 1.1 Phạm vi và ước lượng (WBS)

| STT | Hạng mục | Độ phức tạp | Ước lượng (man-day) |
| --- | --- | --- | --- |
| 1 | **FE01. Xác thực và phân quyền** |  | **8** |
| 1.1 | Đăng ký, đăng nhập JWT | Simple | 3 |
| 1.2 | RBAC 5 vai trò, Admin gán role | Medium | 3 |
| 1.3 | Hồ sơ, đổi mật khẩu | Simple | 2 |
| 2 | **FE02. Khách hàng và đơn giao** |  | **12** |
| 2.1 | Địa chỉ giao hàng | Simple | 3 |
| 2.2 | Tạo, sửa, hủy đơn, kiểm tra tải trọng | Medium | 5 |
| 2.3 | Danh sách, lọc, chi tiết, lịch sử trạng thái | Medium | 4 |
| 3 | **FE03. Gói hàng** (thông tin + ảnh MinIO) | Medium | **4** |
| 4 | **FE04. Quy trình giao** |  | **13** |
| 4.1 | State machine đơn + lịch sử trạng thái | Complex | 5 |
| 4.2 | Duyệt, từ chối, gán drone, lên lịch | Medium | 5 |
| 4.3 | Xử lý giao thất bại | Medium | 3 |
| 5 | **FE05. Trạm hạ cánh** |  | **6** |
| 5.1 | CRUD trạm, drone | Simple | 3 |
| 5.2 | Tình trạng trạm, sức chứa, xác nhận gói tới | Medium | 3 |
| 6 | **FE06. Theo dõi và xác nhận giao** |  | **14** |
| 6.1 | SignalR + bộ giả lập drone | Complex | 6 |
| 6.2 | OTP nhận hàng | Medium | 4 |
| 6.3 | Bản đồ realtime trên web và app | Medium | 4 |
| 7 | **FE07. Dashboard và báo cáo** |  | **6** |
| 7.1 | API thống kê + xuất CSV | Medium | 3 |
| 7.2 | Màn dashboard | Medium | 3 |
| 8 | **FE08. Dịch vụ AI** |  | **10** |
| 8.1 | Ước tính ETA | Simple | 2 |
| 8.2 | Tóm tắt chuyến giao | Medium | 3 |
| 8.3 | Trợ lý AI cho khách | Complex | 5 |
| 9 | **FE09. Quản trị hệ thống** (audit log, backup, health check, Docker) | Medium | **5** |
| 10 | **Phân tích, thiết kế, tài liệu** (SRS, BPMN, ERD, API spec, báo cáo) | Medium | **10** |
| 11 | **Kiểm thử và triển khai** | Medium | **8** |
|  | **TỔNG** |  | **96** |

#### 1.2 Mục tiêu chất lượng

| # | Giai đoạn kiểm thử | Độ bao phủ | Số lỗi cho phép | Ghi chú |
| --- | --- | --- | --- | --- |
| 1 | Review | 100% tài liệu và PR được review | – | Review chéo qua Pull Request |
| 2 | Unit test | State machine, luật hồ sơ, công thức theo dõi, thống kê theo trạm, luật tự ghép của AI | 0 critical | xUnit (`tests/SmartDrone.Tests`) |
| 3 | Integration test | ≥ 80% endpoint trong openapi.yaml | 0 critical, ≤ 10 minor | Postman/Newman |
| 4 | System test | 100% kịch bản demo | 0 critical, ≤ 5 minor | Chạy trên Docker Compose |
| 5 | Acceptance test | 100% luồng chính của 5 vai trò | 0 critical/blocker | Demo với giảng viên |

#### 1.3 Rủi ro dự án

| # | Rủi ro | Ảnh hưởng | Khả năng | Xử lý |
| --- | --- | --- | --- | --- |
| 1 | Thành viên trễ việc hoặc vắng | Cao | Cao | Task kẹt quá 2 ngày thì chia lại; có người hỗ trợ cho từng mảng |
| 2 | Web, mobile và backend lệch API | Cao | Trung bình | openapi.yaml là hợp đồng chung; đổi API phải sửa file này trước |
| 3 | Ghép hệ thống dồn về cuối | Cao | Trung bình | Demo end-to-end mỗi Chủ nhật |
| 4 | Dịch vụ LLM lỗi, hết hạn mức, chậm | Trung bình | Trung bình | Timeout; tóm tắt và nhận xét tự ghép từ dữ liệu khi LLM lỗi |
| 5 | SignalR không ổn định trên mobile | Trung bình | Trung bình | Tự kết nối lại; dự phòng gọi API 10 giây/lần |
| 6 | Phạm vi phình ra | Trung bình | Cao | Đóng băng tính năng ngày 22/11 |
| 7 | Xung đột code, mất dữ liệu | Cao | Trung bình | Khóa nhánh main, bắt buộc PR có review |

### 2. Phương pháp quản lý

#### 2.1 Quy trình dự án

- Scrum với sprint 1 tuần, tổng 7 sprint (W1–W7).
- W1–W2: phân tích yêu cầu, thiết kế kiến trúc, CSDL, API, dữ liệu lõi.
- W3 (26/10–01/11): dọn repo, khung web React, state machine đơn, xác thực JWT, BPMN và ERD.
- W4 (02/11–08/11): lõi đơn hàng chạy thật trên web và app (FE01–FE04); mốc MVP 08/11.
- W5 (09/11–15/11): thực thi chuyến, tracking, xác nhận giao (FE05–FE06); mốc luồng giao trọn vẹn 15/11.
- W6 (16/11–22/11): dashboard, AI, hoàn thiện (FE07–FE09); đóng băng tính năng 22/11.
- W7 (23/11–29/11): kiểm thử hệ thống, triển khai, báo cáo, demo; kết thúc 29/11.
- Nghi thức: họp ngắn 15 phút thứ Hai, Tư, Sáu; lập kế hoạch và demo mỗi Chủ nhật.
- Công cụ: backlog và task trên Jira KAN, label tuần W1–W7, mỗi task có due date và tiêu chí "Xong khi".

#### 2.2 Quản lý chất lượng

- Phòng lỗi: quy ước trong docs/CONVENTIONS.md; ràng buộc CHECK, khóa ngoại, index trong CSDL; validate đầu vào ở API.
- Review: mọi thay đổi vào main qua Pull Request, ít nhất 1 người duyệt; CI (build + test backend, build web, `flutter analyze` + `flutter test`) phải xanh.
- Unit test: xUnit cho OrderStateMachine, luật hồ sơ, công thức theo dõi drone, thống kê theo trạm, luật tự ghép của AI (`tests/SmartDrone.Tests`).
- Integration test: Postman/Newman chạy toàn luồng API.
- System test: chạy kịch bản demo trên Docker Compose; test giao diện web tự động bằng Playwright (API giả lập). Test tải bằng k6 chưa làm.
- Acceptance: demo với giảng viên theo kịch bản 7 bước ở phần V.

#### 2.3 Kế hoạch đào tạo

| Nội dung | Người học | Thời điểm | Điều kiện miễn |
| --- | --- | --- | --- |
| ASP.NET Core Web API, EF Core | Vinh, Tiến | W1, 2 buổi | Đã làm project .NET |
| ReactJS + Vite | Huỳnh | W1, 2 buổi | Đã làm project React |
| Flutter + Dio | Đức | W1, 2 buổi | Đã làm app Flutter |
| Git, Pull Request, review code | Cả nhóm | W3, 1 buổi | Dùng Git thành thạo |
| Docker Compose | Cả nhóm | W1, 1 buổi | Đã dùng Docker |
| SignalR | Vinh, Huỳnh, Đức | W5, 1 buổi | – |
| Gọi LLM API, prompt, function calling | Tài | W2, 1 buổi | – |

### 3. Sản phẩm bàn giao

| # | Sản phẩm | Hạn | Ghi chú |
| --- | --- | --- | --- |
| 1 | Kế hoạch dự án, phân công | 18/10 | Đã có |
| 2 | SRS, use case, BPMN | 01/11 | Phần III |
| 3 | Kiến trúc, ERD, API spec | 01/11 | Phần IV, docs/openapi.yaml |
| 4 | Wireframe web và mobile | 18/10 | Figma, đã có |
| 5 | MVP: tạo đơn trên app, duyệt trên web | 08/11 | Mốc W4 |
| 6 | Luồng giao trọn vẹn có tracking realtime | 15/11 | Mốc W5 |
| 7 | Dashboard, AI, đóng băng tính năng | 22/11 | Mốc W6 |
| 8 | Báo cáo kiểm thử | 26/11 | Phần V |
| 9 | Gói Docker, hướng dẫn cài đặt và sử dụng, mã nguồn | 29/11 | Phần VI, mốc W7 |

### 4. Phân công trách nhiệm

D = thực hiện chính, R = review, S = hỗ trợ, I = được thông báo.

| Công việc | Tiến | Vinh | Huỳnh | Đức | Tài |
| --- | --- | --- | --- | --- | --- |
| Lập và theo dõi kế hoạch | D | I | I | I | I |
| Phân tích yêu cầu, SRS | D | S | S | S | R |
| Use case, khung báo cáo | R | I | I | I | D |
| Kiến trúc, API spec | D | R | I | I | I |
| CSDL, migration, seed | R | D | I | I | I |
| API nghiệp vụ backend | D | D | I | S | I |
| Web React | R | I | D | S | I |
| App Flutter | R | I | S | D | I |
| Dịch vụ AI | R | S | I | I | D |
| Kiểm thử | S | S | S | S | D |
| Docker, triển khai | D | S | I | I | R |
| Báo cáo cuối kỳ, demo | D | S | S | S | S |

### 5. Giao tiếp dự án

| Hoạt động | Người tham gia | Mục đích | Tần suất | Công cụ |
| --- | --- | --- | --- | --- |
| Họp ngắn | Cả nhóm | Làm gì xong, làm gì tiếp, kẹt ở đâu | Thứ Hai, Tư, Sáu, 15 phút | Google Meet / Zalo |
| Lập kế hoạch và demo sprint | Cả nhóm | Demo phần đã xong, chốt task tuần sau | Chủ nhật | Google Meet, Jira |
| Review code | Người viết + người duyệt | Kiểm tra logic, quy ước | Mỗi PR | GitHub Pull Request |
| Xử lý lỗi gấp | Người liên quan | Gỡ vướng ngay | Khi cần | Zalo |
| Báo cáo giảng viên | Trưởng nhóm | Cập nhật tiến độ, xin góp ý | Theo lịch của thầy | Email / gặp trực tiếp |
| Tập dượt demo | Cả nhóm | Canh giờ, chuẩn bị hỏi đáp | 2 lần trong W7 | Trực tiếp |

### 6. Quản lý cấu hình

#### 6.1 Tài liệu

- Tài liệu kỹ thuật (openapi.yaml, schema.sql, quy ước) nằm trong thư mục docs/ của repo, sửa qua PR.
- Báo cáo này đánh số phiên bản v0.1, v0.2…; mỗi lần cập nhật ghi vào mục Lịch sử phiên bản cuối báo cáo.

#### 6.2 Mã nguồn

- GitHub, repo CNPM-group. Nhánh `main` chỉ chứa code đã duyệt; nhánh làm việc đặt tên `feature/KAN-<số>-<mô-tả>`.
- Commit theo dạng `feat:`, `fix:`, `docs:`, kèm mã KAN.
- CI (GitHub Actions) trên mỗi PR: build + unit test backend, build web, `flutter analyze` + `flutter test`.

#### 6.3 Công cụ và hạ tầng

| Nhóm | Công cụ |
| --- | --- |
| Backend | ASP.NET Core 8 Web API, EF Core, ASP.NET Identity, SignalR |
| Web | ReactJS 18, Vite, React Router, CSS thuần |
| Mobile | Flutter, Dio, flutter\_secure\_storage, signalr\_netcore, image\_picker |
| Cơ sở dữ liệu, lưu trữ | PostgreSQL 16, MinIO |
| Triển khai | Docker, Docker Compose |
| Quản lý dự án | Jira (KAN), GitHub |
| Thiết kế | Figma, draw.io, dbdiagram.io |
| Kiểm thử | Swagger, Postman/Newman, xUnit, Playwright (web), flutter\_test |

## III. Đặc tả yêu cầu phần mềm (SRS)

Hệ thống có 5 tác nhân người và 1 tác nhân hệ thống ngoài, dùng 31 use case chia thành 6 module.

### 1. Tổng quan sản phẩm

- Môi trường: web portal chạy trên Chrome, Edge, Safari; app Android/iOS viết bằng Flutter; cần kết nối Internet, không có chế độ offline.
- Hạ tầng: ASP.NET Core Web API, PostgreSQL, MinIO, chạy bằng Docker Compose.
- Người dùng nội bộ: Dispatcher, Station Operator, Logistics Manager, System Administrator. Người dùng ngoài: Customer.
- Ràng buộc: giao diện tiếng Việt; dữ liệu bay do bộ giả lập phát ra; AI chỉ hỗ trợ quyết định, không tự đổi trạng thái đơn.
- Phụ thuộc: dịch vụ LLM bên ngoài cho tóm tắt và trợ lý AI.

#### 1.1 Sơ đồ ngữ cảnh (Context Diagram)

Toàn bộ hệ thống là một tiến trình duy nhất (0); 5 tác nhân người và 2 hệ thống bên ngoài chỉ trao đổi dữ liệu với nó qua các luồng dưới đây.

![context diagram (DFD mức 0) · 7 tác nhân, 39 luồng dữ liệu](../diagrams/01-context-diagram.png)

*Hình: context diagram (DFD mức 0) · 7 tác nhân, 39 luồng dữ liệu*

Chữ đậm màu là luồng đi vào hệ thống, chữ nhạt màu là luồng hệ thống trả ra. Bộ giả lập drone, PostgreSQL và MinIO nằm bên trong hệ thống nên không vẽ ở mức này.

### 2. Yêu cầu người dùng

#### 2.1 Tác nhân

| # | Tác nhân | Mô tả | Nền tảng |
| --- | --- | --- | --- |
| 1 | Customer | Tạo yêu cầu giao, quản lý địa chỉ, theo dõi đơn, nhận hàng bằng OTP, hỏi trợ lý AI | App |
| 2 | Dispatcher | Duyệt đơn, gán drone, lên lịch, bắt đầu chuyến, theo dõi chuyến, xử lý giao thất bại | Web |
| 3 | Station Operator | Xem tình trạng các trạm, xác nhận gói tới trạm, xác nhận khách lấy hàng | Web |
| 4 | Logistics Manager | Quản lý trạm, drone, tuyến; xem dashboard, báo cáo, phân tích AI | Web |
| 5 | System Administrator | Quản lý tài khoản, vai trò, xem nhật ký thao tác (API key: chưa làm) | Web |
| 6 | Hệ thống ngoài | Gọi RESTful API bằng API key để tạo và tra cứu đơn (**chưa làm**, để hướng phát triển) | API |

Trong code, vai trò lưu dạng chuỗi theo Roles.cs: Customer, Dispatcher, StationOperator, Manager, Admin.

#### 2.2 Sơ đồ và danh sách use case

![sơ đồ use case tổng quát · 6 module, 31 use case](../diagrams/02-usecase-tong-quat.png)

*Hình: sơ đồ use case tổng quát · 6 module, 31 use case*

Sơ đồ trên gom use case theo module để nhìn tổng quan; sơ đồ dưới là use case diagram theo chuẩn UML.

![use case diagram UML · 7 actor, 31 use case](../diagrams/03-usecase-uml.png)

*Hình: use case diagram UML · 7 actor, 31 use case*

Customer, Dispatcher, Station Operator, Manager, Admin đều kế thừa actor chung Người dùng nên cùng dùng được UC-01 và UC-03; Hệ thống bên ngoài không đăng nhập mà xác thực bằng API key nên không kế thừa.

| ID | Use case | Tác nhân | Mô tả |
| --- | --- | --- | --- |
| UC-01 | Đăng nhập, đăng xuất | Tất cả | Xác thực email + mật khẩu, cấp JWT, chuyển tới màn hình theo vai trò |
| UC-02 | Đăng ký tài khoản | Customer | Tạo tài khoản vai trò Customer |
| UC-03 | Hồ sơ, đổi mật khẩu | Tất cả | Xem, sửa họ tên, số điện thoại; đổi mật khẩu |
| UC-04 | Quản lý địa chỉ giao | Customer | Thêm, sửa, xóa, đặt mặc định địa chỉ có tọa độ |
| UC-05 | Tạo yêu cầu giao | Customer | Chọn trạm gửi, trạm nhận, nhập thông tin gói, ảnh gói |
| UC-06 | Xem ETA | Customer | Xem thời gian giao dự kiến trước và sau khi tạo đơn |
| UC-07 | Sửa, hủy đơn | Customer | Chỉ khi đơn còn Pending |
| UC-08 | Xem danh sách, chi tiết đơn | Customer, Dispatcher | Lọc, tìm kiếm, phân trang, xem lịch sử trạng thái |
| UC-09 | Theo dõi drone realtime | Customer, Station Operator | App: sơ đồ 2 trạm + chấm drone; web trạm: thanh % đã bay |
| UC-10 | Nhận hàng bằng OTP | Customer | Nhận mã OTP khi đơn tới trạm, đưa cho Station Operator |
| UC-11 | Chat với trợ lý AI | Customer | Hỏi về đơn, trạm, thời gian giao |
| UC-12 | Nhận thông báo | Customer | Thông báo khi đơn đổi trạng thái |
| UC-13 | Duyệt, từ chối đơn | Dispatcher | Duyệt đơn Pending; từ chối phải có lý do |
| UC-14 | Gán drone, lên lịch | Dispatcher | Chọn drone rảnh, đủ tải, pin ≥ 30 %, đủ tầm bay; trạm nhận còn chỗ |
| UC-15 | Bắt đầu chuyến | Dispatcher | Tạo chuyến bay, đơn sang InTransit |
| UC-16 | Xử lý giao thất bại | Dispatcher | Ghi lý do, lên lịch lại hoặc hủy hoàn trả |
| UC-17 | Xem tóm tắt chuyến | Dispatcher | AI tóm tắt diễn biến của đơn trong 3–5 câu |
| UC-18 | Xem tình trạng trạm | Station Operator | Số đơn đang tới + kiện chờ nhận so với sức chứa |
| UC-19 | Cập nhật trạng thái trạm | Manager, Admin | Available, Occupied, Maintenance (Station Operator tự đổi trạm mình: chưa làm) |
| UC-20 | Xác nhận gói tới trạm | Station Operator | Đơn sang ArrivedAtStation, hệ thống sinh OTP |
| UC-21 | Xác nhận khách lấy hàng | Station Operator | Nhập OTP khách đưa, đơn sang Completed |
| UC-22 | Quản lý trạm | Manager, Admin | Thêm, sửa, xóa trạm, sức chứa, tọa độ |
| UC-23 | Quản lý drone | Manager, Admin | Danh mục drone: tải tối đa, tầm bay, vận tốc, trạng thái |
| UC-24 | Quản lý tuyến bay | Manager | Tuyến giữa hai trạm, khoảng cách (**chưa làm**: có bảng và dữ liệu mẫu, chưa có API/màn) |
| UC-25 | Xem dashboard | Manager, Admin | Đơn theo trạng thái, ngày, trạm; tỷ lệ thành công |
| UC-26 | Xuất báo cáo CSV | Manager | Xuất số liệu theo khoảng thời gian |
| UC-27 | Xem phân tích vận hành AI | Manager | AI nhận xét xu hướng, điểm nghẽn từ số liệu tổng hợp |
| UC-28 | Quản lý tài khoản | Admin | Tạo, khóa, mở tài khoản, gán vai trò |
| UC-29 | Xem audit log | Admin | Ai làm gì, lúc nào, giá trị cũ và mới (trang Nhật ký) |
| UC-30 | Quản lý API key | Admin | Cấp, thu hồi API key cho hệ thống ngoài (**chưa làm**) |
| UC-31 | Tích hợp qua API | Hệ thống ngoài | Tạo và tra cứu đơn bằng API key (**chưa làm**) |

Tình trạng ở bản nộp: làm đủ 27/31 use case; UC-19 làm một phần (Manager/Admin đổi trạng thái trạm); chưa làm UC-24, UC-30, UC-31.

### 3. Yêu cầu chức năng

#### 3.1 Danh sách màn hình

| # | Màn hình | Nền tảng | Vai trò | Use case |
| --- | --- | --- | --- | --- |
| SC-01 | Đăng nhập | Web, App | Tất cả | UC-01 |
| SC-02 | Đăng ký | App | Customer | UC-02 |
| SC-03 | Hồ sơ cá nhân | Web, App | Tất cả | UC-03 |
| SC-04 | Địa chỉ giao | App | Customer | UC-04 |
| SC-05 | Tạo đơn | App | Customer | UC-05, UC-06 |
| SC-06 | Đơn của tôi | App | Customer | UC-07, UC-08 |
| SC-07 | Chi tiết đơn + timeline + theo dõi drone + tóm tắt AI | App | Customer | UC-08, UC-09, UC-10, UC-17 |
| SC-08 | Trợ lý AI | App | Customer | UC-11 |
| SC-09 | Bảng điều phối đơn | Web | Dispatcher | UC-08, UC-13 |
| SC-10 | Gán drone, lên lịch | Web | Dispatcher | UC-14, UC-15 |
| SC-11 | Theo dõi chuyến bay (gộp vào trang Trạm: thanh % đã bay) | Web | Station Operator, Dispatcher | UC-09 |
| SC-12 | Trạm (tình trạng các trạm, đơn đang tới, OTP) | Web | Station Operator | UC-18, UC-20, UC-21 |
| SC-13 | Drone & trạm (sức chứa, tải trạm) | Web | Manager, Admin | UC-19, UC-22, UC-23 |
| SC-13b | Khách hàng (thông tin + lịch sử đơn của khách) | Web | Dispatcher, Manager, Admin | FR03 |
| SC-14 | Dashboard, báo cáo | Web | Manager, Admin | UC-25 – UC-27 |
| SC-15 | Quản lý tài khoản | Web | Admin | UC-28 |
| SC-16 | Nhật ký thao tác | Web | Admin | UC-29 |

Ảnh màn hình web nằm trong các báo cáo cập nhật v0.7 → v0.11 (thư mục `docs/report/images`).

#### 3.2 Đăng nhập (UC-01)

- Kích hoạt: mở web hoặc app khi chưa đăng nhập.
- Tác nhân: tất cả vai trò.
- Mục đích: xác thực người dùng và xác định vai trò để mở đúng chức năng.
- Xử lý:
  1. Người dùng nhập email, mật khẩu.
  2. Gọi `POST /api/v1/auth/login`; hệ thống kiểm tra tài khoản còn hoạt động và mật khẩu đúng.
  3. Thành công: trả JWT chứa id, email, vai trò; web lưu trong `localStorage`, app lưu `flutter_secure_storage`.
  4. Chuyển tới màn hình chính theo vai trò; sai thông tin hiện MSG-02.
- Business rules: BR-01, BR-02.

#### 3.3 Tạo yêu cầu giao (UC-05, UC-06)

- Kích hoạt: Customer bấm "Tạo đơn" trên app.
- Tác nhân: Customer.
- Mục đích: tạo đơn giao mới ở trạng thái Pending.
- Xử lý:
  1. Chọn trạm gửi, trạm nhận (trạm đang bảo trì bị từ chối); chọn địa chỉ giao (không bắt buộc); nhập người nhận (bỏ trống thì là chính khách).
  2. Nhập mô tả gói, khối lượng (kg), dài, rộng, cao (cm), ghi chú; chụp ảnh gói (không bắt buộc), ảnh upload lên MinIO trước và trả về image key.
  3. App gọi API ETA để hiển thị thời gian dự kiến trước khi xác nhận.
  4. Gọi `POST /api/v1/orders`. Hệ thống kiểm tra hai trạm khác nhau, tồn tại và không bảo trì; khối lượng không vượt tải của drone lớn nhất; số điện thoại người nhận hợp lệ.
  5. Tạo bản ghi gói hàng + đơn, sinh mã vận đơn, ghi dòng đầu tiên vào lịch sử trạng thái, báo cho Dispatcher.
- Business rules: BR-03, BR-04, BR-05, BR-06, BR-27.

#### 3.4 Xem danh sách và chi tiết đơn (UC-08)

- Kích hoạt: mở màn "Đơn của tôi" (app) hoặc "Bảng điều phối" (web).
- Tác nhân: Customer, Dispatcher, Manager, Admin.
- Xử lý:
  1. Gọi `GET /api/v1/orders?status=&q=&page=&pageSize=`; đơn mới nhất xếp trước, phân trang phía server, `q` tìm theo mã đơn.
  2. Customer chỉ nhận đơn của mình; vai trò nội bộ thấy tất cả.
  3. Mở chi tiết: thông tin đơn, gói, hai trạm, người nhận, ETA, timeline từ `GET /orders/{id}/history`.
  4. Đơn còn Pending: khách bấm "Sửa đơn" trên app (`PUT /orders/{id}`).
- Business rules: BR-07.

#### 3.5 Hủy đơn (UC-07)

- Kích hoạt: Customer bấm "Hủy đơn" trong chi tiết đơn.
- Xử lý: hỏi xác nhận, gọi `POST /orders/{id}/cancel`; đơn sang Cancelled, ghi lịch sử.
- Business rules: BR-08, BR-10.

#### 3.6 Duyệt, từ chối đơn (UC-13)

- Kích hoạt: Dispatcher chọn đơn Pending trên bảng điều phối.
- Tác nhân: Dispatcher (Manager, Admin cũng được).
- Xử lý:
  1. Xem thông tin đơn, gói, ảnh gói, ETA.
  2. Duyệt: `POST /orders/{id}/approve`, đơn sang Approved, lưu người duyệt và thời điểm.
  3. Từ chối: bắt buộc nhập lý do, `POST /orders/{id}/reject`, đơn sang Rejected.
  4. Gửi thông báo cho Customer.
- Business rules: BR-10, BR-11.

#### 3.7 Gán drone và lên lịch (UC-14)

- Kích hoạt: Dispatcher mở đơn Approved, bấm "Gán drone".
- Xử lý:
  1. Web liệt kê drone Idle có tải tối đa ≥ khối lượng gói và pin ≥ 30 %; server kiểm lại cả tầm bay ≥ khoảng cách hai trạm.
  2. Dispatcher chọn drone (mặc định bay ngay; API nhận thêm giờ bay).
  3. Server kiểm trạm nhận không bảo trì và còn chỗ (đơn đang tới + kiện chờ khách < sức chứa), đầy → 409.
  4. Đơn sang Scheduled, drone sang Assigned, tạo bản ghi chuyến bay trạng thái Scheduled, tính ETA.
- Business rules: BR-12, BR-13, BR-15.

#### 3.8 Bắt đầu chuyến và theo dõi realtime (UC-15, UC-09)

- Kích hoạt: Dispatcher bấm "Bắt đầu chuyến" với đơn Scheduled.
- Xử lý:
  1. Đơn sang InTransit, chuyến bay sang Flying, drone sang InFlight.
  2. Bộ giả lập (`DroneSimulator`, chạy nền) mỗi 3 giây tính % đã bay theo giờ cất cánh và ETA, nội suy vị trí trên đường thẳng giữa hai trạm, ghi flight\_telemetry và gửi event `DroneLocation` qua SignalR tới nhóm nhân viên và khách chủ đơn.
  3. App vẽ sơ đồ 2 trạm + chấm drone + thanh %; web trang Trạm hiện thanh "đã bay N%".
  4. Mất kết nối SignalR thì client tự kết nối lại; web tự tải lại 10 giây/lần, app kéo xuống để tải lại.
- Business rules: BR-10, BR-14.

#### 3.9 Xác nhận gói tới trạm (UC-20)

- Kích hoạt: drone giả lập tới trạm nhận; Station Operator xác nhận đã nhận gói.
- Xử lý: chuyến bay sang Landed, drone về Idle tại trạm nhận, đơn sang ArrivedAtStation; hệ thống sinh OTP 6 số, chỉ lưu hash, gửi OTP cho Customer qua thông báo trong app.
- Business rules: BR-15, BR-16.

#### 3.10 Xác nhận khách lấy hàng (UC-21, UC-10)

- Kích hoạt: khách tới trạm, đọc OTP cho Station Operator.
- Xử lý:
  1. Operator nhập OTP khách đọc.
  2. Hệ thống so hash, kiểm tra hạn và số lần sai. (Ảnh bằng chứng giao hàng: chưa làm, OTP là bằng chứng.)
  3. Đúng: đơn sang Completed, lưu người xác nhận và thời điểm. Sai: tăng số lần sai, hiện MSG-08.
- Business rules: BR-16, BR-17.

#### 3.11 Xử lý giao thất bại (UC-16)

- Kích hoạt: chuyến bị hủy giữa chừng hoặc khách không tới lấy hàng khi OTP hết hạn.
- Xử lý: Dispatcher chọn "Giao thất bại", bắt buộc nhập lý do; đơn sang Failed. Sau đó chọn lên lịch lại (về Scheduled) hoặc hủy hoàn trả (Cancelled).
- Business rules: BR-18.

#### 3.12 Quản lý trạm (UC-18, UC-19, UC-22)

- Kích hoạt: mở màn "Quản lý trạm" (Manager, Admin) hoặc "Trạm của tôi" (Station Operator).
- Xử lý: danh sách trạm với tọa độ, sức chứa, số đơn đang tới + kiện chờ nhận (`GET /stations/load`), trạng thái; Manager/Admin thêm, sửa, xóa qua `/api/v1/stations`; trang Trạm của Station Operator hiện tải các trạm. Gán Operator vào từng trạm: chưa làm (bảng `station_operators` có sẵn).
- Business rules: BR-19, BR-20.

#### 3.13 Dashboard và báo cáo (UC-25, UC-26, UC-27)

- Xử lý: hiển thị số đơn theo trạng thái, theo ngày; bảng hiệu quả theo trạm (`GET /reports/stations`); tỷ lệ giao thành công; thời gian giao trung bình; lọc theo khoảng ngày; xuất CSV. Nút "Phân tích bằng AI" gửi số liệu tổng hợp cho LLM và hiển thị 3–5 nhận xét kèm việc nên làm.
- Business rules: BR-21.

#### 3.14 Trợ lý AI cho khách (UC-11)

- Xử lý:
  1. Khách gửi câu hỏi; lịch sử chat giữ trên máy trong phiên (lưu vào bảng `ai_chat_messages`: chưa làm).
  2. LLM được cấp 4 hàm tra cứu: 5 đơn gần nhất của khách, trạng thái 1 đơn, trạm, ETA.
  3. Trả lời dựa trên dữ liệu thật; câu hỏi ngoài phạm vi thì từ chối lịch sự; LLM lỗi thì hiện MSG-10.
- Business rules: BR-22, BR-23.

#### 3.15 Quản lý tài khoản (UC-28)

- Xử lý: Admin xem danh sách tài khoản, tạo tài khoản nội bộ, khóa/mở, gán vai trò (một người có thể nhiều vai trò). Gán Station Operator vào trạm: chưa làm.
- Business rules: BR-02, BR-24.

### 4. Yêu cầu phi chức năng

#### 4.1 Giao tiếp bên ngoài

- Giao diện người dùng: web responsive cho màn hình từ 1280 px; app cho điện thoại 5,5–6,7 inch; tiếng Việt.
- Phần cứng: camera điện thoại để chụp ảnh gói.
- Phần mềm: RESTful API JSON tại `/api/v1`, mô tả bằng OpenAPI (docs/openapi.yaml) và Swagger; SignalR (WebSocket) cho cập nhật realtime; MinIO (giao thức S3) lưu ảnh; dịch vụ LLM bên ngoài qua HTTPS.
- Truyền thông: HTTPS; ngày giờ ISO 8601 UTC; enum trả về dạng chuỗi; lỗi theo chuẩn ProblemDetails (RFC 7807).

#### 4.2 Thuộc tính chất lượng

| Thuộc tính | Yêu cầu | Cách đáp ứng | Cách kiểm chứng |
| --- | --- | --- | --- |
| Bảo mật | Xác thực an toàn, phân quyền theo vai trò | ASP.NET Identity, mật khẩu băm, JWT hết hạn sau 60 phút, `[Authorize(Roles)]`; OTP chỉ lưu hash | Postman: gọi API sai vai trò trả 403 |
| Hiệu năng | Thao tác thông dụng dưới 3 giây | Index cho cột lọc, phân trang server, truy vấn async, AsNoTracking khi chỉ đọc | Đo tay khi demo; test tải k6 chưa làm |
| Khả năng chịu tải | Nhiều người dùng đồng thời ổn định | API stateless, connection pooling, SignalR theo nhóm `staff` và `user-{id}` | Chưa đo (k6 để hướng phát triển) |
| Tích hợp | RESTful API | Đúng HTTP verb và status code, version `/api/v1`, `docs/openapi.yaml` khớp 100 % endpoint trong code (56 endpoint) | Swagger, Postman |
| Triển khai | Chạy bằng Docker | Docker Compose gồm db, minio, api, web | `docker compose up` chạy trên máy sạch |
| Giám sát | Theo dõi sức khỏe hệ thống | Endpoint `/health`, log có cấu trúc | `/health` trả 200 |
| Truy vết | Audit log mọi thao tác ghi | `AuditInterceptor` (SaveChanges interceptor) ghi bảng audit\_logs: người làm, IP, giá trị cũ, mới; bỏ cột băm | Admin xem ở trang Nhật ký; Postman kiểm không lộ mã băm |
| Sao lưu | Khôi phục được dữ liệu | `scripts/backup.sh` (`pg_dump`), `scripts/restore.sh`; MinIO dùng volume riêng | Khôi phục thử từ bản backup (chưa chạy thật) |
| Bảo trì | Kiến trúc module | 4 tầng Api / Services / Domain / Infrastructure theo cấu trúc repo mẫu Clean Architecture của GVHD, mỗi thư mục có README.md | Review cấu trúc repo |
| Khả dụng | Web và app responsive | CSS thuần cho web; Flutter layout co giãn | Test giao diện web chụp ở 1280 px và 390 px |

### 5. Phụ lục yêu cầu

#### 5.1 Business rules

| ID | Quy tắc |
| --- | --- |
| BR-01 | Email là duy nhất; mật khẩu tối thiểu 8 ký tự; mật khẩu chỉ lưu dạng băm. |
| BR-02 | Vai trò thuộc 5 giá trị: Customer, Dispatcher, StationOperator, Manager, Admin; một tài khoản nội bộ có thể giữ nhiều vai trò. Tài khoản bị khóa không đăng nhập được (403). Token hết hạn sau 60 phút. |
| BR-03 | Trạm gửi khác trạm nhận; cả hai phải tồn tại và không ở trạng thái Maintenance. |
| BR-04 | Khối lượng từ 0,01 đến 50 kg và không vượt tải tối đa của drone lớn nhất (đội drone demo chở tối đa 2,5–5 kg); mỗi chiều từ 0,1 đến 100 cm. |
| BR-05 | Ảnh lưu trong MinIO, CSDL chỉ lưu object key; chỉ nhận JPG/PNG/WEBP (kiểm bằng byte đầu file), tối đa 5 MB. |
| BR-06 | Đơn mới luôn ở trạng thái Pending; mã vận đơn duy nhất, dạng `SDD-` + 16 ký tự hex (20 ký tự). |
| BR-07 | Customer chỉ xem được đơn của mình; đơn người khác trả 404. |
| BR-08 | Customer chỉ hủy được đơn đang Pending. |
| BR-09 | Customer chỉ sửa được đơn đang Pending: trạm, ghi chú, địa chỉ giao, người nhận, thông tin gói. |
| BR-10 | Mọi thay đổi trạng thái đơn đi qua OrderStateMachine theo bảng chuyển ở mục IV.3; chuyển sai trả 409. Mỗi lần chuyển ghi order\_status\_history và phát sự kiện SignalR. |
| BR-11 | Từ chối đơn hoặc đánh dấu thất bại bắt buộc có lý do. |
| BR-12 | Drone được gán phải Idle, tải tối đa ≥ khối lượng gói, tầm bay ≥ khoảng cách hai trạm, pin ≥ 30 %. (Ràng buộc "đang đậu ở trạm gửi": chưa áp dụng.) |
| BR-13 | Một drone chỉ thực hiện một chuyến tại một thời điểm. |
| BR-14 | Bộ giả lập phát vị trí mỗi 3 giây (chỉnh được), % đã bay = thời gian đã bay ÷ (ETA − giờ cất cánh), nội suy tuyến tính giữa hai trạm; tới 100 % thì đứng chờ nhân viên xác nhận. |
| BR-15 | Trạm Maintenance không nhận đơn mới và không nhận drone. Không gán drone tới trạm khi (đơn đang tới + kiện chờ khách) ≥ sức chứa (capacity, số kiện). |
| BR-16 | OTP gồm 6 chữ số, chỉ lưu hash, hết hạn sau 24 giờ. |
| BR-17 | Nhập sai OTP 5 lần thì khóa xác nhận, Dispatcher xử lý tiếp. |
| BR-18 | Đơn Failed chỉ được chuyển sang Scheduled (giao lại) hoặc Cancelled (hoàn trả). |
| BR-19 | Station Operator chỉ thao tác trên trạm được gán cho mình. (**Chưa làm**: hiện Operator thao tác trên mọi trạm.) |
| BR-20 | Không xóa trạm đã có đơn (khóa ngoại chặn, API trả 409 "chuyển sang Bảo trì thay vì xóa"); không xóa drone đã có chuyến bay. |
| BR-21 | Dữ liệu lưu giờ UTC; thống kê và hiển thị theo giờ Việt Nam (UTC+7). |
| BR-22 | Trợ lý AI chỉ đọc dữ liệu của chính khách hỏi và không được thay đổi dữ liệu. |
| BR-23 | Gọi LLM có timeout (`Llm:TimeoutSeconds`); lỗi thì tóm tắt và nhận xét trả nội dung tự ghép, chatbot trả 503. |
| BR-24 | Admin không tự khóa hoặc tự gỡ quyền Admin của chính mình (409). |
| BR-25 | Mọi thao tác tạo, sửa, xóa dữ liệu qua API được ghi audit log (trừ telemetry, thông báo, lịch sử trạng thái vốn đã là nhật ký). |
| BR-26 | Hệ thống ngoài xác thực bằng API key; key chỉ hiện một lần khi tạo, CSDL lưu hash. (**Chưa làm**.) |
| BR-27 | Đơn phải có người nhận: khách nhập tên, số điện thoại người nhận hoặc bỏ trống để dùng thông tin của chính mình. |

#### 5.2 Yêu cầu chung

- Danh sách có phân trang, mặc định 20 dòng/trang, bản ghi mới nhất trước.
- Thao tác xóa, hủy, từ chối có hộp thoại xác nhận.
- Trường bắt buộc có dấu \*, kiểm tra cả ở client và server.
- Thời gian hiển thị dạng dd/MM/yyyy HH:mm.

#### 5.3 Danh sách thông báo

| ID | Nội dung | Khi nào |
| --- | --- | --- |
| MSG-01 | Vui lòng nhập đầy đủ thông tin bắt buộc. | Thiếu trường bắt buộc |
| MSG-02 | Email hoặc mật khẩu không đúng. | Đăng nhập sai |
| MSG-03 | Tài khoản đã bị khóa. Liên hệ quản trị viên. | Tài khoản bị khóa |
| MSG-04 | Trạm gửi và trạm nhận phải khác nhau. | Vi phạm BR-03 |
| MSG-05 | Kiện hàng vượt quá tải trọng hoặc kích thước cho phép. | Vi phạm BR-04 |
| MSG-06 | Tạo đơn thành công. Mã vận đơn: {mã}. | Tạo đơn xong |
| MSG-07 | Không thể thực hiện thao tác ở trạng thái hiện tại của đơn. | API trả 409 |
| MSG-08 | Mã OTP không đúng. Còn {n} lần thử. | Sai OTP |
| MSG-09 | Phiên đăng nhập đã hết hạn, vui lòng đăng nhập lại. | API trả 401 |
| MSG-10 | Trợ lý AI đang bận, vui lòng thử lại sau. | LLM lỗi hoặc quá hạn |
| MSG-11 | Bạn không có quyền thực hiện thao tác này. | API trả 403 |
| MSG-12 | Đã lưu thay đổi. | Lưu thành công |

## IV. Mô tả thiết kế phần mềm (SDD)

Hệ thống theo mô hình client–server: web và app dùng chung một API, backend chia 4 tầng theo Clean Architecture, cấu trúc thư mục bám repo mẫu Flask-CleanArchitecture của giảng viên hướng dẫn.

### 1. Thiết kế hệ thống

#### 1.1 Kiến trúc hệ thống

![kiến trúc hệ thống · 2 client, 1 API, 3 nơi lưu/gọi dữ liệu](../diagrams/04-kien-truc.png)

*Hình: kiến trúc hệ thống · 2 client, 1 API, 3 nơi lưu/gọi dữ liệu*

Web và app không truy cập thẳng CSDL hay MinIO. Ảnh được upload qua API rồi lưu vào MinIO, CSDL chỉ giữ object key.

| Thành phần | Công nghệ | Vai trò |
| --- | --- | --- |
| Web portal | ReactJS 18, Vite, React Router, CSS thuần | Giao diện cho Dispatcher, Station Operator, Manager, Admin |
| Mobile app | Flutter, Dio, signalr\_netcore | Giao diện cho Customer |
| Web API | ASP.NET Core 8, ASP.NET Identity, JWT | Nghiệp vụ, xác thực, phân quyền, Swagger |
| Realtime | SignalR | Đẩy đổi trạng thái đơn và vị trí drone |
| Cơ sở dữ liệu | PostgreSQL 16, EF Core | Lưu toàn bộ dữ liệu nghiệp vụ |
| Lưu trữ file | MinIO | Ảnh gói hàng |
| AI | LLM API bên ngoài (chuẩn OpenAI) + công thức Haversine | Trợ lý (function calling), tóm tắt chuyến, nhận xét vận hành; ước tính ETA |

#### 1.2 Cấu trúc thư mục backend

Backend là một project ASP.NET Core 8 (`src/SmartDrone.csproj`), chia thư mục theo repo mẫu Flask-CleanArchitecture của giảng viên hướng dẫn: cùng tên tầng, cùng vị trí các file khởi động, chỉ đổi ngôn ngữ. Mỗi thư mục có `README.md` giải thích từng file làm gì, dùng thế nào; file quan trọng có thêm file `.md` cùng tên. Bảng đối chiếu từng file Flask ↔ C# và sequence diagram nằm ở `docs/clean-architecture.md`.

| Tầng | Thư mục | Nội dung | Hiện trạng (v0.11, 16/11) |
| --- | --- | --- | --- |
| Api | `src/Api`: Controllers, Schemas, Middleware, Requests, Responses, Swagger | Controller REST mỏng, schema request/response có validate, log request, Swagger | 12 controller: Auth, Stations, Drones, Packages, Orders, Notifications, Customers, CustomerAddresses, Reports, Ai, Users, AuditLogs (+ DeliveryHub SignalR) |
| Services | `src/Services` | Nghiệp vụ, chỉ phụ thuộc interface của Domain | Auth, Station, Drone, Order, Delivery, Tracking, DroneSimulator, Eta, PackageImage, Notification, Customer, CustomerAddress, Report, Ai, AiInsight, User, Audit, ProfileRules |
| Domain | `src/Domain`: Constants, Exceptions, Models | model cho 16 bảng của schema v2, interface `I*Repository`, enum trạng thái, Roles, `OrderStateMachine`, exception nghiệp vụ | Đủ |
| Infrastructure | `src/Infrastructure`: Databases, Models, Repositories, Services, Realtime | AppDbContext + seed + `AuditInterceptor`, cấu hình bảng EF Core, repository, JWT, MinIO, LLM client, `DeliveryHub` | Đủ |
| Khởi động | `src/Program.cs`, `CreateApp.cs`, `Config.cs`, `Cors.cs`, `DependencyContainer.cs`, `ErrorHandler.cs`, `AppLogging.cs`, `Migrations/` | Tương ứng `app.py`, `create_app.py`, `config.py`, `cors.py`, `dependency_container.py`, `error_handler.py`, `app_logging.py`, `migrations` của mẫu | Có |
| Test | `tests/SmartDrone.Tests` | xUnit cho state machine, luật hồ sơ, công thức theo dõi, thống kê theo trạm, luật tự ghép AI | 17 hàm test |

Phụ thuộc một chiều: Api → Services → Domain ← Infrastructure. Domain không phụ thuộc EF Core; Services không gọi DbContext mà gọi interface `I*Repository` do Infrastructure cài đặt; `DependencyContainer.cs` là nơi duy nhất nối interface với class cài đặt. Lỗi nghiệp vụ được ném dạng exception trong `Domain/Exceptions.cs`, `ErrorHandler.cs` đổi thành ProblemDetails. Web và app tổ chức theo màn hình, lớp gọi API tách riêng (`api.dart` trên app, service trên web).

### 2. Thiết kế cơ sở dữ liệu

CSDL PostgreSQL gồm 16 bảng chia 3 cụm; nguồn sự thật là EF Core migration, file docs/schema.sql dùng làm bản vẽ và dựng thử.

![ERD logic · 16 bảng, 3 cụm: người dùng, đơn hàng, trạm và đội drone](../diagrams/05-erd.png)

*Hình: ERD logic · 16 bảng, 3 cụm: người dùng, đơn hàng, trạm và đội drone*

Cột trái là cụm người dùng (nối về users), cột giữa là cụm đơn hàng, cột phải là cụm trạm và đội drone. Bản đầy đủ ký hiệu crow's foot (KAN-9), vẽ từ `docs/schema.sql`, kèm khóa chính (PK), khóa ngoại (FK), duy nhất (UK):

```mermaid
erDiagram
users ||--o{ customer_addresses : "có"
users ||--o{ delivery_orders : "đặt"
users ||--o{ notifications : "nhận"
users ||--o{ ai_chat_messages : "chat"
users ||--o{ audit_logs : "thao tác"
users ||--o{ station_operators : "trực"
landing_stations ||--o{ station_operators : "có nhân viên"
landing_stations ||--o{ station_routes : "điểm đi"
landing_stations ||--o{ station_routes : "điểm đến"
landing_stations ||--o{ drones : "đang đậu"
landing_stations ||--o{ delivery_orders : "trạm gửi"
landing_stations ||--o{ delivery_orders : "trạm nhận"
packages ||--|| delivery_orders : "thuộc"
customer_addresses |o--o{ delivery_orders : "giao tới"
delivery_orders ||--o{ order_status_history : "lịch sử"
delivery_orders ||--o{ drone_flights : "chặng bay"
delivery_orders ||--o| delivery_confirmations : "OTP"
delivery_orders |o--o{ notifications : "liên quan"
drones ||--o{ drone_flights : "thực hiện"
drone_flights ||--o{ flight_telemetry : "vị trí"
users { uuid user_id PK
  string email UK
  string full_name
  bool is_active }
landing_stations { uuid station_id PK
  string name UK
  decimal latitude
  decimal longitude
  string status }
station_routes { uuid route_id PK
  uuid from_station_id FK
  uuid to_station_id FK
  decimal distance_km }
station_operators { uuid station_id PK
  uuid user_id PK }
drones { uuid drone_id PK
  string drone_code UK
  decimal max_payload_kg
  smallint battery_pct
  string status
  uuid current_station_id FK }
packages { uuid package_id PK
  decimal weight_kg
  string image_key }
customer_addresses { uuid address_id PK
  uuid customer_id FK
  string address_line
  bool is_default }
delivery_orders { uuid order_id PK
  string tracking_code UK
  uuid customer_id FK
  uuid package_id FK
  uuid origin_station_id FK
  uuid destination_station_id FK
  string status
  timestamptz eta
  string failure_reason }
order_status_history { bigint history_id PK
  uuid order_id FK
  string from_status
  string to_status
  uuid changed_by }
drone_flights { uuid flight_id PK
  uuid order_id FK
  uuid drone_id FK
  smallint leg_index
  string flight_status }
flight_telemetry { bigint telemetry_id PK
  uuid flight_id FK
  decimal latitude
  decimal longitude
  smallint battery_pct }
delivery_confirmations { uuid confirmation_id PK
  uuid order_id FK
  string code_hash
  timestamptz expires_at
  smallint failed_attempts }
notifications { uuid notification_id PK
  uuid user_id FK
  uuid order_id FK
  string title
  bool is_read }
audit_logs { bigint audit_id PK
  uuid user_id FK
  string action
  jsonb old_values
  jsonb new_values }
ai_chat_messages { uuid message_id PK
  uuid user_id FK
  string sender
  string content }
api_keys { uuid api_key_id PK
  string name
  string key_hash
  bool is_active }
```

| Bảng | Mục đích | Ràng buộc chính |
| --- | --- | --- |
| users | Tài khoản và vai trò (trong code là AspNetUsers của ASP.NET Identity) | Email duy nhất; role thuộc 5 giá trị |
| customer\_addresses | Địa chỉ giao của khách | Mỗi khách tối đa 1 địa chỉ mặc định; tọa độ hợp lệ |
| landing\_stations | Trạm hạ cánh | Tên duy nhất; status Available, Occupied, Maintenance; capacity 1–50 (số kiện trạm giữ cùng lúc) |
| station\_routes | Tuyến bay giữa 2 trạm (chưa dùng trong code: khoảng cách tính bằng Haversine) | Cặp trạm duy nhất, hai trạm khác nhau, distance\_km > 0 |
| station\_operators | Gán Station Operator vào trạm (chưa dùng trong code) | Khóa chính kép (station\_id, user\_id) |
| drones | Danh mục drone | Mã duy nhất; status Idle, Assigned, InFlight, Charging, Maintenance; pin 0–100 |
| packages | Gói hàng | Khối lượng và kích thước > 0; ảnh lưu object key |
| delivery\_orders | Đơn giao | Mã vận đơn duy nhất; 9 trạng thái; hai trạm khác nhau; Failed phải có lý do; người nhận (recipient\_name, recipient\_phone) |
| order\_status\_history | Lịch sử đổi trạng thái, vẽ timeline | Ghi mỗi lần chuyển trạng thái |
| drone\_flights | Chuyến bay của đơn | Duy nhất theo (order\_id, leg\_index); status Scheduled, Flying, Landed, Aborted |
| flight\_telemetry | Vị trí, pin theo thời gian | Ghi mỗi 3 giây từ bộ giả lập |
| delivery\_confirmations | OTP xác nhận nhận hàng | Mỗi đơn một bản ghi; chỉ lưu hash OTP; method OTP hoặc QR |
| notifications | Thông báo cho người dùng | Index theo user, trạng thái đọc |
| audit\_logs | Nhật ký thao tác | Lưu giá trị cũ và mới dạng jsonb; ghi tự động bởi `AuditInterceptor` |
| ai\_chat\_messages | Lịch sử chat với trợ lý AI (chưa dùng: lịch sử chat giữ trên app) | sender là User hoặc Assistant |
| api\_keys | Khóa cho hệ thống ngoài (chưa dùng: UC-30, UC-31 để hướng phát triển) | Chỉ lưu hash, không có khóa ngoại |

4 bảng ghi "chưa dùng" được giữ trong schema để làm tiếp mà không phải đổi CSDL.

Khóa chính dùng uuid (trừ các bảng nhật ký dùng bigint tự tăng); mọi khóa ngoại đều có index; trường updated\_at cập nhật bằng trigger; hàm fn\_haversine\_km tính khoảng cách cho tuyến và ETA.

### 3. Thiết kế chi tiết

Phần lõi của hệ thống là vòng đời đơn: mọi màn hình và API đều đọc hoặc chuyển trạng thái thông qua một lớp OrderStateMachine duy nhất.

#### 3.1 Vòng đời đơn giao

![state machine đơn giao · 9 trạng thái](../diagrams/06-state-machine.png)

*Hình: state machine đơn giao · 9 trạng thái*

| Từ | Sự kiện | Sang | Người thực hiện | API |
| --- | --- | --- | --- | --- |
| Pending | Duyệt | Approved | Dispatcher | POST /orders/{id}/approve |
| Pending | Từ chối (có lý do) | Rejected | Dispatcher | POST /orders/{id}/reject |
| Pending | Hủy | Cancelled | Customer | POST /orders/{id}/cancel |
| Approved | Gán drone, lên lịch | Scheduled | Dispatcher | POST /orders/{id}/schedule |
| Scheduled | Bắt đầu chuyến | InTransit | Dispatcher | POST /orders/{id}/start |
| InTransit | Drone tới trạm, Operator xác nhận | ArrivedAtStation | Simulator + Station Operator | POST /orders/{id}/arrive |
| ArrivedAtStation | Nhập đúng OTP | Completed | Station Operator | POST /orders/{id}/confirm |
| InTransit | Sự cố khi bay | Failed | Dispatcher | POST /orders/{id}/fail |
| ArrivedAtStation | OTP hết hạn, khách không tới | Failed | Dispatcher | POST /orders/{id}/fail |
| Failed | Giao lại | Scheduled | Dispatcher | POST /orders/{id}/schedule |
| Failed | Hủy và hoàn hàng về trạm gửi (có lý do) | Cancelled | Dispatcher | POST /orders/{id}/return |

Mọi dòng trong bảng đã có API (dòng hoàn trả thêm ở v0.8). Cặp (trạng thái, sự kiện) nào không có trong bảng thì API trả 409.

Thiết kế lớp:

- `OrderStatus` (enum, Domain): 9 giá trị trùng CHECK trong CSDL.
- `OrderStateMachine` (Domain): bảng chuyển hợp lệ + hàm `CanTransition(from, to)`; dùng chung cho mọi service.
- `OrderService`, `DeliveryService` (Services): kiểm tra quyền, gọi state machine, cập nhật đơn qua `IOrderRepository`, ghi `OrderStatusHistory`, phát sự kiện qua `IOrderEvents`.
- `OrdersController` (Api): nhận request, trả ProblemDetails khi lỗi.
- `DeliveryHub` (Infrastructure/Realtime): SignalR hub `/hubs/delivery`, nhóm `staff` (nhân viên) và `user-{id}` (khách chủ đơn).
- `DroneSimulator` (Services, BackgroundService) + `TrackingService`: tính % đã bay, nội suy vị trí, ghi telemetry, đẩy event `DroneLocation`.

#### 3.2 Luồng giao trọn vẹn

![sequence luồng giao trọn vẹn · 5 thành phần, 16 bước](../diagrams/07-sequence.png)

*Hình: sequence luồng giao trọn vẹn · 5 thành phần, 16 bước*

Bước 10–11 lặp lại trong suốt chuyến bay. Web nội bộ đóng vai Dispatcher ở bước 4–7 và Station Operator ở bước 15.

#### 3.3 Các luồng chi tiết còn lại

Các sơ đồ dưới đây vẽ theo code thật trong repo (cấu trúc Clean Architecture), mỗi luồng ghi rõ lớp nào gọi lớp nào. Mọi bước đổi trạng thái đơn đều đi qua `OrderStateMachine`; sai bước trả 409.

**a) Sơ đồ lớp của luồng đơn hàng (4 tầng)**

Controller chỉ gọi Service; Service chỉ biết interface `I*Repository` ở Domain; Infrastructure cài đặt interface bằng EF Core trên `AppDbContext`.

```mermaid
classDiagram
direction TB
namespace Api {
  class OrdersController {
    +Create(OrderRequest)
    +Approve(id, NoteRequest)
    +Reject(id, RejectRequest)
    +Schedule(id, ScheduleRequest)
    +Start(id)
    +Arrive(id)
    +Confirm(id, ConfirmRequest)
    +Fail(id, FailRequest)
  }
}
namespace Services {
  class OrderService {
    +CreateAsync()
    +ApproveAsync()
    +RejectAsync()
    +CancelAsync()
  }
  class DeliveryService {
    +ScheduleAsync()
    +StartAsync()
    +ArriveAsync()
    +ConfirmAsync()
    +FailAsync()
  }
  class EtaService {
    +EstimateAsync()
  }
}
namespace Domain {
  class DeliveryOrder {
    +Guid Id
    +string TrackingCode
    +string Status
    +DateTime? EtaUtc
  }
  class OrderStateMachine {
    +CanTransition(from, to)
    +EnsureTransition(from, to)
  }
  class IOrderRepository {
    <<interface>>
  }
  class IDroneRepository {
    <<interface>>
  }
  class IDeliveryRepository {
    <<interface>>
  }
  class INotificationRepository {
    <<interface>>
  }
}
namespace Infrastructure {
  class OrderRepository
  class DroneRepository
  class DeliveryRepository
  class NotificationRepository
  class AppDbContext
}
OrdersController --> OrderService
OrdersController --> DeliveryService
DeliveryService --> EtaService
OrderService --> IOrderRepository
DeliveryService --> IOrderRepository
DeliveryService --> IDroneRepository
DeliveryService --> IDeliveryRepository
DeliveryService --> INotificationRepository
OrderService ..> OrderStateMachine : dùng
DeliveryService ..> OrderStateMachine : dùng
OrderRepository ..|> IOrderRepository
DroneRepository ..|> IDroneRepository
DeliveryRepository ..|> IDeliveryRepository
NotificationRepository ..|> INotificationRepository
OrderRepository --> AppDbContext
DroneRepository --> AppDbContext
DeliveryRepository --> AppDbContext
NotificationRepository --> AppDbContext
IOrderRepository ..> DeliveryOrder : trả về
```

**b) Đăng nhập**

```mermaid
sequenceDiagram
autonumber
actor U as Người dùng
participant C as AuthController
participant S as AuthService
participant R as UserRepository
participant T as JwtTokenService
participant DB as PostgreSQL
U->>C: POST /api/v1/auth/login {email, password}
C->>S: LoginAsync(email, password)
S->>R: GetByEmailAsync(email)
R->>DB: SELECT users WHERE email
DB-->>R: user
R-->>S: User
S->>R: CheckPasswordAsync(id, password)
alt sai email hoặc mật khẩu
  S-->>C: UnauthorizedException
  C-->>U: 401 "Email hoặc mật khẩu không đúng."
else tài khoản bị khóa
  S-->>C: ForbiddenException
  C-->>U: 403
else hợp lệ
  S->>R: GetRolesAsync(id)
  S->>T: Create(user, roles)
  T-->>S: JWT (sub, email, name, role), hạn 60 phút
  S-->>C: AuthSession
  C-->>U: 200 {token, expiresAtUtc, userId, fullName, roles}
end
```

Sai email hoặc sai mật khẩu đều báo chung một câu để không lộ email nào đã đăng ký; token chứa `sub`, `email`, `name`, `role`.

**c) Khách tạo đơn**

```mermaid
sequenceDiagram
autonumber
actor K as Customer (app)
participant C as OrdersController
participant S as OrderService
participant ST as StationRepository
participant AD as CustomerAddressRepository
participant DR as DroneRepository
participant OR as OrderRepository
participant DB as PostgreSQL
K->>C: POST /api/v1/orders {trạm gửi, trạm nhận, kiện hàng}
C->>C: Validate schema (cân nặng 0.01-50 kg, kích thước)
C->>S: CreateAsync(customerId, order, package)
S->>S: Trạm gửi khác trạm nhận?
S->>ST: ExistsAsync(origin), ExistsAsync(destination)
opt có địa chỉ giao
  S->>AD: BelongsToAsync(addressId, customerId)
end
S->>DR: AnyCanCarryAsync(weightKg)
alt sai một luật
  S-->>C: ValidationException
  C-->>K: 400 ProblemDetails (detail tiếng Việt)
else hợp lệ
  S->>OR: AddAsync(order Pending, package, history)
  OR->>DB: INSERT packages, delivery_orders, order_status_history (1 transaction)
  OR-->>S: ok
  S-->>C: DeliveryOrder
  C-->>K: 201 OrderResponse (trackingCode, status Pending)
end
```

Kiện hàng, đơn và dòng lịch sử đầu tiên được ghi trong một transaction; sai một luật thì không bảng nào bị ghi.

**d) Giao hàng trọn vẹn: duyệt → gán drone → bay → tới trạm → OTP**

```mermaid
sequenceDiagram
autonumber
actor D as Dispatcher (web)
actor O as Station Operator
actor K as Customer (app)
participant API as OrdersController
participant S as DeliveryService
participant SM as OrderStateMachine
participant DB as PostgreSQL
D->>API: POST /orders/{id}/approve
API->>SM: Pending -> Approved
API->>DB: lưu đơn + lịch sử
D->>API: POST /orders/{id}/schedule {droneId}
API->>S: ScheduleAsync
S->>SM: Approved -> Scheduled
S->>DB: drone Idle? chở nổi kiện? tạo drone_flights, drone Assigned, ETA
D->>API: POST /orders/{id}/start
API->>S: StartAsync
S->>SM: Scheduled -> InTransit
S->>DB: chuyến Flying, drone InFlight, thông báo khách
O->>API: POST /orders/{id}/arrive
API->>S: ArriveAsync
S->>SM: InTransit -> ArrivedAtStation
S->>DB: chuyến Landed, drone Idle tại trạm nhận, lưu hash OTP (hạn 24h), thông báo có OTP
K->>API: GET /notifications
API-->>K: "Mã nhận hàng: 123456"
K->>O: đọc mã OTP tại trạm
O->>API: POST /orders/{id}/confirm {otp}
API->>S: ConfirmAsync
alt sai OTP (tối đa 5 lần)
  S-->>O: 400 còn N lần thử
else đúng OTP
  S->>SM: ArrivedAtStation -> Completed
  S->>DB: VerifiedAt, CompletedAt, thông báo "Giao hàng thành công"
  S-->>O: 200 Completed
end
```

OTP 6 số chỉ lưu dạng SHA-256, hạn 24 giờ, sai tối đa 5 lần; nhân viên trạm không xem được mã, khách đọc mã từ thông báo trên app.

**e) Giao thất bại và giao lại**

```mermaid
sequenceDiagram
autonumber
actor D as Dispatcher
participant API as OrdersController
participant S as DeliveryService
participant SM as OrderStateMachine
participant DB as PostgreSQL
actor K as Customer
D->>API: POST /orders/{id}/fail {reason}
API->>S: FailAsync(reason)
S->>SM: InTransit/ArrivedAtStation -> Failed
S->>DB: FailureReason, chuyến đang bay -> Aborted, drone -> Idle
S->>DB: thông báo khách
DB-->>K: "Giao hàng thất bại: lý do..."
alt giao lại
  D->>API: POST /orders/{id}/schedule {droneId}
  API->>S: ScheduleAsync
  S->>SM: Failed -> Scheduled
  S->>DB: tạo chặng bay mới (leg_index + 1)
else hoàn trả
  D->>API: POST /orders/{id}/return {reason}
  API->>S: ReturnAsync, Failed -> Cancelled
end
```

**f) Quy trình nghiệp vụ giao hàng (BPMN rút gọn, KAN-8)**

```mermaid
flowchart TB
subgraph KH[Customer]
  A([Bắt đầu]) --> B[Tạo đơn: trạm gửi, trạm nhận, kiện hàng]
  B --> C{Hợp lệ?}
  C -- không --> B
  N[Nhận thông báo có OTP] --> P[Tới trạm, đọc OTP]
  X([Kết thúc: đã nhận hàng])
  Q[Hủy đơn khi còn Pending]
end
subgraph DP[Dispatcher]
  D{Duyệt đơn?}
  E[Từ chối, ghi lý do]
  F[Gán drone Idle + giờ bay]
  G[Bắt đầu chuyến]
  H{Sự cố?}
  J[Báo giao thất bại]
  L{Giao lại?}
  M([Hủy và hoàn hàng, Cancelled])
end
subgraph OP[Station Operator]
  I[Xác nhận drone tới trạm]
  R{OTP đúng?}
  S[Hoàn tất đơn]
end
subgraph HT[Hệ thống]
  K[Sinh OTP 6 số, lưu hash, gửi thông báo]
  T{Hết 5 lần hoặc hết hạn 24h?}
end
C -- có --> D
B -.-> Q
D -- không --> E
D -- có --> F --> G --> H
H -- có --> J
H -- không --> I --> K --> N
P --> R
R -- đúng --> S --> X
R -- sai --> T
T -- chưa --> P
T -- rồi --> J
J --> L
L -- có --> F
L -- không --> M
```

**g) API đã cài đặt (bản v0.11: 56 endpoint, khớp `docs/openapi.yaml`)**

| Chức năng | Endpoint | Vai trò | Trạng thái đơn |
| --- | --- | --- | --- |
| Đăng ký / đăng nhập | `POST /auth/register`, `/auth/login` | Mọi người |  |
| Hồ sơ, đổi mật khẩu | `GET/PUT /auth/me`, `POST /auth/me/password` | Đã đăng nhập |  |
| Quản lý trạm | `GET/POST/PUT/DELETE /stations`, `GET /stations/load` | Xem: đã đăng nhập; sửa: Admin, Manager; tải trạm: nhân viên vận hành |  |
| Quản lý drone | `GET/POST/PUT/DELETE /drones` | Xem: nhân viên vận hành; sửa: Admin, Manager |  |
| Khách hàng | `GET /customers`, `GET/PUT /customers/{id}` | Xem: Dispatcher, Manager, Admin; sửa: Manager, Admin |  |
| Địa chỉ giao | `/customers/me/addresses` (CRUD) | Customer |  |
| Ảnh kiện hàng | `POST /packages/images`, `GET /orders/{id}/package-image` | Customer tải lên; chủ đơn và nhân viên xem |  |
| Ước tính ETA | `GET /orders/eta` | Đã đăng nhập |  |
| Tạo / sửa / hủy đơn | `POST /orders`, `PUT /orders/{id}`, `POST /orders/{id}/cancel` | Customer | → Pending / Cancelled |
| Duyệt / từ chối | `POST /orders/{id}/approve`, `/reject` | Dispatcher, Manager, Admin | Pending → Approved / Rejected |
| Gán drone | `POST /orders/{id}/schedule` | Dispatcher, Manager, Admin | Approved, Failed → Scheduled |
| Bắt đầu chuyến | `POST /orders/{id}/start` | Dispatcher, Manager, Admin | Scheduled → InTransit |
| Tới trạm + sinh OTP | `POST /orders/{id}/arrive` | Station Operator, Dispatcher, Manager, Admin | InTransit → ArrivedAtStation |
| Xác nhận OTP | `POST /orders/{id}/confirm` | Station Operator, Dispatcher, Manager, Admin | ArrivedAtStation → Completed |
| Giao thất bại | `POST /orders/{id}/fail` | Dispatcher, Manager, Admin | InTransit, ArrivedAtStation → Failed |
| Hủy và hoàn | `POST /orders/{id}/return` | Dispatcher, Manager, Admin | Failed → Cancelled |
| Theo dõi drone | `GET /orders/{id}/tracking` + SignalR `/hubs/delivery` | Chủ đơn, nhân viên |  |
| Xem, tìm đơn, lịch sử | `GET /orders?status=&q=`, `/orders/{id}`, `/orders/{id}/history` | Khách: đơn của mình; nhân viên: mọi đơn |  |
| Thông báo | `GET /notifications`, `POST /notifications/{id}/read` | Đã đăng nhập |  |
| Thống kê + CSV | `GET /reports/orders`, `/reports/stations`, `/reports/orders.csv` (lọc `from`, `to`) | Admin, Manager |  |
| Nhận xét vận hành AI | `GET /reports/ai-insights` | Admin, Manager |  |
| Quản lý tài khoản | `GET/POST /users`, `PUT /users/{id}/roles`, `/users/{id}/active` | Admin |  |
| Nhật ký thao tác | `GET /audit-logs?entity=&page=` | Admin |  |
| Trợ lý AI | `POST /ai/chat`, `/ai/feasibility-test` | Đã đăng nhập |  |
| Tóm tắt chuyến AI | `POST /orders/{id}/ai-summary` | Chủ đơn, nhân viên |  |

Tiền tố chung `/api/v1`; hợp đồng đầy đủ ở `docs/openapi.yaml`, thử trực tiếp trên Swagger `/swagger`. Không có trong API: ảnh bằng chứng giao, API key cho hệ thống ngoài, quản lý tuyến bay, gán Station Operator vào trạm (xem mục *Tình trạng theo yêu cầu đề*).

## V. Tài liệu kiểm thử

Kiểm thử tập trung vào API và vòng đời đơn, vì đây là nơi cả web lẫn app cùng phụ thuộc. Danh sách test case đầy đủ: `KiemThu_v0.8.md` → `KiemThu_v0.11.md` (88 case), Postman `docs/postman/SmartDrone.postman_collection.json` (66 request có assert), xUnit `tests/SmartDrone.Tests` (17 hàm test), test giao diện web tự động (Playwright, API giả lập).

### 1. Phạm vi kiểm thử

- Trong phạm vi: xác thực và phân quyền; toàn bộ API trong openapi.yaml; chuyển trạng thái đơn theo bảng IV.3; SignalR; OTP; dịch vụ AI (ETA, tóm tắt, nhận xét, trợ lý); dashboard; nhật ký thao tác; triển khai Docker.
- Ngoài phạm vi: điều khiển drone thật; kiểm thử bảo mật chuyên sâu (pentest); test tải (k6); chất lượng nội dung của LLM; trình duyệt cũ và điện thoại dưới Android 8.
- Ràng buộc: môi trường chạy trên một máy bằng Docker Compose; dữ liệu dùng seed; LLM dùng gói miễn phí nên có giới hạn số lần gọi.

### 2. Chiến lược kiểm thử

| Cấp độ | Đối tượng | Công cụ | Người làm |
| --- | --- | --- | --- |
| Unit test | OrderStateMachine, luật hồ sơ, % đã bay, thống kê theo trạm, luật tự ghép AI | xUnit (CI), `flutter test` (model app) | Người viết code |
| Integration test | API theo openapi.yaml, phân quyền, CSDL thật | Postman/Newman, Swagger | Tài |
| UI test | 9 trang web: điều phối, trạm, thống kê, drone & trạm, tài khoản, khách hàng, nhật ký, hồ sơ, AI | Playwright + API giả lập | Người viết code |
| System test | Kịch bản demo trên web + app + Docker | Thủ công | Cả nhóm |
| Hiệu năng | – | k6 (không làm, ngoài phạm vi) | – |
| Acceptance | Kịch bản demo (VI.3) | Demo với giảng viên | Cả nhóm |

### 3. Kế hoạch kiểm thử

- Nhân sự: Tài phụ trách kế hoạch, test case, báo cáo; mỗi thành viên tự test phần mình trước khi mở PR.
- Môi trường: Windows 10/11 hoặc Ubuntu, Docker Desktop, .NET 8 SDK, Node 20, Flutter 3.x; Chrome bản mới; 1 điện thoại Android thật + emulator.
- Mốc: Postman collection luồng tạo đơn 01/11 (KAN-37); test API toàn luồng 22/11 (KAN-68); test hệ thống + báo cáo 26/11 (KAN-70).
- Lỗi phát hiện được tạo issue loại Bug trên Jira KAN, gắn mã test case.

### 4. Test case chính

| ID | Use case | Bước thực hiện | Kết quả mong đợi | Kết quả |
| --- | --- | --- | --- | --- |
| TC-01 | UC-01 | Đăng nhập admin@drone.local / Demo@12345 | 200, trả token, vai trò Admin | Chưa chạy |
| TC-02 | UC-01 | Đăng nhập sai mật khẩu | 401, MSG-02 | Chưa chạy |
| TC-03 | UC-22 | Customer gọi POST /stations | 403, MSG-11 | Chưa chạy |
| TC-04 | UC-05 | Customer tạo đơn hợp lệ | 201, đơn Pending, mã `SDD-` 20 ký tự, người nhận mặc định là khách, có 1 dòng lịch sử | Chưa chạy |
| TC-05 | UC-05 | Tạo đơn trạm gửi = trạm nhận | 400, MSG-04 | Chưa chạy |
| TC-06 | UC-05 | Tạo đơn gói 25 kg | 400, MSG-05 | Chưa chạy |
| TC-07 | UC-08 | Customer A xem đơn của Customer B | 404 | Chưa chạy |
| TC-08 | UC-07 | Hủy đơn Pending | 200, Cancelled | Chưa chạy |
| TC-09 | UC-07 | Hủy đơn Approved | 409, MSG-07 | Chưa chạy |
| TC-10 | UC-13 | Dispatcher duyệt đơn Pending | 200, Approved, lưu người duyệt | Chưa chạy |
| TC-11 | UC-13 | Duyệt lại đơn đã Approved | 409 | Chưa chạy |
| TC-12 | UC-13 | Từ chối không nhập lý do | 400, MSG-01 | Chưa chạy |
| TC-13 | UC-14 | Gán drone có tải tối đa nhỏ hơn gói / pin < 30 % | 400 | Chưa chạy |
| TC-14 | UC-15, UC-09 | Bắt đầu chuyến, mở chi tiết đơn trên app | InTransit; vị trí cập nhật trong ≤ 3 giây | Chưa chạy |
| TC-15 | UC-21 | Nhập đúng OTP | Completed, có bản ghi xác nhận | Chưa chạy |
| TC-16 | UC-21 | Nhập sai OTP 5 lần | Lần 5 bị khóa, MSG-08 đếm lùi | Chưa chạy |
| TC-17 | UC-11 | Ngắt kết nối LLM rồi chat | MSG-10, không lỗi 500 | Chưa chạy |
| TC-18 | – | Gọi /health | 200 Healthy | Chưa chạy |
| TC-19 | UC-29 | Admin xem nhật ký sau khi sửa trạm | Có dòng Update LandingStation, giá trị cũ/mới; không có cột băm | Chưa chạy |

Bảng trên là 19 case bắt buộc phải pass trước demo; mỗi case đều có request tương ứng trong Postman collection. "Chưa chạy" nghĩa là chưa chạy trên backend thật, xem V.5.

### 5. Báo cáo kiểm thử

Tình trạng tại bản v1.0 (16/11/2026):

| Hạng mục | Số lượng | Đã chạy | Kết quả |
| --- | --- | --- | --- |
| Test giao diện web (Playwright, API giả lập) | 20 ảnh chụp, 9 trang | Có | Đạt hết; lỗi 400/409 trên console là case cố ý |
| Đối chiếu openapi.yaml ↔ controller (script) | 56 endpoint | Có | Khớp 100 % |
| Postman collection: kiểm cú pháp script | 66 request | Có | 66/66 hợp lệ |
| Postman/Newman với backend thật | 66 request | **Chưa** | Chạy theo `docs/postman/README.md` |
| xUnit `dotnet test` | 17 hàm | **Chưa** (chạy khi CI chạy) | – |
| `flutter analyze` + `flutter test` | 5 test | **Chưa** (chạy khi CI chạy) | – |
| Test case tay (KiemThu v0.8–v0.11) | 88 | Một phần (case UI) | Ghi tiếp vào cột Kết quả |

Môi trường soạn báo cáo không có .NET SDK và Flutter SDK nên chưa build backend và app. Trước khi nộp nhóm phải chạy `dotnet build`, `dotnet test`, Newman và `flutter test`, rồi điền kết quả vào bảng trên và cột Kết quả ở V.4. Migration `AddOrderRecipient` viết tay, nên chạy lại `dotnet ef migrations add` để sinh file Designer chuẩn (`src/Migrations/README.md`).

## VI. Gói bàn giao và hướng dẫn

Hệ thống dựng được trên một máy chỉ bằng Docker Compose (`docker compose up -d --build`); tài khoản demo của 5 vai trò được seed sẵn.

### 1. Gói bàn giao

| Thành phần | Vị trí |
| --- | --- |
| Mã nguồn backend | src/ (SmartDrone.csproj: Api, Services, Domain, Infrastructure, Migrations); tests/SmartDrone.Tests |
| Mã nguồn web | web/ |
| Mã nguồn mobile | mobile/ |
| Hợp đồng API | docs/openapi.yaml, Swagger tại /swagger |
| Thiết kế CSDL | docs/schema.sql, EF Core migration |
| Triển khai | docker-compose.yml, .env.example, chay-thu.bat, scripts/backup.sh, restore.sh |
| Kiểm thử | docs/postman/ (collection), docs/report/KiemThu\_v0.8 → v0.11 |
| Tài liệu | Báo cáo này + các báo cáo cập nhật v0.7 → v0.11 (docs/report), slide; video demo: nhóm quay khi demo |
| File cài app | APK: nhóm build bằng `flutter build apk --release` (mobile/README.md) |

### 2. Hướng dẫn cài đặt

#### 2.1 Yêu cầu hệ thống

- Windows 10/11, macOS hoặc Linux; RAM tối thiểu 8 GB.
- Docker Desktop (đang chạy), .NET 8 SDK, Node.js 20 (cho web), Flutter SDK 3.x + Android Studio (cho app).
- Cổng trống: 5432 (PostgreSQL), 9000/9001 (MinIO), 5080 (API), 5173 (web).

#### 2.2 Các bước cài đặt

1. Clone repo: `git clone https://github.com/nguyenmauhuynh/CNPM-group`.
2. Chép `.env.example` thành `.env` (giữ hoặc đổi mật khẩu DB, MinIO).
3. Chạy cả hệ thống: `docker compose up -d --build` (CSDL, MinIO, API, web). Migration và dữ liệu mẫu tự chạy khi API khởi động.
   - API + Swagger: http://localhost:5080/swagger
   - Web: http://localhost:5173
   - MinIO console: http://localhost:9001
4. Muốn chạy API ngoài Docker để debug: `docker compose up -d db minio` rồi `dotnet run --project src --launch-profile http` (Windows: nhấp đúp `chay-thu.bat`); web: `cd web`, `npm install`, `npm run dev`.
5. Chạy app: `cd mobile`, `flutter pub get`, rồi `flutter run` (emulator) hoặc `flutter run --dart-define=API_URL=http://<IP-máy-tính>:5080/api/v1` (điện thoại thật cùng mạng Wi-Fi).
6. Sao lưu, khôi phục CSDL: `scripts/backup.sh` (Windows `scripts\backup.bat`) và `scripts/restore.sh <file>`.

#### 2.3 Tài khoản demo

Mật khẩu chung: `Demo@12345`.

| Vai trò | Email |
| --- | --- |
| Admin | admin@drone.local |
| Manager | manager@drone.local |
| Dispatcher | dispatcher@drone.local |
| Station Operator | stationoperator@drone.local |
| Customer | customer@drone.local |

### 3. Hướng dẫn sử dụng

Ảnh màn hình web của từng bước nằm trong các báo cáo cập nhật v0.7 → v0.11. Bấm tên mình ở góc trên (web) hoặc menu "Hồ sơ của tôi" (app) để sửa họ tên, số điện thoại, đổi mật khẩu.

#### 3.1 Customer (app)

1. Đăng ký hoặc đăng nhập.
2. Menu góc phải → "Sổ địa chỉ" thêm địa chỉ giao (không bắt buộc).
3. Bấm "Tạo đơn": chọn trạm gửi, trạm nhận, nhập người nhận (bỏ trống nếu tự nhận), thông tin gói, chụp ảnh, xem ETA, xác nhận.
4. Vào "Đơn của tôi" xem trạng thái, gõ mã để tìm đơn; đơn còn Chờ duyệt thì mở chi tiết bấm "Sửa đơn"; khi đơn đang bay, chi tiết đơn hiện sơ đồ 2 trạm và vị trí drone.
5. Khi nhận thông báo "Đã tới trạm", mang OTP tới trạm nhận và đọc cho nhân viên.
6. Đơn hoàn tất: bấm "Nhờ AI tóm tắt"; menu → "Trợ lý AI" để hỏi về đơn, trạm, thời gian giao.

#### 3.2 Dispatcher (web)

1. Mở "Điều phối", tab "Chờ duyệt"; ô tìm kiếm lọc theo mã đơn.
2. Duyệt hoặc từ chối (nhập lý do).
3. Tab "Đã duyệt": bấm "Gán drone", chọn drone trong danh sách (chỉ hiện drone rảnh, đủ tải, pin ≥ 30 %).
4. Tab "Đã gán drone": bấm "Cất cánh"; theo dõi % đã bay ở trang "Trạm".
5. Khi có sự cố: bấm "Báo lỗi", nhập lý do; ở tab "Thất bại" chọn "Giao lại" hoặc "Hủy & hoàn".
6. Trang "Khách hàng": tra thông tin và lịch sử đơn của khách.

#### 3.3 Station Operator (web)

1. Mở "Trạm": thanh trên cùng hiện tải từng trạm (đang tới + chờ nhận / sức chứa); danh sách đơn đang bay tới, đã tới.
2. Khi drone tới: bấm "Drone đã tới trạm" (hệ thống sinh OTP gửi khách).
3. Khi khách tới: nhập OTP khách đọc, bấm "Xác nhận giao".
4. Đổi trạng thái trạm sang Bảo trì: nhờ Manager/Admin ở trang "Drone & trạm".

#### 3.4 Logistics Manager (web)

1. Mở "Thống kê", chọn khoảng ngày; xem số đơn theo trạng thái, theo ngày và bảng hiệu quả theo trạm.
2. Bấm "Tải CSV" để tải báo cáo; bấm "Phân tích bằng AI" để xem nhận xét.
3. Vào "Drone & trạm" để thêm, sửa, đổi trạng thái drone và trạm (kể cả sức chứa).
4. Vào "Khách hàng" sửa thông tin khách khi cần.

#### 3.5 System Administrator (web)

1. Vào "Tài khoản" tạo tài khoản nội bộ, gán vai trò, khóa/mở tài khoản.
2. Vào "Nhật ký" tra cứu thao tác theo loại đối tượng; mỗi dòng có người làm, lúc nào, trường nào đổi từ gì sang gì.
3. Sao lưu CSDL bằng `scripts/backup.sh` trước khi demo hoặc đổi dữ liệu lớn.

## Lịch sử phiên bản

| Phiên bản | Ngày | Người cập nhật | Nội dung |
| --- | --- | --- | --- |
| v0.1 | 28/10/2026 | Tiến | Bản đầu đủ 6 phần theo khung SPMP/SRS/SDD/Test/Release; số liệu bám schema v2 và openapi.yaml |
| v0.2 | 29/10/2026 | Tiến | Thêm context diagram (III.1.1) và use case diagram chuẩn UML (III.2) theo yêu cầu giảng viên; UC-08, UC-17 chỉnh lại tác nhân cho khớp sơ đồ |
| v0.3 | 29/10/2026 | Tiến | Thêm mục lục có liên kết tới từng mục; đổi danh sách việc cần bổ sung thành Checklist tiến độ theo tuần W1–W6, khớp Jira KAN |
| v0.4 | 29/10/2026 | Tiến | Điền giảng viên hướng dẫn và email thành viên; giãn kế hoạch thành 7 tuần (kết thúc 29/11), dời mốc MVP sang 08/11 và đóng băng tính năng sang 22/11; cập nhật Checklist tiến độ theo Jira |
| v0.5 | 02/11/2026 | Tiến | Tái cấu trúc backend theo repo mẫu Clean Architecture của GVHD (Api / Services / Domain / Infrastructure), thêm README.md cho từng thư mục, cài OrderStateMachine; cập nhật IV.1.2 và hướng dẫn cài đặt VI.2 |
| v0.6 | 05/11/2026 | Tiến | Thêm ERD crow's foot (IV.2); viết IV.3.3: sơ đồ lớp, 4 sơ đồ tuần tự (đăng nhập, tạo đơn, giao hàng, giao thất bại), lưu đồ BPMN, bảng API; code luồng giao hàng (lên lịch, bay, tới trạm, OTP, thất bại), thông báo, thống kê + CSV, quản lý người dùng, Docker Compose 1 lệnh |
| v0.7 | 14/11/2026 | Tiến | Web React (điều phối, trạm, thống kê, drone & trạm), app thông báo + OTP; chi tiết ở `BaoCao_v0.7_Web_Mobile.md` |
| v0.8 | 15/11/2026 | Tiến | SignalR, ảnh kiện hàng MinIO, hủy và hoàn đơn thất bại, Postman collection; `BaoCao_v0.8.md`, `KiemThu_v0.8.md` |
| v0.9 | 15/11/2026 | Tiến | Giả lập drone + theo dõi realtime, trang quản lý tài khoản; `BaoCao_v0.9.md`, `KiemThu_v0.9.md` |
| v0.10 | 16/11/2026 | Tiến | AI tóm tắt chuyến giao, AI nhận xét vận hành; `BaoCao_v0.10.md`, `KiemThu_v0.10.md` |
| v0.11 | 16/11/2026 | Tiến | Lấp yêu cầu đề: hồ sơ + đổi mật khẩu, trang Khách hàng, người nhận, tìm đơn, sửa đơn trên app, sức chứa trạm, báo cáo theo trạm, lọc ngày, nhật ký thao tác, luật pin/tầm bay, script sao lưu, xUnit; `BaoCao_v0.11.md`, `KiemThu_v0.11.md` |
| v1.0 | 16/11/2026 | Tiến | Rà toàn bộ báo cáo cho khớp code v0.11: bỏ chỗ ghi \[Cập nhật\]/\[dự kiến\], ghi rõ phần chưa làm, cập nhật bảng API, kiểm thử, hướng dẫn cài đặt và sử dụng; thay Checklist tiến độ bằng bảng Tình trạng theo yêu cầu đề |

## Tình trạng theo yêu cầu đề

Đối chiếu 12 nhóm chức năng FR01–FR12 và 7 quy tắc nghiệp vụ của đề (`docs/SmartDroneDeliveryproject-overview/project-overview/03-phan-tich-yeu-cau-he-thong.md`) với code bản v0.11.

| Mã | Yêu cầu | Tình trạng | Ở đâu |
| --- | --- | --- | --- |
| FR01 | Đăng ký, đăng nhập, đăng xuất, cập nhật thông tin cá nhân | Đủ | `/auth/*`; web: bấm tên ở góc trên; app: Hồ sơ của tôi |
| FR02 | Vai trò, quyền truy cập | Đủ | JWT + `[Authorize(Roles)]`; trang Tài khoản |
| FR03 | Thêm, xem, cập nhật khách hàng, tra lịch sử đơn | Đủ (khách tự đăng ký; nhân viên xem, Manager/Admin sửa) | trang Khách hàng, `/customers` |
| FR04 | Địa chỉ giao nhận, thông tin người nhận | Đủ | app: Địa chỉ; tạo đơn có người nhận |
| FR05 | Tạo, xem, sửa, tìm, lọc, hủy đơn | Đủ | app + web Điều phối (tìm theo mã, lọc trạng thái) |
| FR06 | Khối lượng, kích thước, điều kiện vận chuyển | Đủ | BR-04, ảnh kiện hàng MinIO |
| FR07 | Trạm: thông tin, trạng thái, số đơn tại trạm, khả năng tiếp nhận | Đủ | Drone & trạm (sức chứa), thanh tải ở trang Trạm, BR-15 |
| FR08 | Duyệt, phân công | Đủ | Điều phối, BR-12 |
| FR09 | Cập nhật, hiển thị tiến trình | Đủ | timeline, SignalR, giả lập drone |
| FR10 | Xác nhận thành công/thất bại, lưu lịch sử | Đủ (OTP; không có ảnh bằng chứng) | trang Trạm, `order_status_history` |
| FR11 | Thống kê trạng thái, tỷ lệ thành công, thời gian trung bình, hiệu quả theo trạm | Đủ | trang Thống kê, CSV |
| FR12 | AI dự đoán và phân tích | Đủ | ETA, tóm tắt chuyến, nhận xét vận hành, chatbot |
| Quy tắc 1–7 | Khách hợp lệ, người nhận + địa chỉ, điều kiện kiện hàng, kiểm tra trước phân công, đúng quyền, lưu lịch sử, xác nhận khi hoàn tất | Đủ | BR-03 → BR-27 |

Phần mở rộng ngoài đề nhóm tự đặt trong SRS nhưng **chưa làm**, để ở hướng phát triển: API key cho hệ thống ngoài (UC-30, UC-31, BR-26), gán Station Operator vào trạm (BR-19), quản lý tuyến bay (UC-24), lưu lịch sử chat AI, ảnh bằng chứng giao hàng, test tải k6, ràng buộc drone phải đậu ở trạm gửi.

Việc còn lại trước khi nộp (cần máy có .NET 8 SDK và Flutter):

- [ ] `dotnet build` + `dotnet test tests/SmartDrone.Tests`; sinh lại migration `AddOrderRecipient` bằng `dotnet ef`.
- [ ] `docker compose up -d --build`, chạy Newman với collection Postman, điền V.5.
- [ ] `flutter analyze`, `flutter test`, build APK, chụp ảnh màn hình app.
- [ ] Chạy kịch bản demo, quay video, điền ngày bảo vệ, thêm trang bìa theo mẫu trường, xuất PDF/Word.

## Nguồn tham khảo

- [Zipline surpasses 2 million deliveries, raises more than $600M](https://www.zipline.com/newsroom/zipline-surpasses-2-million-deliveries-raises-more-than-600m-to-power-next-phase-of-growth-and-expands-operations-to-houston-and-phoenix) — Zipline, 21/01/2026
- [Walmart expands drone delivery with Wing to 150 more stores](https://www.axios.com/2026/01/11/walmart-drone-delivery-wing-los-angeles-miami) — Axios, 01/2026
- [Matternet Station drone delivery hub](https://newatlas.com/drones/matternet-station-drone-delivery-hub-medical) — New Atlas, 11/03/2020
- Đề tài "AI-powered Drone Delivery Management Platform" và mã nguồn, tài liệu trong repo CNPM-group (docs/schema.sql, docs/openapi.yaml, docs/CONVENTIONS.md)
