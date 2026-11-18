# scripts

Script tiện ích (tương đương `scripts/run_postgres.sh` của repo mẫu). Chạy từ đâu cũng được, script tự `cd` về thư mục gốc repo.

| File | Hệ điều hành | Tác dụng |
|---|---|---|
| `run_postgres.sh` | Linux / macOS | Tạo `.env` từ `.env.example` nếu chưa có, `docker compose up -d db`, đợi tối đa 40 giây tới khi `pg_isready` báo sẵn sàng |
| `run_postgres.ps1` | Windows PowerShell | Giống trên |
| `backup.sh` / `backup.bat` | Linux, macOS / Windows | (v0.11) Sao lưu CSDL bằng `pg_dump` ra `backups/drone_<ngày giờ>.sql`. Thư mục `backups/` đã nằm trong `.gitignore` |
| `restore.sh` | Linux / macOS / Git Bash | (v0.11) Khôi phục từ 1 file sao lưu, hỏi lại trước khi ghi đè |

```bash
./scripts/run_postgres.sh        # Linux/macOS (lần đầu: chmod +x scripts/run_postgres.sh)
.\scripts\run_postgres.ps1       # Windows
```

PostgreSQL chạy ở `localhost:5432`, database `drone`, user `drone`, mật khẩu = `DB_PASSWORD` trong `.env`. Khớp với connection string trong `src/appsettings.Development.json`.

Chạy toàn bộ (DB + build + API + test đăng nhập) trên Windows: `chay-thu.bat` ở thư mục gốc.

## Sao lưu và khôi phục (v0.11)

```bash
./scripts/backup.sh                                   # tạo backups/drone_20261116_120000.sql
./scripts/restore.sh backups/drone_20261116_120000.sql  # ghi đè CSDL hiện tại, hỏi y/N trước
```

Ảnh kiện hàng nằm trong volume `miniodata` của MinIO, không nằm trong file `.sql`. Muốn sao lưu cả ảnh thì chép volume đó (hoặc dùng `mc mirror` của MinIO).

Đã kiểm cú pháp script bằng `bash -n`; chưa chạy thử với Docker thật (máy làm bản này không có Docker).

