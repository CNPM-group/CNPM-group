
# 🚁 SmartDroneDelivery
### Hệ thống quản lý giao hàng bằng Drone tích hợp AI

## 1. Giới thiệu đề tài

**SmartDroneDelivery** là nền tảng quản lý giao hàng bằng thiết bị bay không người lái (Drone), tích hợp trí tuệ nhân tạo (AI) nhằm nâng cao hiệu quả trong hoạt động giao nhận hàng hóa.

Hệ thống hỗ trợ doanh nghiệp quản lý toàn bộ quy trình giao hàng, từ khi khách hàng tạo đơn đến khi hàng hóa được giao và xác nhận thành công.

Trong bối cảnh thương mại điện tử và logistics ngày càng phát triển, việc ứng dụng Drone và AI góp phần tối ưu quy trình vận hành, giảm thời gian giao hàng trong những điều kiện phù hợp và cải thiện trải nghiệm khách hàng.

## 2. Mục tiêu của hệ thống

- Quản lý thông tin khách hàng, đơn hàng và kiện hàng.
- Theo dõi trạng thái giao hàng theo từng đơn.
- Hỗ trợ quản lý quá trình phân công và thực hiện giao hàng bằng Drone.
- Ứng dụng AI để phân tích dữ liệu, đề xuất tuyến giao hàng và dự đoán thời gian giao dự kiến.
- Cung cấp thông tin minh bạch để khách hàng theo dõi và xác nhận giao hàng.

## 3. Các chức năng chính

| Chức năng | Mô tả |
|---|---|
| Đăng nhập | Cho phép người dùng truy cập hệ thống. |
| Tạo đơn giao hàng | Nhập địa chỉ lấy hàng, địa chỉ nhận hàng và thông tin kiện hàng. |
| Quản lý danh sách đơn | Hiển thị mã đơn, thông tin giao nhận và trạng thái đơn hàng. |
| Theo dõi trạng thái đơn | Theo dõi tiến trình từ lúc tạo đơn đến khi hoàn thành. |
| Xác nhận giao hàng | Xác nhận người nhận đã nhận hàng và lưu thông tin minh chứng. |
| Hỗ trợ AI | Đề xuất tuyến đường, ước tính thời gian giao hàng và cảnh báo nguy cơ chậm trễ. |

## 4. Vai trò của AI

AI đóng vai trò hỗ trợ ra quyết định trong hoạt động giao hàng.

Dựa trên dữ liệu đơn hàng, địa điểm giao nhận, thời tiết và các điều kiện vận hành khả dụng, hệ thống có thể:

- Đề xuất tuyến đường giao hàng phù hợp.
- Dự đoán thời gian giao hàng.
- Cảnh báo các trường hợp có nguy cơ giao hàng chậm.
- Hỗ trợ phân tích dữ liệu và nâng cao hiệu quả vận hành.

**Lưu ý:** Các đề xuất của AI cần được đánh giá cùng với giới hạn tải trọng, dung lượng pin, vùng bay được phép và điều kiện an toàn. AI không thay thế các yêu cầu an toàn và quy định vận hành Drone.

## 5. Đối tượng sử dụng

### Khách hàng (Customer)
- Tạo đơn giao hàng.
- Quản lý thông tin đơn hàng.
- Theo dõi quá trình giao hàng.
- Xác nhận khi nhận được hàng.

### Nhân viên vận hành (Staff)
- Quản lý đơn hàng.
- Kiểm tra thông tin giao nhận.
- Theo dõi tiến độ giao hàng.
- Hỗ trợ xử lý các vấn đề phát sinh.

### Quản trị viên (Administrator)
- Quản lý tài khoản người dùng.
- Quản lý dữ liệu hệ thống.
- Theo dõi hoạt động vận hành.
- Quản lý và giám sát hệ thống.

## 6. Công nghệ sử dụng

Các thành phần công nghệ của dự án được tổ chức theo cấu trúc:

- **Backend:** ASP.NET Core.
- **Database:** PostgreSQL.
- **Frontend:** Web Application.
- **Mobile:** Mobile Application.
- **AI:** Tích hợp các dịch vụ AI hỗ trợ phân tích và đề xuất.

## 7. Kết quả mong đợi

SmartDroneDelivery hướng đến xây dựng một nền tảng quản lý giao hàng hiện đại, có khả năng:

- Hỗ trợ tự động hóa quy trình giao nhận.
- Cải thiện khả năng quản lý và theo dõi đơn hàng.
- Nâng cao hiệu quả sử dụng Drone.
- Hỗ trợ doanh nghiệp đưa ra quyết định vận hành dựa trên dữ liệu và AI.
- Cải thiện trải nghiệm của khách hàng.

## 8. Cấu trúc dự án

```text
CNPM-group/
├── .github/
│   └── workflows/
├── docs/
├── mobile/
├── src/
├── web/
├── .env.example
├── .gitignore
├── README.md
└── schema.sql
```

## 9. Repository

GitHub: https://github.com/CNPM-group/CNPM-group

---

**SmartDroneDelivery – AI-powered Drone Delivery Management Platform**

*Dự án phát triển nền tảng quản lý giao hàng bằng Drone tích hợp trí tuệ nhân tạo.*

