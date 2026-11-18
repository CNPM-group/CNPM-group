# Kiểm thử v0.11 – test case bổ sung (lấp yêu cầu đề)

Đồ án SmartDroneDelivery · Nhóm Mậu Huỳnh · Cập nhật 16/11/2026

Nối tiếp [KiemThu_v0.8.md](KiemThu_v0.8.md) (38 case), [KiemThu_v0.9.md](KiemThu_v0.9.md) (18 case), [KiemThu_v0.10.md](KiemThu_v0.10.md) (12 case), vẫn giữ nguyên và vẫn phải chạy. File này chỉ ghi phần thêm ở v0.11, đánh số từ TC92. Tổng sau v0.11: **38 + 18 + 12 + 20 = 88 test case**, cộng 17 hàm xUnit ở mục 6.

Ký hiệu như các bản trước: **PM** = Postman thư mục 10 (backend thật), **UI** = test giao diện web tự động với API giả lập, **FT** = `flutter test` (CI), **XU** = xUnit (CI), **TC** = làm bằng tay.

## 1. Hồ sơ và mật khẩu (FR01)

| ID | Kiểm gì | Mong đợi | Cách | Kết quả |
|---|---|---|---|---|
| TC92 | Khách gọi `GET /auth/me` | 200, có `email`, `fullName`, `phone`, `roles` đọc từ CSDL | PM | Chưa chạy |
| TC93 | `PUT /auth/me` với số điện thoại `abc` | 400, câu báo số điện thoại không hợp lệ | PM | Chưa chạy |
| TC94 | `POST /auth/me/password` với mật khẩu hiện tại sai | 400 "Mật khẩu hiện tại không đúng." | PM | Chưa chạy |
| TC95 | Đổi mật khẩu đúng rồi đăng nhập lại bằng mật khẩu mới; mật khẩu cũ bị từ chối | 204; đăng nhập mới 200, cũ 401 | TC (đổi lại `Demo@12345` sau khi test) | Chưa chạy |
| TC96 | Web: bấm tên ở góc trên → sửa họ tên → Lưu | Hộp thoại đóng, thanh trên hiện tên mới | UI | Đạt (giả lập) |
| TC97 | App: menu → Hồ sơ của tôi → đổi số điện thoại | Báo đã lưu, mở lại thấy số mới | TC | Chưa chạy |

## 2. Khách hàng và người nhận (FR03, FR04)

| ID | Kiểm gì | Mong đợi | Cách | Kết quả |
|---|---|---|---|---|
| TC98 | Khách gọi `GET /customers` | 403 | PM | Chưa chạy |
| TC99 | Điều phối gọi `GET /customers?q=customer`, rồi `GET /customers/{id}` | 200; chi tiết có số đơn theo trạng thái + đơn gần đây | PM | Chưa chạy |
| TC100 | Manager `PUT /customers/{id}` đổi số điện thoại; Điều phối gọi cùng API | Manager 200; Điều phối 403 | PM | Chưa chạy |
| TC101 | Tạo đơn không nhập người nhận / nhập người nhận khác | Không nhập: người nhận = tên, số của khách. Có nhập: đúng tên, số đã nhập | PM | Chưa chạy |
| TC102 | Web: trang Khách hàng → tìm → mở chi tiết | Bảng lọc theo từ khóa; hộp thoại hiện đơn gần đây; Manager thấy nút Sửa, Điều phối không | UI | Đạt (giả lập) |

## 3. Đơn hàng (FR05)

| ID | Kiểm gì | Mong đợi | Cách | Kết quả |
|---|---|---|---|---|
| TC103 | `GET /orders?q=<8 ký tự đầu mã đơn vừa tạo>` | Kết quả có đúng đơn đó | PM | Chưa chạy |
| TC104 | Web Điều phối: gõ mã không tồn tại | "Không có đơn nào khớp ..." | UI | Đạt (giả lập) |
| TC105 | App: đơn Chờ duyệt → Sửa đơn → đổi người nhận, không chọn ảnh mới → Lưu | Đơn cập nhật, ảnh kiện giữ nguyên; đơn đã duyệt không có nút Sửa đơn | TC | Chưa chạy |

## 4. Trạm và điều phối (FR07, FR08)

| ID | Kiểm gì | Mong đợi | Cách | Kết quả |
|---|---|---|---|---|
| TC106 | `GET /stations/load` bằng Điều phối; Khách gọi cùng API | 200 mỗi trạm có `stationId`, `incoming`, `waiting`, `outgoing`; Khách 403 | PM | Chưa chạy |
| TC107 | Đặt sức chứa trạm nhận = 1 khi đã có 1 đơn đang tới, gán drone thêm 1 đơn tới trạm đó | 409 "... đã đủ sức chứa ..." | TC (trang Drone & trạm + Điều phối) | Chưa chạy |
| TC108 | Gán drone có pin 20 % (sửa pin ở Drone & trạm) bằng Swagger; web Gán drone | API 400 "... cần tối thiểu 30% ..."; web không liệt kê drone đó | TC, UI | UI: Đạt (giả lập) · API: Chưa chạy |

## 5. Báo cáo và nhật ký (FR11, UC-29)

| ID | Kiểm gì | Mong đợi | Cách | Kết quả |
|---|---|---|---|---|
| TC109 | `GET /reports/stations` + web Thống kê chọn khoảng ngày | 200 mỗi trạm có gửi/nhận/hoàn tất/thất bại; tổng "Hoàn tất" theo trạm = ô Hoàn tất ở trên (cùng khoảng ngày) | PM, UI | UI: Đạt (giả lập) · PM: Chưa chạy |
| TC110 | Manager sửa trạm; Admin gọi `GET /audit-logs?entity=LandingStation` | Có dòng `Update`, email Manager, giá trị cũ/mới đúng trường vừa sửa | PM, UI | UI: Đạt (giả lập) · PM: Chưa chạy |
| TC111 | Admin xem nhật ký sau khi tạo đơn có OTP; Manager gọi `/audit-logs` | Không có `CodeHash`, `PasswordHash` trong giá trị; Manager 403 | PM | Chưa chạy |

## 6. Unit test xUnit (`tests/SmartDrone.Tests`)

| File | Số hàm | Kiểm gì |
|---|---|---|
| `OrderStateMachineTests.cs` | 2 (17 bộ dữ liệu) | Cặp trạng thái hợp lệ được phép, cặp không có trong bảng bị chặn |
| `TrackingProgressTests.cs` | 5 | % đã bay: nửa đường = 50 %, quá ETA dừng ở 100 %, chưa cất cánh = 0, đã tới trạm = 100 %, tua nhanh ×10 |
| `ProfileRulesTests.cs` | 5 | Số điện thoại hợp lệ/sai/bỏ trống, họ tên quá ngắn bị chặn, họ tên được cắt khoảng trắng |
| `AiRuleTextTests.cs` | 4 | Câu tự ghép khi không có LLM: đơn hoàn tất có mã drone và sớm/trễ, đơn thất bại có lý do, không có đơn thì báo chưa đủ dữ liệu, tỷ lệ thấp thì cảnh báo kèm lý do |
| `StationReportTests.cs` | 1 | Thống kê tính theo trạm nhận, đếm đúng trên dữ liệu giả |

Chạy: `dotnet test tests/SmartDrone.Tests` (CI chạy tự động). Chưa chạy được ở máy soạn tài liệu vì không có .NET SDK.

## Ghi kết quả

Các case PM nằm trong thư mục **10. Khách hàng, hồ sơ, tìm đơn, trạm, nhật ký (v0.11)** của `docs/postman/SmartDrone.postman_collection.json`, chạy sau 9 thư mục cũ. Khi chạy Newman xong, điền cột Kết quả ở cả 4 file KiemThu và bảng V.5 của báo cáo chính.
