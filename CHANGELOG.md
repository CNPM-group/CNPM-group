# CHANGELOG

Nhật ký thay đổi của repo, mới nhất ở trên. Mỗi lần cập nhật ghi: **Thêm** (chức năng/file mới), **Sửa** (lỗi đã fix), **Thay đổi** (cái cũ làm khác đi), **Xóa** (bỏ gì, vì sao). Mã KAN-xx là task trên Jira.

Cách ghi khi mình cập nhật: thêm một mục mới lên đầu file trong cùng PR, ghi ngắn gọn từng ý, có mã KAN nếu có.

---

## v0.6 – 05/11/2026 · Luồng giao hàng chạy trọn

Nhánh `feature/delivery-flow`. So với v0.5, backend đi được hết vòng đời đơn thay vì dừng ở bước duyệt.

**Thêm**
- Giao hàng (KAN-56, KAN-57) – `DeliveryService`:
  - `POST /orders/{id}/schedule`: gán drone Idle đủ tải + giờ bay, drone → Assigned, tạo chặng bay, tính ETA. Dùng lại được cho đơn Failed (giao lại).
  - `POST /orders/{id}/start`: drone → InFlight, đơn → InTransit, báo khách.
  - `POST /orders/{id}/arrive`: đơn → ArrivedAtStation, drone hạ cánh về Idle, sinh OTP 6 số (chỉ lưu hash SHA-256, hạn 24 giờ) gửi khách qua thông báo.
  - `POST /orders/{id}/confirm`: nhập OTP, đúng → Completed; sai tối đa 5 lần.
  - `POST /orders/{id}/fail`: bắt buộc lý do, chuyến bay → Aborted, drone về Idle.
  - Mỗi bước ghi đơn, chuyến bay, OTP, lịch sử, thông báo trong **một** lần lưu (không bị lưu nửa chừng).
- Thông báo: `GET /notifications`, `POST /notifications/{id}/read`.
- Thống kê (KAN-64): `GET /reports/orders` (tổng đơn, tỷ lệ thành công, thời gian giao TB, theo trạng thái/ngày), `GET /reports/orders.csv`.
- Quản lý tài khoản (KAN-29): `/users` cho Admin – liệt kê, tạo tài khoản nhân viên, gán role, khóa/mở. Tài khoản bị khóa đăng nhập trả 403.
- Docker 1 lệnh (KAN-58): `src/Dockerfile`, `docker-compose.yml` thêm service `api` (cổng 5080) và `web` (nginx, cổng 5173).
- `docs/openapi.yaml` bổ sung các endpoint trên (27 đường dẫn).
- README gốc: bảng chức năng, phạm vi AI thực tế, tình trạng hiện tại, hướng dẫn thử luồng giao hàng trên Swagger, bảng "thư mục nào là gì, tác dụng gì".
- File `CHANGELOG.md` này.
- `src/Domain/Models/OrderStateMachine.md`: bảng luật chuyển trạng thái, sơ đồ, cách thêm trạng thái mới.
- `src/Api/Controllers/OrdersController.md`: bổ sung bảng nhanh 14 endpoint và chi tiết ETA, duyệt/từ chối, schedule, start, arrive, confirm, fail (bản cũ chỉ có tạo/sửa/hủy/xem).

**Sửa**
- Build lỗi `Duplicate 'AssemblyVersionAttribute'` trên máy còn thư mục `bin/obj` của 4 project cũ → `SmartDrone.csproj` bỏ qua `**/bin/**`, `**/obj/**`.
- README: link clone đổi sang `CNPM-group/CNPM-group` (link cũ `nguyenmauhuynh/cnpm-group` chỉ còn chạy nhờ redirect).

**Thay đổi**
- `.env.example` thêm `JWT_KEY`, `LLM_API_KEY` cho docker compose.
- `IUserRepository` thêm lấy theo id, liệt kê, đổi role, khóa/mở.
- `Domain/Constants.cs` thêm `FlightStatus` (Scheduled, Flying, Landed, Aborted).
- README mô tả AI đúng mức đang làm (ETA bằng công thức Haversine, chatbot function calling); bỏ hứa "gợi ý tuyến đường, cảnh báo trễ".

**Chưa làm (chuyển sang bản sau)**
- `POST /packages/images` (KAN-33): có trong openapi, app đã gọi nhưng API chưa có.
- SignalR hub + bộ giả lập drone (KAN-55).
- Hoàn trả Failed → Cancelled cho Dispatcher.

---

## v0.5.1 – 04/11/2026 · Duyệt đơn, drone, ETA, dọn file

Commit `50c761d`, `a34686a` (PR #6).

**Thêm**
- Duyệt / từ chối đơn (KAN-50): `POST /orders/{id}/approve`, `/reject` cho Dispatcher, Manager, Admin; đi qua `OrderStateMachine`, ghi người duyệt + lịch sử; từ chối bắt buộc lý do.
- CRUD drone (KAN-35): `/drones`; trùng mã → 409, không xóa drone đang gán/đang bay.
- ETA (KAN-53): `GET /orders/eta`, tách thành `EtaService` + `EtaOptions` (lấy từ nhánh `feature/KAN-53-eta-haversine` của Tài).

**Sửa**
- `mobile/pubspec.yaml`: `signalr_netcore` bị thụt lề sai, không nằm trong `dependencies` → app không cài được.
- SignalR trên app trỏ `localhost:5000` (API chạy 5080) → lấy theo `apiBaseUrl`.
- `appsettings.json`: section `Eta` thêm sai chỗ làm hỏng JSON → đặt lại đúng.

**Thay đổi**
- File SignalR và màn theo dõi đơn của Đức dời từ `src/smartdrone_mobile` vào `mobile/lib`.
- Gom `KPI_Definitions.md`, `analytics_queries.sql` vào `docs/analytics/`.

**Xóa**
- `cpp_demo/` (C++, không thuộc đồ án).
- `order_tracking_screen.dart` dữ liệu cứng, trùng với `order_detail_page.dart`.
- Ghi chú KAN-18 cũ đã lỗi thời, 2 file docs rỗng trùng tên, `main.dart` thừa.

---

## v0.5 – 02/11/2026 · Tái cấu trúc theo Clean Architecture của GVHD

Commit `9d038ee`, merge vào `main` qua PR #6.

**Thay đổi**
- Gộp 4 project (`SmartDrone.Api`, `.Application`, `.Domain`, `.Infrastructure`) thành 1 project `src/SmartDrone.csproj`, chia thư mục giống repo mẫu Flask-CleanArchitecture: `Api / Services / Domain / Infrastructure`, các file khởi động `Program.cs`, `CreateApp.cs`, `Config.cs`, `Cors.cs`, `DependencyContainer.cs`, `ErrorHandler.cs`, `AppLogging.cs` ở gốc `src/` (tương ứng `app.py`, `create_app.py`...).
- Service chỉ phụ thuộc interface `I*Repository`, không gọi `DbContext`.
- Lỗi nghiệp vụ ném exception trong `Domain/Exceptions.cs`, `ErrorHandler.cs` đổi thành ProblemDetails (400/401/403/404/409/5xx).
- Chatbot gọi thẳng service thay vì gọi vòng qua HTTP; route AI về `/api/v1/ai`.
- CI, `chay-thu.bat` cập nhật theo đường dẫn mới.

**Thêm**
- `README.md` cho từng thư mục, `.md` cho file quan trọng; `docs/clean-architecture.md` (bảng Flask ↔ C#, sequence diagram); `scripts/run_postgres.sh|ps1`.
- `OrderStateMachine`: bảng chuyển trạng thái hợp lệ của đơn.

**Xóa**
- File rác trong `src/` (`src/frontend`, `src/src`, `src/smartdrone_mobile`, html/js lạc chỗ); tài liệu dời vào `docs/`.

Code các bạn làm trước đó (KAN-13, 32, 34, 38) được **giữ nguyên logic**, chỉ chuyển sang đúng tầng.

---

## v0.4 trở về trước – 12/10 → 02/11/2026 · Khung dự án

Các phần cả nhóm đã làm trước khi tái cấu trúc:

| Phần | Người | Jira |
|---|---|---|
| Repo, CI, schema CSDL v2 + migration, OpenAPI v1 | Tiến, Vinh | W1–W2 |
| Enum + DTO theo schema v2 | Vinh | KAN-13 (PR #3) |
| CRUD địa chỉ giao của khách, kiểm tra chủ sở hữu | Vinh | KAN-32 (PR #4) |
| API đơn hàng: tạo, sửa, hủy, xem, kiểm tra tải trọng | Vinh | KAN-34 (PR #5) |
| Prompt + function calling cho chatbot | Tài | KAN-38 |
| Định nghĩa KPI + câu SQL thống kê | Tài | KAN-39 |
| App: đăng nhập, danh sách đơn, chi tiết, tạo đơn; SignalR service | Đức | KAN-45, 46 |
| Web (HTML/JS): đăng nhập, đăng ký, tạo đơn, danh sách đơn, quản lý địa chỉ | Huỳnh, Đức | W1–W3 |
| Tài liệu tổng quan đề tài 01–09 (`docs/SmartDroneDeliveryproject-overview/`) | Đức, Tài | — |
