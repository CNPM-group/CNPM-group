# Kiểm thử v0.8 – bảng test case

Đồ án SmartDroneDelivery · Nhóm Mậu Huỳnh · Cập nhật 15/11/2026

Ba cách kiểm:

| Ký hiệu | Cách kiểm | Chạy thế nào |
|---|---|---|
| **PM** | Postman collection tự kiểm, chạy trên backend thật | `npx newman run docs/postman/SmartDrone.postman_collection.json` (xem [../postman/README.md](../postman/README.md)) |
| **UI** | Test giao diện web tự động (Playwright) với API giả lập | Đã chạy khi làm v0.7–v0.8 |
| **TC** | Làm bằng tay trên web + app | Theo kịch bản demo trong báo cáo |

Cột **Kết quả** ghi trạng thái ở thời điểm viết. Các dòng "Chưa chạy" phải chạy trên máy có Docker rồi điền vào trước buổi báo cáo.

## 1. Đăng nhập và phân quyền

| ID | Kiểm gì | Mong đợi | Cách | Kết quả |
|---|---|---|---|---|
| TC01 | Đăng nhập đúng 4 role | 200, có token | PM | Chưa chạy |
| TC02 | Gọi API không có token | 401 | PM | Chưa chạy |
| TC03 | Khách tự duyệt đơn | 403 | PM | Chưa chạy |
| TC04 | Nhân viên trạm duyệt đơn | 403 | PM | Chưa chạy |
| TC05 | Khách xem báo cáo | 403 | PM | Chưa chạy |
| TC06 | Khách đăng nhập web | Báo "dùng app mobile", không lưu token | UI | Đạt (giả lập) |
| TC07 | Nhân viên trạm gõ thẳng `/dashboard` | Bị đưa về `/station` | UI | Đạt (giả lập) |
| TC08 | Menu web theo role | Trạm: chỉ "Trạm"; Quản lý: đủ 4 trang | UI | Đạt (giả lập) |

## 2. Tạo đơn và điều phối

| ID | Kiểm gì | Mong đợi | Cách | Kết quả |
|---|---|---|---|---|
| TC10 | Khách tạo đơn 1,5 kg giữa 2 trạm | 201, trạng thái Pending | PM, TC | Chưa chạy |
| TC11 | Chi tiết đơn | Có tên 2 trạm, kiện hàng đúng 1,5 kg | PM | Chưa chạy |
| TC12 | Upload ảnh JPG/PNG ≤ 5 MB | 200, trả `imageKey` | TC (app / Swagger) | Chưa chạy |
| TC13 | Upload file không phải ảnh (đổi đuôi .jpg) | 400 "Chỉ nhận ảnh JPG, PNG hoặc WEBP" | TC (Swagger) | Chưa chạy |
| TC14 | Cất cánh khi chưa gán drone | 409 | PM | Chưa chạy |
| TC15 | Duyệt đơn | Approved | PM, UI | Đạt (giả lập) |
| TC16 | Hộp thoại gán drone chỉ hiện drone rảnh đủ tải | Kiện 2,5 kg: ẩn drone đang bay | UI | Đạt (giả lập) |
| TC17 | Gán drone | Scheduled, có ETA | PM, UI | Đạt (giả lập) |
| TC18 | Cất cánh | InTransit | PM, UI | Đạt (giả lập) |
| TC19 | Hộp thoại gán drone hiện ảnh kiện | Có ảnh khi đơn có `imageKey` | UI | Đạt (giả lập) |

## 3. Trạm nhận và OTP

| ID | Kiểm gì | Mong đợi | Cách | Kết quả |
|---|---|---|---|---|
| TC20 | Nhân viên trạm thấy đơn đang bay | Danh sách có đơn (lỗi v0.6: luôn rỗng) | PM | Chưa chạy |
| TC21 | Xác nhận tới trạm | ArrivedAtStation, khách nhận thông báo có OTP | PM, UI | Đạt (giả lập) |
| TC22 | App hiện OTP chữ to ở màn Thông báo | Thấy mã 6 số | TC | Chưa chạy |
| TC23 | Nhập sai OTP | 400, báo còn bao nhiêu lần thử | PM, UI | Đạt (giả lập) |
| TC24 | Dán OTP có dấu cách "482 913" | Ô nhập tự thành `482913` | UI | Đạt (giả lập) |
| TC25 | Nhập đúng OTP | Completed | PM, UI | Đạt (giả lập) |
| TC26 | Lịch sử đơn | Đủ 6 mốc Pending → Completed | PM | Chưa chạy |

## 4. Thất bại, hủy và hoàn (KAN-57)

| ID | Kiểm gì | Mong đợi | Cách | Kết quả |
|---|---|---|---|---|
| TC30 | Báo giao thất bại khi đang bay | Failed, drone về Idle | PM | Chưa chạy |
| TC31 | Hủy & hoàn đơn không ở Failed | 409 | PM | Chưa chạy |
| TC32 | Hủy & hoàn đơn Failed | Cancelled, khách nhận thông báo | PM, UI | Đạt (giả lập) |

## 5. Realtime (SignalR)

| ID | Kiểm gì | Mong đợi | Cách | Kết quả |
|---|---|---|---|---|
| TC40 | Web nối được hub | Nhãn "Realtime" màu xanh | TC | Chưa chạy |
| TC41 | Điều phối bấm Cất cánh | App khách tự đổi trạng thái + SnackBar, không cần kéo làm mới | TC | Chưa chạy |
| TC42 | Khách A không nhận event đơn của khách B | Không có SnackBar ở máy khách A | TC (2 tài khoản khách) | Chưa chạy |
| TC43 | Tắt API giữa chừng | Web chuyển "Tự làm mới 10s", bật lại thì về "Realtime" | TC | Chưa chạy |

## 6. Quản lý, thống kê, AI

| ID | Kiểm gì | Mong đợi | Cách | Kết quả |
|---|---|---|---|---|
| TC50 | Thống kê | Có ≥ 1 đơn hoàn tất, tỷ lệ 0–100 % | PM, UI | Đạt (giả lập) |
| TC51 | Tải CSV | File `text/csv`, mở bằng Excel không lỗi font | PM, TC | Chưa chạy |
| TC52 | Thêm drone | Hiện trong bảng, gán được cho đơn | UI | Đạt (giả lập) |
| TC53 | Xóa drone đang bay | Báo lỗi, không xóa | UI | Đạt (giả lập) |
| TC54 | Sổ địa chỉ: thêm, đặt mặc định | Địa chỉ cũ tự bỏ mặc định | TC | Chưa chạy |
| TC55 | Trợ lý AI hỏi "đơn gần nhất của tôi tới đâu rồi" | Trả lời đúng trạng thái đơn thật | TC (cần `LLM_API_KEY`) | Chưa chạy |

## 7. Build tự động (CI)

Mỗi lần push lên `main` hoặc mở PR, GitHub Actions chạy 3 job:

| Job | Làm gì |
|---|---|
| `build` | `dotnet build` backend |
| `web` | `npm install` + `npm run build` React |
| `mobile` | `flutter analyze` + `flutter test` |

3 job xanh = code của cả 3 phần biên dịch được.
