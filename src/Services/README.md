# Services

Nghiệp vụ của hệ thống (tương đương `src/services` của repo mẫu, ví dụ `TodoService(ITodoRepository)`). Mỗi service nhận interface repository qua constructor, kiểm tra luật, gọi repository, trả model Domain. Sai luật thì ném exception trong `Domain/Exceptions.cs`.

Service **không** biết HTTP (không có `IActionResult`, không đọc header) và **không** biết EF Core (không có `AppDbContext`).

| File | Phụ thuộc | Chức năng | Chi tiết |
|---|---|---|---|
| `AuthService.cs` | `IUserRepository`, `ITokenService` | Đăng ký (luôn role Customer), đăng nhập, tạo JWT | [AuthService.md](AuthService.md) |
| `StationService.cs` | `IStationRepository` | CRUD trạm; không tìm thấy → `NotFoundException` | bên dưới |
| `OrderService.cs` | `IOrderRepository`, `IStationRepository`, `ICustomerAddressRepository`, `IDroneRepository` | Tạo, sửa, hủy, xem, phân trang, lịch sử đơn | [OrderService.md](OrderService.md) |
| `CustomerAddressService.cs` | `ICustomerAddressRepository` | CRUD địa chỉ của khách | bên dưới |
| `EtaService.cs` | `IStationRepository`, `EtaOptions` | Ước tính ETA trước khi tạo đơn: khoảng cách Haversine giữa 2 trạm ÷ vận tốc drone + phút cộng thêm theo cân nặng (KAN-53, công thức của Tài) | bên dưới |
| `AiService.cs` | `ILlmClient`, `OrderService`, `StationService` | Hỏi LLM 1 câu; chatbot có gọi tool tra đơn/trạm/ETA | [AiService.md](AiService.md) |

## StationService.cs

- `ListAsync`: tất cả trạm, sắp theo tên.
- `GetAsync(id)`: không có → `NotFoundException("Không tìm thấy trạm.")`.
- `CreateAsync` / `UpdateAsync`: cắt khoảng trắng tên, gán tọa độ, trạng thái.
- `DeleteAsync`: xóa. Trạm đang được đơn/drone tham chiếu thì PostgreSQL chặn bằng khóa ngoại (lỗi 500) – cần xử lý thêm khi làm màn quản lý trạm.

## EtaService.cs

`EstimateAsync(origin, destination, weightKg)`:

```
phút = khoảng_cách_km / Eta:DroneSpeedKmPerHour × 60 + weightKg × Eta:WeightMinutesPerKg
```

Mặc định 40 km/h và 0.5 phút/kg (`appsettings.json`, section `Eta`). Id trạm rỗng hoặc cân nặng ngoài 0.01–50 kg → 400; không thấy trạm → 404. Khoảng cách tính bằng `GeoDistance.HaversineKm` ở `Domain/Models/EtaEstimate.cs`.

## CustomerAddressService.cs

- Mọi hàm nhận `customerId` của người đang đăng nhập; địa chỉ của khách khác coi như không tồn tại (404).
- Mỗi khách tối đa **một** địa chỉ mặc định: tạo/sửa với `isDefault = true` khi đã có địa chỉ mặc định khác → `ConflictException` (409).
