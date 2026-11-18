# web/src

Toàn bộ code React. File nào có logic đáng kể thì có file `.md` cùng tên ở cạnh để giải thích.

| File | Tác dụng | Giải thích |
|---|---|---|
| `main.jsx` | Điểm vào: gắn `App` vào `#root`, bọc `BrowserRouter` (điều hướng) và `AuthProvider` (đăng nhập), nạp `styles.css` | – |
| `App.jsx` | Khai báo đường dẫn của 8 trang và chặn trang theo role | [App.md](App.md) |
| `api.js` | Mọi lời gọi API, lưu token, đổi lỗi server thành câu tiếng Việt | [api.md](api.md) |
| `auth.jsx` | Trạng thái đăng nhập dùng chung, role nào vào trang nào | [auth.md](auth.md) |
| `orderEvents.js` | Hook nhận cập nhật realtime qua SignalR, hub chưa chạy thì tự làm mới 10 giây/lần | [orderEvents.md](orderEvents.md) |
| `format.js` | `STATUS_LABELS` đổi mã trạng thái sang tiếng Việt (Pending → Chờ duyệt...), `formatDate` in ngày giờ kiểu `dd/MM HH:mm` | – |
| `styles.css` | Toàn bộ CSS, màu chủ đạo `#0878d1` giữ theo bản HTML cũ. Màu trạng thái nằm ở các class `.badge-<Trạng thái>` | – |
| `pages/` | 8 màn hình: Login, Dispatcher, Station, Dashboard, Fleet, Users, Customers, Audit | [pages/README.md](pages/README.md) |
| `components/` | Khối giao diện dùng lại: khung trang, hộp thoại, thông báo nổi, nhãn trạng thái, hộp thoại Hồ sơ, hộp thoại AI tóm tắt | [components/README.md](components/README.md) |

## Dữ liệu đi như thế nào

```
pages/*.jsx ──gọi──> api.js ──fetch + Bearer token──> ASP.NET API /api/v1
     ▲                                                     │
     └── orderEvents.js <──── SignalR /hubs/delivery ──────┘  (event OrderStatusUpdated)
```

Trang nào cũng theo một khuôn: hàm `load()` gọi API rồi `setState`; gọi `load()` khi mở trang, sau mỗi thao tác thành công và mỗi khi `useOrderEvents` báo có thay đổi.
