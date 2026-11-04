# Migrations

Lịch sử thay đổi cấu trúc CSDL do EF Core sinh ra (tương đương thư mục `migrations` của Flask-Migrate/Alembic). **Không sửa tay** các file trong này.

| File | Nội dung |
|---|---|
| `20261022053444_InitialCreate.cs` | Tạo toàn bộ bảng: 15 bảng nghiệp vụ + bảng Identity (users, roles...), khóa ngoại, index, check constraint |
| `20261022054939_UpdateUserConstraints.cs` | Chỉnh bảng `users`: email ≤ 255 + unique `ux_users_email`, thêm `is_active` |
| `*.Designer.cs` | Ảnh chụp model tại thời điểm tạo migration đó (EF dùng nội bộ) |
| `AppDbContextModelSnapshot.cs` | Ảnh chụp model hiện tại; EF so với model mới để biết lần sau cần sinh migration gì |

## Lệnh

```bash
dotnet tool install --global dotnet-ef                               # cài 1 lần
dotnet ef migrations add <TenMigration> --project src -o Migrations  # sau khi sửa Domain/Models hoặc Infrastructure/Models
dotnet ef database update --project src                              # áp vào CSDL (API ở Development tự làm khi khởi động)
dotnet ef migrations script --project src -o docs/migration.sql      # xuất SQL để nộp/đối chiếu
```

Quy tắc: mỗi PR đổi bảng phải kèm migration; hai người cùng tạo migration song song thì người merge sau xóa migration của mình, pull về rồi tạo lại.
