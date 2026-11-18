# Web cho nhân viên (ReactJS)

Web dành cho 4 nhóm người: **điều phối viên** (tìm, duyệt đơn, gán drone, cho cất cánh, báo lỗi, hủy và hoàn đơn thất bại, xem khách hàng), **nhân viên trạm** (xem tải trạm, xác nhận drone tới trạm, nhập OTP giao hàng), **quản lý** (thống kê theo ngày và theo trạm, tải CSV, nhận xét AI, quản lý drone, trạm, khách hàng) và **admin** (thêm trang Tài khoản, Nhật ký thao tác). Ai cũng sửa được hồ sơ, đổi mật khẩu bằng cách bấm tên mình ở góc trên. Khách hàng dùng app mobile trong thư mục `mobile/`.

Viết bằng React 18 + Vite, điều hướng bằng React Router, nhận cập nhật realtime bằng `@microsoft/signalr`. Bản HTML cũ chuyển sang `web-old/` để tham khảo.

## Chạy khi đang code

Cần Node.js 20 trở lên.

```bash
# 1. Ở gốc repo: chạy database, MinIO, API (không chạy service web để khỏi tranh cổng 5173)
docker compose up -d db minio api

# 2. Chạy web
cd web
npm install
npm run dev
```

Mở `http://localhost:5173`. API mặc định là `http://localhost:5080`, muốn đổi thì tạo file `web/.env.local` có dòng `VITE_API_URL=http://<địa-chỉ-khác>:5080`.

## Chạy cả hệ thống bằng Docker

```bash
docker compose up -d --build
```

Service `web` tự build React (xem `Dockerfile`) rồi phục vụ bằng nginx ở `http://localhost:5173`.

## Tài khoản demo

Mật khẩu chung `Demo@12345`, ở trang đăng nhập có nút điền nhanh.

| Email | Vào trang | Làm được |
|---|---|---|
| `dispatcher@drone.local` | Điều phối | Tìm, duyệt, từ chối, gán drone, cất cánh, báo lỗi, giao lại; xem trang Trạm, Khách hàng |
| `stationoperator@drone.local` | Trạm | Xác nhận tới trạm, nhập OTP |
| `manager@drone.local` | Thống kê | Mọi trang ở trên + thống kê, tải CSV, nhận xét AI, quản lý drone & trạm, sửa khách hàng |
| `admin@drone.local` | Thống kê | Như quản lý + trang Tài khoản (tạo, khóa, đổi vai trò) + Nhật ký thao tác |

Tài khoản `customer@drone.local` đăng nhập web sẽ bị báo "dùng app mobile".

## Cấu trúc thư mục

| Đường dẫn | Là gì |
|---|---|
| `index.html` | Trang HTML gốc, React gắn vào thẻ `#root` |
| `package.json` | Thư viện dùng và lệnh `dev`, `build`, `preview` |
| `vite.config.js` | Cấu hình Vite, cố định cổng 5173 (CORS của API chỉ cho phép cổng này) |
| `Dockerfile`, `nginx.conf`, `.dockerignore` | Build React ra `dist/` rồi chạy bằng nginx; mọi đường dẫn không phải file đều trả `index.html` để F5 ở `/dispatch` không bị 404 |
| `src/` | Code React, xem [src/README.md](src/README.md) |

## Luồng trong 1 đơn hàng

```
Khách tạo đơn (mobile) → Pending
  Điều phối: Duyệt → Approved → Gán drone → Scheduled → Cất cánh → InTransit
  Trạm: Drone đã tới trạm → ArrivedAtStation (server gửi OTP cho khách)
  Trạm: nhập OTP khách đọc → Completed
  Điều phối: Báo lỗi lúc đang bay/tới trạm → Failed → Giao lại (gán drone khác)
```

Mỗi nút chỉ hiện ở đúng trạng thái cho phép. Server vẫn kiểm tra lại bằng `OrderStateMachine`, sai luật thì trả 409 và web hiện câu lỗi đó.
