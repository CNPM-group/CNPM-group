# docs/report

| File | Là gì | Tác dụng |
|---|---|---|
| `BaoCao_SmartDroneDelivery.md` | Báo cáo đồ án chính (v1.0, 16/11/2026, khớp code v0.11) theo khung SPMP / SRS / SDD / Test / Release: giới thiệu, kế hoạch, đặc tả yêu cầu, thiết kế, kiểm thử, triển khai | Bản nộp. Cuối file có bảng *Tình trạng theo yêu cầu đề* FR01–FR12 và việc còn lại trước khi nộp. Sơ đồ hiện bằng ảnh trong `../diagrams/` và Mermaid (GitHub tự vẽ) |
| `BaoCao_v0.7_Web_Mobile.md` | Báo cáo đợt v0.7 (14/11/2026): web nhân viên ReactJS, mobile thông báo + OTP, realtime, lỗi backend đã sửa, kiểm thử, kịch bản demo | Nộp kèm báo cáo chính, dùng làm kịch bản khi demo |
| `BaoCao_v0.8.md` | Báo cáo đợt v0.8 (15/11/2026): SignalR, MinIO, hủy và hoàn đơn, quản lý drone & trạm, trợ lý AI, sổ địa chỉ, Postman, CI | Nộp kèm báo cáo chính |
| `BaoCao_v0.9.md` | Báo cáo đợt v0.9 (15/11/2026): bộ giả lập drone, API theo dõi, tiến độ bay trên web, sơ đồ theo dõi trên app, trang Tài khoản, tài liệu cho mọi file code | Nộp kèm báo cáo chính |
| `BaoCao_v0.10.md` | Báo cáo đợt v0.10 (16/11/2026): AI tóm tắt chuyến giao (KAN-62), AI nhận xét vận hành, chạy được khi không có API key | Nộp kèm báo cáo chính, phần AI |
| `BaoCao_v0.11.md` | Báo cáo đợt v0.11 (16/11/2026): lấp yêu cầu đề (hồ sơ, khách hàng, người nhận, tìm/sửa đơn, sức chứa trạm, báo cáo theo trạm, nhật ký thao tác, luật pin/tầm bay, sao lưu, xUnit) | Nộp kèm báo cáo chính |
| `KiemThu_v0.8.md` | 38 test case chia 6 nhóm (kèm phần CI), ghi cách kiểm (Postman / test giao diện / bằng tay) và kết quả | Phần Kiểm thử của báo cáo, slide |
| `KiemThu_v0.9.md` | 18 test case bổ sung của v0.9 (TC60–TC77): tracking, bộ giả lập, trang Tài khoản | Đi cùng `KiemThu_v0.8.md` (tổng 56 case) |
| `KiemThu_v0.10.md` | 12 test case của v0.10: AI (TC80–TC89) + sửa lỗi xóa trạm/drone (TC90–TC91) | Tổng 68 case |
| `KiemThu_v0.11.md` | 20 test case của v0.11 (TC92–TC111) + danh sách xUnit | Tổng 88 case |
| `images/` | Ảnh chụp màn hình web dùng trong báo cáo v0.7 → v0.11 | – |

Bản chính để chỉnh sửa vẫn là trang Confluence của nhóm. Mỗi đợt cập nhật viết **một file báo cáo mới** (`BaoCao_v0.x.md`) nối tiếp đợt trước, không sửa đè báo cáo cũ, rồi thêm 1 dòng vào bảng trên và 1 mục vào `CHANGELOG.md` ở gốc repo. Thứ tự đọc: báo cáo chính v1.0 (đã gộp mọi thay đổi) → nếu cần chi tiết từng đợt thì v0.7 → v0.8 → v0.9 → v0.10 → v0.11.

Email thành viên không đưa vào bản trên GitHub vì repo công khai.
