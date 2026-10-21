# App mobile (Flutter), cho Customer

Đăng nhập / đăng ký, tạo đơn (chọn trạm, nhập kiện, kèm ảnh), danh sách đơn, chi tiết + timeline, hủy đơn khi còn Pending.

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
