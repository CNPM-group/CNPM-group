
# PHẦN 4. THIẾT KẾ HỆ THỐNG

## 4.1. Kiến trúc tổng thể

SmartDroneDelivery được định hướng theo kiến trúc Client–Server. Web và Mobile gửi yêu cầu đến Backend API. Backend xử lý nghiệp vụ, xác thực, phân quyền và trao đổi dữ liệu với PostgreSQL.

```mermaid
flowchart TB
    U[Người dùng] --> W[ReactJS Web]
    U --> M[Flutter Mobile]
    W --> API[ASP.NET Core Web API]
    M --> API
    API --> DB[(PostgreSQL)]
    API --> RT[SignalR]
    API --> AI[Dịch vụ AI]
```

**Hình 4.1. Kiến trúc tổng thể hệ thống**

## 4.2. Thiết kế Use Case

Các tác nhân chính:
- Customer.
- Dispatcher.
- Station Operator.
- Logistics Manager.
- System Administrator.

```mermaid
flowchart LR
    C[Customer] --> A((Đăng nhập))
    C --> B((Tạo đơn hàng))
    C --> D((Theo dõi đơn))
    C --> E((Xem kết quả giao))
    P[Dispatcher] --> F((Duyệt đơn))
    P --> G((Phân công đơn))
    S[Station Operator] --> H((Cập nhật tiến trình))
    S --> I((Xác nhận giao hàng))
    L[Logistics Manager] --> J((Xem báo cáo))
    AD[Administrator] --> K((Quản lý tài khoản))
```

**Hình 4.2. Sơ đồ Use Case tổng quát**

Lưu ý: Sơ đồ trên mô tả các nhóm chức năng chính. Khi hoàn thiện báo cáo, nhóm có thể tách thành các sơ đồ Use Case chi tiết theo từng tác nhân.

## 4.3. Thiết kế Activity Diagram

### 4.3.1. Quy trình tạo và xử lý đơn hàng

```mermaid
flowchart TD
    A([Bắt đầu]) --> B[Khách hàng nhập thông tin đơn]
    B --> C[Hệ thống kiểm tra dữ liệu]
    C --> D{Dữ liệu hợp lệ?}
    D -- Không --> E[Thông báo lỗi]
    E --> B
    D -- Có --> F[Lưu đơn hàng]
    F --> G[Nhân viên điều phối kiểm tra]
    G --> H{Duyệt đơn?}
    H -- Không --> I[Thông báo từ chối]
    H -- Có --> J[Phân công giao hàng]
    J --> K([Kết thúc])
```

**Hình 4.3. Activity Diagram quy trình xử lý đơn hàng**

### 4.3.2. Quy trình giao hàng

```mermaid
flowchart TD
    A([Bắt đầu]) --> B[Tiếp nhận đơn đã phân công]
    B --> C[Thực hiện giao hàng]
    C --> D[Cập nhật trạng thái]
    D --> E{Giao thành công?}
    E -- Có --> F[Ghi nhận xác nhận giao]
    E -- Không --> G[Ghi nhận lý do thất bại]
    F --> H[Lưu lịch sử giao hàng]
    G --> H
    H --> I([Kết thúc])
```

**Hình 4.4. Activity Diagram quy trình giao hàng**

## 4.4. Thiết kế DFD

### 4.4.1. DFD mức ngữ cảnh

```mermaid
flowchart LR
    C[Khách hàng] -->|Thông tin và yêu cầu giao hàng| SYS((SmartDroneDelivery))
    SYS -->|Trạng thái và kết quả| C
    D[Nhân viên điều phối] -->|Duyệt và phân công| SYS
    SYS -->|Danh sách đơn hàng| D
    S[Nhân viên trạm] -->|Thông tin tiến trình| SYS
    SYS -->|Thông tin đơn được giao| S
    M[Quản lý] -->|Yêu cầu báo cáo| SYS
    SYS -->|Báo cáo thống kê| M
    A[Quản trị viên] -->|Cấu hình và quản lý| SYS
```

**Hình 4.5. DFD mức ngữ cảnh**

### 4.4.2. DFD mức 0

```mermaid
flowchart TB
    C[Khách hàng]
    D[Nhân viên điều phối]
    S[Nhân viên trạm]
    M[Quản lý]

    P1((1. Quản lý tài khoản))
    P2((2. Quản lý đơn hàng))
    P3((3. Quản lý kiện hàng))
    P4((4. Điều phối giao hàng))
    P5((5. Theo dõi và xác nhận))
    P6((6. Báo cáo và AI))

    DB1[(Dữ liệu người dùng)]
    DB2[(Dữ liệu đơn hàng)]
    DB3[(Dữ liệu kiện hàng)]
    DB4[(Lịch sử giao hàng)]

    C --> P1
    C --> P2
    P1 --> DB1
    P2 --> DB2
    P3 --> DB3
    D --> P2
    D --> P4
    P4 --> DB2
    S --> P5
    P5 --> DB4
    M --> P6
    DB2 --> P6
    DB4 --> P6
```

**Hình 4.6. DFD mức 0**

## 4.5. Thiết kế cơ sở dữ liệu

Các thực thể dự kiến:

| STT | Thực thể | Mục đích |
|---|---|---|
| 1 | Users | Lưu thông tin tài khoản |
| 2 | Roles | Lưu vai trò |
| 3 | Customers | Lưu thông tin khách hàng |
| 4 | Addresses | Lưu địa chỉ giao nhận |
| 5 | Orders | Lưu thông tin đơn hàng |
| 6 | Packages | Lưu thông tin kiện hàng |
| 7 | Drones | Lưu thông tin Drone |
| 8 | Stations | Lưu thông tin trạm |
| 9 | DeliveryHistories | Lưu lịch sử giao hàng |
| 10 | DeliveryConfirmations | Lưu xác nhận giao hàng |

### Sơ đồ quan hệ dữ liệu dự kiến

```mermaid
erDiagram
    USERS ||--o{ ORDERS : creates
    ROLES ||--o{ USERS : assigns
    CUSTOMERS ||--o{ ORDERS : owns
    CUSTOMERS ||--o{ ADDRESSES : has
    ORDERS ||--o{ PACKAGES : contains
    STATIONS ||--o{ DRONES : manages
    STATIONS ||--o{ ORDERS : handles
    ORDERS ||--o{ DELIVERY_HISTORIES : records
    ORDERS ||--o| DELIVERY_CONFIRMATIONS : has
    DRONES ||--o{ DELIVERY_HISTORIES : performs
```

**Hình 4.7. ERD dự kiến**

Lưu ý: Đây là mô hình khái niệm ban đầu. Tên bảng, khóa và quan hệ cần được đối chiếu với schema PostgreSQL thực tế của nhóm trước khi nghiệm thu.

## 4.6. Thiết kế API

| Nhóm API | Chức năng |
|---|---|
| Authentication | Đăng ký, đăng nhập, đăng xuất |
| User | Quản lý người dùng |
| Customer | Quản lý khách hàng |
| Order | Quản lý đơn hàng |
| Package | Quản lý kiện hàng |
| Station | Quản lý trạm |
| Delivery | Theo dõi và xác nhận giao hàng |
| Analytics | Báo cáo và thống kê |
| AI | Hỗ trợ phân tích và dự đoán |

Ví dụ một số endpoint xác thực:

| Phương thức | Endpoint | Chức năng |
|---|---|---|
| POST | `/api/v1/auth/register` | Đăng ký |
| POST | `/api/v1/auth/login` | Đăng nhập |
| GET | `/api/v1/auth/me` | Lấy thông tin người dùng |
| POST | `/api/v1/auth/logout` | Đăng xuất |

## 4.7. Thiết kế giao diện

Các màn hình dự kiến:
- Đăng nhập.
- Đăng ký.
- Dashboard.
- Quản lý đơn hàng.
- Tạo đơn hàng.
- Quản lý kiện hàng.
- Quản lý trạm.
- Theo dõi giao hàng.
- Xác nhận giao hàng.
- Báo cáo và thống kê.

### Hình ảnh giao diện Figma

Sau khi tải ảnh từ Figma lên thư mục `images`, sử dụng các đường dẫn dưới đây:

![Hình 4.8. Giao diện đăng nhập](images/figma-login.png)

![Hình 4.9. Giao diện Dashboard](images/figma-dashboard.png)

![Hình 4.10. Giao diện quản lý đơn hàng](images/figma-orders.png)

![Hình 4.11. Giao diện theo dõi giao hàng](images/figma-tracking.png)

## 4.8. Thiết kế bảo mật

- Xác thực người dùng.
- Phân quyền theo vai trò.
- Bảo vệ thông tin đăng nhập.
- Kiểm tra dữ liệu đầu vào.
- Sử dụng HTTPS khi triển khai.
- Cấu hình CORS phù hợp.
- Bảo vệ thông tin bí mật bằng biến môi trường.

## 4.9. Kết luận

Thiết kế hệ thống là cơ sở để xây dựng cơ sở dữ liệu, Backend API, giao diện Web và Mobile. Các sơ đồ cần được cập nhật theo kiến trúc và cấu trúc thực tế của nhóm.
