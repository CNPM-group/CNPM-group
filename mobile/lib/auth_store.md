# auth_store.dart

`AuthStore.instance` (`ChangeNotifier`) giữ thông tin đăng nhập cho cả app.

| Hàm | Làm gì |
|---|---|
| `load()` | Đọc token từ `flutter_secure_storage` (khóa `sd_auth`); hết hạn thì xóa |
| `login(email, password)` / `register(email, password, fullName)` | Gọi API, lưu kết quả, báo `Gate` đổi màn |
| `logout()` | Ngắt SignalR, xóa token, quay về màn đăng nhập |
| `token`, `isLoggedIn`, `loaded` | Đọc nhanh trạng thái |

Lưu bằng `flutter_secure_storage` (Keystore trên Android, Keychain trên iOS) chứ không dùng `SharedPreferences`, vì token là thông tin đăng nhập, lưu dạng chữ thường thì app khác hoặc người có máy đọc được.
