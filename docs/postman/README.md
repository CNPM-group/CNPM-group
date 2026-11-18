# docs/postman

`SmartDrone.postman_collection.json`: 66 request đi trọn luồng giao hàng (v0.9 thêm theo dõi drone và quản lý tài khoản, v0.10 thêm AI, v0.11 thêm khách hàng, hồ sơ, tìm đơn, tải trạm, nhật ký), mỗi request có test tự kiểm kết quả (KAN-37, KAN-68). Chạy một lần là biết backend thật có chạy đúng không.

## Chạy

Backend phải đang chạy (`docker compose up -d --build`) với dữ liệu demo của `DbSeeder` (5 trạm, 3 drone, mỗi role 1 tài khoản). Bộ giả lập drone (`Simulator:Enabled`) để bật như mặc định.

**Bằng Postman:** Import file → chuột phải collection → **Run collection** → Run. Phải chạy **đúng thứ tự từ trên xuống**, vì request sau dùng token, id đơn, OTP mà request trước lưu lại.

**Bằng dòng lệnh** (không cần cài Postman):

```bash
npx newman run docs/postman/SmartDrone.postman_collection.json
```

Đổi địa chỉ API: thêm `--env-var baseUrl=http://<địa-chỉ>:5080`.

## Các nhóm request

| Thư mục | Kiểm gì |
|---|---|
| 0. Kiểm tra server | `/health` trả 200 |
| 1. Đăng nhập 5 role | Lấy token khách, điều phối, nhân viên trạm, quản lý, admin, lưu vào biến `customerToken`, `dispatcherToken`, `operatorToken`, `managerToken`, `adminToken` (và `adminId`) |
| 2. Khách tạo đơn | Lấy 2 trạm đang hoạt động, tạo đơn 1,5 kg (Pending), chi tiết đơn có tên trạm + kiện hàng |
| 3. Kiểm tra quyền | Không token → 401; khách / nhân viên trạm tự duyệt → 403; khách xem báo cáo → 403; cất cánh khi chưa gán drone → 409 |
| 4. Điều phối | Duyệt → chọn drone `Idle` đủ tải → gán (có ETA) → cất cánh → khách gọi `/tracking` thấy `InTransit`, có mã drone, % đã bay 0–100 |
| 5. Trạm nhận + OTP | Nhân viên trạm thấy đơn đang bay → tới trạm → khách đọc OTP trong thông báo → nhập sai (400, báo số lần còn lại) → nhập đúng (Completed) → `/tracking` báo 100 %, drone đúng tọa độ trạm đến → lịch sử đủ 6 mốc |
| 6. Thất bại → hủy và hoàn | Đơn thứ 2: duyệt → gán → bay → báo lỗi (Failed) → hủy đơn đã hoàn tất bị chặn (409) → hủy và hoàn trả (Cancelled) |
| 7. Báo cáo | Thống kê có ≥ 1 đơn hoàn tất, tỷ lệ thành công 0–100; CSV đúng kiểu `text/csv` |
| 8. Quản lý tài khoản (v0.9) | Quản lý gọi `/users` → 403; admin thấy đủ 5 tài khoản demo → tạo tài khoản điều phối mới (email theo `$timestamp`, chạy lại không trùng) → thêm vai trò Trạm → khóa → tài khoản bị khóa đăng nhập bị từ chối (403) → admin tự khóa mình → 409 |
| 9. AI (v0.10) | Điều phối và khách gọi AI tóm tắt đơn đã hoàn tất (`text` không rỗng, `source` là `llm` hoặc `rule`; `rule` thì phải có `note`, có mã đơn và chữ "hoàn tất") → khách gọi nhận xét vận hành → 403 → quản lý gọi nhận xét: 1–5 ý không rỗng. Chạy được cả khi **không có** `LLM_API_KEY` |
| 10. Khách hàng, hồ sơ, tìm đơn, trạm, nhật ký (v0.11) | Khách xem / sửa hồ sơ (số điện thoại sai → 400), đổi mật khẩu sai mật khẩu cũ → 400 → tạo đơn gửi cho người khác, chi tiết có đúng người nhận → điều phối tìm đơn theo mã ra đúng 1 đơn → khách gọi `/customers` → 403 → điều phối tìm khách, xem chi tiết (≥ 2 đơn), sửa khách → 403 → quản lý sửa khách → 200 → tải trạm, thống kê theo trạm → xóa trạm đã có đơn → 409 → hủy đơn vừa tạo → khách xem nhật ký → 403 → admin xem nhật ký đơn hàng (có lần đổi trạng thái, có email người làm) → nhật ký OTP không chứa `CodeHash` |

OTP lấy bằng cách đọc thông báo "Hàng đã tới trạm" của khách (tìm `Mã nhận hàng: ` + 6 số), giống hệt cách khách xem trên app.

## Biến

| Biến | Giá trị mặc định | Ghi chú |
|---|---|---|
| `baseUrl` | `http://localhost:5080` | |
| `password` | `Demo@12345` | Mật khẩu demo trong `appsettings.Development.json` |
| `orderId`, `order2Id`, `droneId`, `otp`, `adminId`, `newUserId`, ... | tự set | Không cần sửa |

Upload ảnh (`POST /packages/images`) không có trong collection vì cần chọn file trên máy; kiểm bằng Swagger hoặc app mobile.

Chạy lại nhiều lần được: mỗi lần tạo đơn mới, không cần reset database.

## Lưu ý từ v0.11

- **Sức chứa trạm**: gán drone tới trạm đã đủ chỗ (đơn đang tới + kiện chờ khách ≥ sức chứa) bị 409. Chạy collection dở dang nhiều lần có thể để lại đơn `Scheduled` / `InTransit`, lần sau gán drone sẽ fail. Khi đó xóa dữ liệu làm lại: `docker compose down -v && docker compose up -d --build`.
- **Pin drone**: collection chỉ chọn drone rảnh, đủ tải và còn ≥ 30 % pin.
- Request "Khách sửa hồ sơ hợp lệ" đặt lại số điện thoại của khách demo thành `0909000111` mỗi lần chạy.

