# App mobile (Flutter), cho Customer

Đăng nhập / đăng ký, tạo đơn (chọn trạm, nhập kiện, kèm ảnh, **người nhận**), danh sách đơn (**tìm theo mã**), chi tiết + timeline, **sửa** và hủy đơn khi còn Pending, **theo dõi drone**, **AI tóm tắt chuyến giao**, **hồ sơ + đổi mật khẩu**, **thông báo + mã OTP nhận hàng**, trạng thái đơn **tự cập nhật** (SignalR, mất kết nối thì tự làm mới 10 giây), xem ảnh kiện, **trợ lý AI**, **sổ địa chỉ**.

Thư mục này chỉ chứa phần code của mình (`lib/`, `test/`, `pubspec.yaml`). Các thư mục Android do Flutter tự sinh.

## Dựng project (làm 1 lần)

Cần Flutter SDK + Android Studio (xem `flutter doctor`). Ở **gốc repo**:

```bash
flutter create --project-name smart_drone_mobile --org vn.edu.ut mobile
```

Rồi chép **đè** `lib/`, `test/`, `pubspec.yaml`, `README.md` từ bản zip vào thư mục `mobile/` vừa tạo (Replace). Sau đó:

```bash
cd mobile
flutter pub get
```

## Sửa Android 1 lần (để gọi được backend http)

Mở `android/app/src/main/AndroidManifest.xml`:
1. Thêm dòng này ngay trên thẻ `<application`:
   `<uses-permission android:name="android.permission.INTERNET"/>`
2. Trong thẻ `<application ...`, thêm thuộc tính: `android:usesCleartextTraffic="true"`

## Chạy

Backend phải đang chạy ở cổng 5080.

```bash
# Emulator Android (10.0.2.2 là máy tính của mày)
flutter run

# Điện thoại thật (cùng wifi, backend chạy với --urls http://0.0.0.0:5080, mở port 5080 trên Firewall)
flutter run --dart-define=API_URL=http://<IP-LAN-may-tinh>:5080/api/v1
```

Tài khoản demo (mật khẩu `Demo@12345`): `customer@drone.local`. Tạo đơn chỉ Customer làm được.

## Test

```bash
flutter test
```

## Build APK để nộp

```bash
flutter build apk --release --dart-define=API_URL=http://<IP-máy-chạy-backend>:5080/api/v1
```

File ra ở `build/app/outputs/flutter-apk/app-release.apk`. Không truyền `API_URL` thì app gọi `10.0.2.2` (chỉ chạy được trên emulator).

## Các file trong `lib/`

Mỗi file có file `.md` cùng tên giải thích chi tiết, mục lục: [lib/README.md](lib/README.md).

| File | Tác dụng |
|---|---|
| `main.dart` | Điểm vào app; `Gate` kiểm tra đã đăng nhập chưa để mở trang login hoặc danh sách đơn |
| `config.dart` | `apiBaseUrl`, mặc định `http://10.0.2.2:5080/api/v1` (emulator), đổi bằng `--dart-define=API_URL=...` |
| `api.dart` | Lớp `Api` gọi backend (Dio): gắn header `Authorization: Bearer`, đổi lỗi ProblemDetails (`errors` / `detail`) thành `ApiException` có câu tiếng Việt. v0.7: thêm `getNotifications`, `markRead`; v0.8: thêm `chat`, `getAddresses`, `saveAddress`, `deleteAddress`, `estimateEta`; `createOrder` trả id đơn, `cancelOrder` không trả gì (API 2 chỗ này chỉ trả bản tóm tắt); v0.9: thêm `getTracking`; v0.10: `getAiSummary`; v0.11: `getMe`, `updateMe`, `getOrders(search:)` |
| `auth_store.dart` | `AuthStore` giữ token + thông tin user, lưu lại để mở app không phải đăng nhập lại. Đăng xuất thì ngắt SignalR |
| `models.dart` | Model JSON: `AuthData`, `Station`, `PackageInfo`, đơn hàng, trang đơn, lịch sử, `AppNotification` (có getter `otp` tách mã 6 số trong nội dung), `CustomerAddress`; v0.9: `StationPoint`, `TrackingInfo` (vị trí drone, có `moved()` áp event); v0.10: `AiSummary`; v0.11: `UserProfile`, người nhận trong `Order` |
| `format.dart` | Đổi mã trạng thái (`Pending`...) sang nhãn tiếng Việt, định dạng ngày giờ |
| `nav.dart` | `navigatorKey` toàn cục (đá về trang login khi token hết hạn) |
| `widgets.dart` | `StatusChip` hiển thị trạng thái đơn có màu |
| `tracking_map.dart` | (v0.9) Sơ đồ theo dõi drone vẽ bằng `CustomPaint`: 2 trạm, đường bay, chấm drone, thanh %. Xem [tracking_map.md](lib/tracking_map.md) |
| `pages/login_page.dart` | Đăng nhập / đăng ký |
| `pages/orders_page.dart` | Danh sách đơn của khách (`GET /orders`), bấm vào xem chi tiết, nút tạo đơn. Mở SignalR khi vào, có event thì tải lại + hiện SnackBar "Đơn ...: Đang giao". Icon chuông có số thông báo chưa đọc, menu ⋮ mở Trợ lý AI và Sổ địa chỉ |
| `pages/create_order_page.dart` | Tạo đơn: chọn trạm gửi/nhận, nhập cân nặng + kích thước, chọn ảnh kiện. Upload ảnh lỗi thì vẫn tạo đơn không kèm ảnh và báo cho khách. Có ô chọn địa chỉ nhận từ sổ địa chỉ. Chọn đủ 2 trạm + cân nặng thì hiện ngay ước tính "bay khoảng X phút · Y km" (`GET /orders/eta`) |
| `pages/profile_page.dart` | (v0.11) Hồ sơ của tôi: sửa họ tên, số điện thoại, đổi mật khẩu |
| `pages/order_detail_page.dart` | Chi tiết đơn + timeline (`/orders/{id}/history`), nút hủy khi `Pending`. Tự tải lại khi có event của đúng đơn đang xem. Đơn có ảnh kiện thì hiện ảnh (`GET /orders/{id}/package-image` kèm token). v0.9: đơn đang bay có thẻ **Theo dõi drone** (`GET /orders/{id}/tracking` + event `DroneLocation`). v0.10: đơn đã kết thúc có thẻ **Tóm tắt chuyến giao** do AI viết |
| `pages/notifications_page.dart` | Thông báo của khách, mã OTP hiện chữ to. Xem [notifications_page.md](lib/pages/notifications_page.md) |
| `pages/chat_page.dart` | Trợ lý AI (`POST /ai/chat`). Xem [chat_page.md](lib/pages/chat_page.md) |
| `pages/addresses_page.dart` | Sổ địa chỉ: thêm, sửa, xóa, đặt mặc định. Xem [addresses_page.md](lib/pages/addresses_page.md) |
| `services/signalr_service.dart` | Kết nối SignalR hub có gửi token, đẩy sự kiện ra `Stream` `updates` (đổi trạng thái) và `drone` (vị trí drone, v0.9); hub chưa chạy thì 10 giây phát 1 event rỗng. Xem [signalr_service.md](lib/services/signalr_service.md) |
| `services/signalr_config.dart` | URL hub = gốc của `apiBaseUrl` + `/hubs/delivery`, tên sự kiện `OrderStatusUpdated` và `DroneLocation` (v0.9) |

`test/widget_test.dart`: test `statusLabel` (mã trạng thái → nhãn tiếng Việt), `formatDate`, `TrackingInfo` (v0.9), `AiSummary` (v0.10). Xem [test/widget_test.md](test/widget_test.md).

## Demo phần khách hàng

Chạy cùng web nhân viên (`web/README.md`), 2 máy để cạnh nhau:

1. App: đăng nhập `customer@drone.local` / `Demo@12345`, bấm **Tạo đơn**: Trạm KTX Khu A → Trạm Thư viện, 1,5 kg, 20 × 15 × 10 cm.
2. Web: `dispatcher@drone.local` duyệt → gán DRONE-001 → cất cánh. App hiện SnackBar "Đang giao", danh sách tự đổi trạng thái (nếu chưa có hub thì chờ tối đa 10 giây). Mở chi tiết đơn: thẻ **Theo dõi drone** có chấm drone chạy dần sang trạm nhận.
3. Web: `stationoperator@drone.local` bấm **Drone đã tới trạm**. App: chuông hiện số thông báo mới, mở **Thông báo** thấy mã OTP chữ to.
4. Đọc mã cho người cầm web nhập. Đơn trên app chuyển **Hoàn tất**, timeline ở chi tiết đơn đủ các mốc.

## Lỗi đã biết

- Trợ lý AI cần backend có `LLM_API_KEY`, không có thì báo lỗi 503.
- Sổ địa chỉ chưa nhập tọa độ (chỉ tên + địa chỉ chữ).
- Theo dõi drone là sơ đồ tự vẽ (2 trạm + chấm drone), chưa có nền bản đồ thật.
- Nhập OTP làm ở web (trang Trạm), app không có màn cho nhân viên trạm.
- CI GitHub chạy `flutter analyze` + `flutter test` cho thư mục này mỗi lần mở PR (`.github/workflows/ci.yml`).
