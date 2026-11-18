# tests/SmartDrone.Tests

Unit test backend bằng **xUnit** (v0.11), đúng kế hoạch ở mục II.1.2 và V.2 của báo cáo. Không cần CSDL, không cần Docker: chỉ kiểm các luật thuần.

```bash
dotnet test tests/SmartDrone.Tests
```

CI (job `build` trong `.github/workflows/ci.yml`) chạy lệnh này sau khi build backend.

| File | Kiểm gì |
|---|---|
| `SmartDrone.Tests.csproj` | Project test, tham chiếu `src/SmartDrone.csproj`. `src` có `InternalsVisibleTo` để test gọi được hàm `internal` |
| `OrderStateMachineTests.cs` | 11 bước chuyển hợp lệ trong bảng IV.3.1 được phép; 6 bước sai bị chặn và ném `ConflictException` (→ 409) |
| `TrackingProgressTests.cs` | Công thức % đã bay: nửa đường = 50 %, quá ETA dừng ở 100 %, chưa cất cánh = 0, đã tới trạm = 100, `TimeScale` tua nhanh |
| `ProfileRulesTests.cs` | Số điện thoại hợp lệ / sai, trống thành `null`; họ tên quá ngắn bị chặn, có cắt khoảng trắng |
| `AiRuleTextTests.cs` | Bản tự ghép khi không có LLM: tóm tắt có mã đơn, mã drone, "sớm 2 phút", lý do thất bại; nhận xét nói "chưa đủ dữ liệu" khi không có đơn, cảnh báo khi tỷ lệ < 90 % |
| `StationReportTests.cs` | Thống kê theo trạm (FR11) với dữ liệu giả: gửi, nhận, hoàn tất, thất bại, tỷ lệ 66,7 %, thời gian TB 15 phút tính theo trạm nhận |

Tên hàm test viết không dấu, đọc như câu tiếng Việt (`Chan_chuyen_sai` = chặn chuyển sai).

Lưu ý: project test được viết trên máy không có .NET SDK nên **chưa chạy thử**; CI sẽ là lần chạy đầu tiên.
