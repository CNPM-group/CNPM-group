# Web

Giao diện web của nhóm (Huỳnh phụ trách; Đức đã làm bản HTML tạm).

## Chạy thử
Backend phải chạy trước (`chay-thu.bat`, API ở http://localhost:5080).

    npx serve -l 5173 web

Hoặc chạy cả hệ thống bằng `docker compose up -d --build` ở gốc repo (web được nginx phục vụ sẵn).

Mở http://localhost:5173/login.html. Cổng 5173 là cổng backend cho phép (CORS).
Nhấp đúp mở file trực tiếp thì trình duyệt sẽ chặn gọi API.

## Tài khoản demo
admin@drone.local / Demo@12345 (còn customer, dispatcher, stationoperator, manager).

## Ghi chú
- Khi Huỳnh làm React thì thay phần HTML này, giữ nguyên thư mục `web/`.

## Các file

| File | Tác dụng | Gọi API thật? |
|---|---|---|
| `login.html` | Đăng nhập, gọi `POST /api/v1/auth/login`, lỗi thì hiện `detail` từ ProblemDetails | Có |
| `register.html` | Đăng ký, gọi `POST /api/v1/auth/register`, hiện lỗi từ `errors` / `detail` | Có |
| `index.html` | Trang chủ / dashboard | Chưa |
| `create-order.html` | Form tạo đơn | Chưa (đang lưu `localStorage`), sẽ nối `POST /api/v1/orders` |
| `order-list.html` | Danh sách đơn | Chưa (đang đọc `localStorage`), sẽ nối `GET /api/v1/orders` |
| `address-management.html` | Quản lý địa chỉ giao | Chưa (đang lưu `localStorage`), sẽ nối `/api/v1/customers/me/addresses` |
| `script.js` | Dữ liệu đơn mẫu + hàm vẽ thẻ đơn dùng chung | |
| `style.css` | CSS dùng chung | |

Hợp đồng API để nối: `docs/openapi.yaml`. Lỗi luôn trả dạng ProblemDetails, đọc trường `detail` để hiện cho người dùng.
