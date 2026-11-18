# CHANGELOG

Nhật ký thay đổi của repo, mới nhất ở trên. Mỗi lần cập nhật ghi: **Thêm** (chức năng/file mới), **Sửa** (lỗi đã fix), **Thay đổi** (cái cũ làm khác đi), **Xóa** (bỏ gì, vì sao). Mã KAN-xx là task trên Jira.

Cách ghi khi mình cập nhật: thêm một mục mới lên đầu file trong cùng PR, ghi ngắn gọn từng ý, có mã KAN nếu có.

---

## v0.11 – 16/11/2026 · Lấp yêu cầu đề, nhật ký thao tác, báo cáo v1.0

Nhánh `feature/v0.11-hoan-thien` (nối tiếp v0.10). Báo cáo: `docs/report/BaoCao_v0.11.md`, test case bổ sung: `docs/report/KiemThu_v0.11.md`, báo cáo chính lên **v1.0**.

**Thêm**
- **FR01** Hồ sơ: `GET/PUT /auth/me` (họ tên, số điện thoại), `POST /auth/me/password` (mật khẩu mới ≥ 8 ký tự, khác mật khẩu cũ). Web: bấm tên ở góc trên mở hộp thoại Hồ sơ; app: menu → Hồ sơ của tôi (`profile_page.dart`).
- **FR03** Khách hàng: `GET /customers?q=` (Dispatcher, Manager, Admin), `GET /customers/{id}` (thông tin + số đơn + đơn gần đây), `PUT /customers/{id}` (Manager, Admin). Web: trang **Khách hàng** (`Customers.jsx`).
- **FR04 / quy tắc 2** Người nhận: cột `recipient_name`, `recipient_phone` trên `delivery_orders` (migration `20261116060000_AddOrderRecipient`, viết tay). Bỏ trống thì lấy tên, số của chính khách. App tạo/sửa đơn có ô người nhận; web Trạm và hộp thoại Gán drone hiện người nhận.
- **FR05** Tìm đơn theo mã: `GET /orders?q=` (tối đa 50 ký tự). Web Điều phối có ô tìm (chờ 300 ms mới gọi); app Đơn của tôi có ô tìm (400 ms). App: nút **Sửa đơn** ở đơn Chờ duyệt (`CreateOrderPage(editing:)`, giữ ảnh cũ nếu không chọn ảnh mới).
- **FR07** Sức chứa trạm: `capacity` (1–50) trong API trạm, seed = 4; `GET /stations/load` (đơn đang tới + kiện chờ nhận). Web: cột tải ở Drone & trạm, thanh tải ở trang Trạm.
- **FR11** `GET /reports/stations?from=&to=`: gửi đi, nhận về, hoàn tất, thất bại, tỷ lệ, thời gian TB theo trạm nhận. Web Thống kê: chọn khoảng ngày (áp cho mọi số liệu, CSV, nhận xét AI) + bảng **Hiệu quả theo trạm**.
- **UC-29 / BR-25** Nhật ký thao tác: `AuditInterceptor` (EF SaveChanges interceptor) tự ghi `audit_logs` mỗi lần tạo/sửa/xóa qua API (người làm, IP, giá trị cũ/mới, bỏ cột băm mật khẩu/OTP); `GET /audit-logs?entity=&page=` (Admin); web trang **Nhật ký** (`Audit.jsx`).
- **BR-12** Gán drone: pin ≥ 30 %, tầm bay ≥ khoảng cách hai trạm (400). **BR-15**: trạm nhận Bảo trì hoặc đã đầy → 409; tạo đơn tới trạm Bảo trì → 400.
- `scripts/backup.sh`, `backup.bat`, `restore.sh` (pg_dump trong container `db`), thư mục `backups/` vào `.gitignore`.
- `tests/SmartDrone.Tests` (xUnit, 17 hàm): OrderStateMachine, % đã bay, ProfileRules, luật tự ghép AI, thống kê theo trạm. CI chạy `dotnet test`.
- Postman 47 → **66 request** (thư mục 10). `openapi.yaml` đủ **56 endpoint**, đã đối chiếu bằng script với controller.
- `docs/SmartDroneDeliveryproject-overview/project-overview/07`, `08`, `09` (kiểm thử và đánh giá, kết quả đạt được, kết luận và hướng phát triển). `mobile/README.md` thêm cách build APK.

**Sửa**
- `openapi.yaml` lệch code: thiếu `PUT /orders/{id}`, CRUD địa chỉ, `/ai/feasibility-test`; giới hạn kiện ghi 20 kg (code 50 kg); mẫu mã đơn sai dạng. Đã sửa hết.
- Sửa trạm trên web trước đây gửi thiếu `capacity` → bị về mặc định; giờ gửi kèm.
- `Services/README.md` ghi sai "đặt địa chỉ mặc định thứ hai trả 409" (code tự bỏ mặc định cũ).

**Thay đổi**
- `GET /auth/me` trả hồ sơ đọc từ CSDL (`ProfileResponse`) thay vì đọc từ token.
- Gói kiện: mỗi chiều 0,1–100 cm (trước không giới hạn trên).
- Báo cáo chính `BaoCao_SmartDroneDelivery.md` lên **v1.0**: rà hết chỗ lệch code (Tailwind → CSS thuần, 2 giây → 3 giây, cookie → localStorage, ảnh bằng chứng, API key, tuyến bay, gán Operator vào trạm ghi rõ **chưa làm**), bảng API 56 endpoint, phần kiểm thử, hướng dẫn cài đặt/sử dụng theo màn hình thật; thay Checklist tiến độ bằng bảng **Tình trạng theo yêu cầu đề**.
- README: tình trạng lên v0.11, thêm việc bắt buộc trước khi nộp (build .NET, sinh lại migration, Newman, flutter test, APK).

---

## v0.10 – 16/11/2026 · AI tóm tắt chuyến giao, AI nhận xét vận hành

Nhánh `feature/v0.10-ai` (nối tiếp v0.9). Báo cáo: `docs/report/BaoCao_v0.10.md`, test case bổ sung: `docs/report/KiemThu_v0.10.md`.

**Thêm**
- **KAN-62** `POST /orders/{id}/ai-summary`: AI viết 3–4 câu tóm tắt chuyến giao từ đơn, lịch sử, chuyến bay (giờ cất/hạ cánh, sớm/trễ so với ETA, số lần giao, lý do thất bại). Khách chỉ tóm tắt đơn của mình.
- `GET /reports/ai-insights` (Manager, Admin): AI đọc số liệu 30 ngày (tỷ lệ thành công, lý do thất bại hay gặp, tuyến đông nhất, đơn chờ duyệt) và viết 3–5 nhận xét kèm việc nên làm.
- `Services/AiInsightService.cs`, `Domain/Models/AiInsight.cs` (`AiSummary`, `AiInsights`, `AiSource`), `AiSummaryResponse`, `AiInsightsResponse`.
- Không có API key hoặc LLM lỗi → server tự ghép câu theo luật từ cùng dữ liệu, trả `source: "rule"` + `note` (2 API này không trả 503 như chatbot).
- Web: nút **Tóm tắt AI** ở mọi dòng trang Điều phối + tab **Hoàn tất**; khung **Nhận xét của AI** ở trang Thống kê; component `AiSummaryDialog`, `AiSource` (nhãn "AI viết · model" / "Tự ghép từ dữ liệu"); `Dialog` thêm `cancelText`.
- Mobile: thẻ **Tóm tắt chuyến giao** ở chi tiết đơn đã kết thúc; `AiSummary`, `Api.getAiSummary`; test `AiSummary`.
- Postman 43 → **47 request** (nhóm 9 AI, chạy được cả khi không có key).
- `docs/ai/ai-summary-insights.md` (prompt + luật), `openapi.yaml` thêm nhóm **AI** (`/orders/{id}/ai-summary`, `/reports/ai-insights`, `/ai/chat`), md cho mọi file mới.

**Sửa**
- Xóa trạm đã có đơn, hoặc xóa drone đã từng bay, trước đây ra **lỗi 500** (khóa ngoại PostgreSQL chặn). Giờ `StationRepository` / `DroneRepository` bắt lỗi và trả **409** kèm câu "chuyển sang Bảo trì thay vì xóa"; trang Drone & trạm hiện đúng câu đó.
- `mobile/lib/api.dart`: chú thích "Upload ảnh kiện" nằm nhầm trên `getTracking`, trả về đúng chỗ.

**Thay đổi**
- README: bảng "Phạm vi AI thực tế" chuyển 2 dòng "Chưa làm" thành "Đã có"; mô tả tình trạng lên v0.10.

---

## v0.9 – 15/11/2026 · Giả lập drone, theo dõi realtime, quản lý tài khoản

Nhánh `feature/v0.9-tracking` (nối tiếp v0.8). Báo cáo: `docs/report/BaoCao_v0.9.md`, test case bổ sung: `docs/report/KiemThu_v0.9.md`.

**Thêm**
- Backend **bộ giả lập drone** `Services/DroneSimulator.cs` (BackgroundService): mỗi 3 giây tính vị trí mọi drone đang bay theo giờ cất cánh + ETA, lưu vào `drone_flights` (`current_latitude/longitude`) và `flight_telemetry`, gửi event SignalR **`DroneLocation`**. Cấu hình section `Simulator` (`Enabled`, `IntervalSeconds`, `TimeScale` để tua nhanh khi demo).
- Backend **`GET /orders/{id}/tracking`** (`Services/TrackingService.cs`): tọa độ drone, % đã bay, 2 trạm, ETA. Khách chỉ xem đơn của mình. Thêm vào `openapi.yaml` (kèm schema `StationPoint`).
- Domain: `TrackingSnapshot`, `IDeliveryRepository.SavePositionAsync`, `IOrderEvents.DroneMovedAsync`.
- Web: trang Trạm có **thanh tiến độ bay** ("DRONE-001 đã bay 43%") chạy theo event; trang **Tài khoản** `/users` cho Admin (tìm, tạo, đổi vai trò, khóa/mở; hàng của chính mình bị khóa nút); nút demo `admin@drone.local`.
- Mobile: thẻ **Theo dõi drone** ở chi tiết đơn đang bay (`tracking_map.dart`, vẽ bằng CustomPaint, không cần API key bản đồ); `TrackingInfo` + `Api.getTracking`; `SignalRService` nghe thêm `DroneLocation`.
- Postman: 33 → **43 request** (đăng nhập admin, tracking lúc bay và sau hoàn tất, nhóm 8 quản lý tài khoản).
- Test: `flutter test` cho `TrackingInfo`; test giao diện web thêm tiến độ bay, trang Tài khoản, Manager mở `/users` bị chặn.
- **File `.md` cho mọi file code**: thêm tài liệu cho 14 file Dart (13 file cũ còn thiếu + `tracking_map.dart` mới), `mobile/lib/README.md` (mục lục app), `mobile/test/widget_test.md`, `src/Properties/README.md`, `TrackingService.md`, `DroneSimulator.md`, `Users.md`. Kiểm bằng script: 152/152 file có tài liệu (trừ migration tự sinh).

**Sửa**
- Ở 0 % và 100 %, tọa độ drone giờ trả **đúng** tọa độ trạm (tính `double` kiểu `a + (b − a) × 1` có thể lệch chữ số cuối).
- `README.md`: chatbot có 4 tool (thiếu `list_my_orders` thêm từ v0.8).

**Thay đổi**
- Tài liệu cập nhật theo v0.9: `src/README.md` (Config, appsettings), `DependencyContainer.md`, `Controllers/README.md` (thêm cả `/tracking`, `/package-image`, `/return`), `Infrastructure/Realtime/README.md`, `Domain/Models/README.md`, `Schemas/README.md`, `Services/README.md`, các file `.md` của web và `signalr_service.md`.

---

## v0.8 – 15/11/2026 · SignalR, ảnh MinIO, hủy đơn thất bại, Postman

Nhánh `feature/v0.8-hub-minio` (nối tiếp v0.7). Báo cáo: `docs/report/BaoCao_v0.8.md`, test case: `docs/report/KiemThu_v0.8.md`.

**Thêm**
- Backend **SignalR hub** `/hubs/delivery` (KAN-55): event `OrderStatusUpdated` gửi sau khi lưu ở mọi chỗ đổi trạng thái đơn (tạo, hủy, duyệt, từ chối, gán drone, cất cánh, tới trạm, xác nhận OTP, thất bại, hủy và hoàn). Nhân viên nhận mọi đơn, khách chỉ nhận đơn của mình. Token qua query `access_token` cho đường dẫn `/hubs`. Code ở `src/Infrastructure/Realtime/`.
- Backend **upload ảnh kiện lên MinIO** (KAN-33): `POST /packages/images` (≤ 5 MB, kiểm loại ảnh bằng byte đầu), `GET /orders/{id}/package-image` (kiểm quyền theo đơn). `IFileStorage` + `MinioFileStorage`, gói NuGet `Minio` 6.x, cấu hình `Minio:*`.
- Backend **hủy và hoàn đơn thất bại** (KAN-57): `POST /orders/{id}/return`, chỉ nhận đơn `Failed`, gửi thông báo cho khách.
- Web: nút **Hủy & hoàn** cho đơn thất bại; hộp thoại gán drone hiện ảnh kiện; trang **Drone & trạm** (`/fleet`) thêm/sửa/xóa drone và trạm cho Manager, Admin (KAN-59).
- Chatbot thêm tool `list_my_orders` (5 đơn gần nhất của người hỏi): khách hỏi "đơn của tôi tới đâu rồi" không cần đưa ID đơn.
- Mobile: màn **Trợ lý AI** (`chat_page.dart`), **Sổ địa chỉ** (`addresses_page.dart`), chọn địa chỉ nhận khi tạo đơn, **ước tính thời gian bay ngay khi chọn trạm** (`GET /orders/eta`), xem ảnh kiện ở chi tiết đơn (KAN-67).
- **Postman collection** 33 request tự kiểm toàn luồng (`docs/postman/`, KAN-37, KAN-68) và bảng 38 test case (`docs/report/KiemThu_v0.8.md`).
- CI: thêm job build web (`npm run build`) và mobile (`flutter analyze`, `flutter test`).
- `openapi.yaml`: thêm `/orders/{id}/return`, `/orders/{id}/package-image`.

**Sửa**
- Đặt địa chỉ mặc định mới bị lỗi 409 nếu đã có địa chỉ mặc định → giờ tự bỏ mặc định ở địa chỉ cũ (`ICustomerAddressRepository.ClearDefaultAsync`).
- Tài liệu ghi thống kê mặc định 7 ngày, thực tế server lấy 30 ngày → sửa lại tài liệu.

**Thay đổi**
- `docker-compose.yml`: service `api` nhận cấu hình MinIO (`Minio__Endpoint=minio:9000`, tài khoản lấy từ `.env`) và chờ service `minio`.
- Web Fleet: xóa lỗi thì đóng hộp thoại và hiện thông báo đỏ.

---

## v0.7 – 14/11/2026 · Web React, mobile thông báo + OTP

Nhánh `feature/web-react-mobile-otp`. Báo cáo chi tiết: `docs/report/BaoCao_v0.7_Web_Mobile.md`.

**Thêm**
- Web nhân viên viết lại bằng **ReactJS + Vite** trong `web/` (KAN-41, 42, 60, 65):
  - Đăng nhập, mở trang theo role; tài khoản Customer bị báo dùng app mobile.
  - Màn **Điều phối**: tab lọc theo trạng thái, duyệt / từ chối (bắt buộc lý do), gán drone (chỉ hiện drone rảnh và đủ tải), cất cánh, báo lỗi, giao lại đơn thất bại.
  - Màn **Trạm**: xác nhận drone tới trạm, nhập OTP 6 số giao hàng.
  - Màn **Thống kê**: tổng đơn, tỷ lệ thành công, thời gian giao TB, số đơn theo trạng thái / ngày, tải CSV.
  - Nghe SignalR `OrderStatusUpdated`; hub chưa chạy thì tự làm mới 10 giây/lần.
  - `web/Dockerfile` + `nginx.conf`: build React rồi chạy bằng nginx.
- Mobile (KAN-47, 61):
  - Màn **Thông báo** (`notifications_page.dart`), mã OTP nhận hàng hiện chữ to; icon chuông có số chưa đọc.
  - `SignalRService` gửi token, dùng chung một kết nối, có chế độ làm mới 10 giây khi chưa có hub; danh sách và chi tiết đơn tự cập nhật, có SnackBar báo đổi trạng thái.
- File `.md` giải thích cạnh các file code mới (`web/src/**/*.md`, `mobile/lib/pages/notifications_page.md`, `mobile/lib/services/signalr_service.md`).

**Sửa**
- `GET /orders/{id}` trả bản rút gọn (chỉ id trạm, không có kiện hàng) trong khi `openapi.yaml` và app mobile cần `originStation`, `destinationStation`, `package` → mở chi tiết đơn trên app bị lỗi. Thêm `OrderDetailResponse` đúng spec (`OrderService.GetDetailAsync`, `Domain/Models/OrderDetail.cs`).
- Station Operator gọi `GET /orders` chỉ thấy "đơn của mình" (luôn rỗng) vì `Roles.Staff` thiếu role này → thêm `StationOperator` vào `Roles.Staff`.
- Mobile: tạo đơn thành công nhưng app báo lỗi vì đọc response tạo đơn như chi tiết đầy đủ → `createOrder` chỉ lấy `id`, `cancelOrder` không đọc response.
- Mobile: upload ảnh lỗi (chưa có API) làm hỏng cả lần tạo đơn → vẫn tạo đơn không kèm ảnh và báo cho khách.

**Thay đổi**
- `docker-compose.yml`: service `web` build từ `web/Dockerfile` thay vì mount thư mục HTML.

**Xóa**
- Web HTML cũ chuyển sang `web-old/` (không xóa hẳn, để tham khảo).

---

## v0.6.1 – 05/11/2026 · Báo cáo và sơ đồ vào repo

**Thêm**
- `docs/report/BaoCao_SmartDroneDelivery.md`: bản chép báo cáo v0.6 để đọc trên GitHub (bỏ email thành viên).
- `docs/diagrams/`: 14 sơ đồ PNG + mã Mermaid `.mmd`, có README ghi sơ đồ nào thuộc mục nào của báo cáo.

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
