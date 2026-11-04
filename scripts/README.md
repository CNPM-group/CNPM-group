# scripts

Script tiện ích (tương đương `scripts/run_postgres.sh` của repo mẫu). Chạy từ đâu cũng được, script tự `cd` về thư mục gốc repo.

| File | Hệ điều hành | Tác dụng |
|---|---|---|
| `run_postgres.sh` | Linux / macOS | Tạo `.env` từ `.env.example` nếu chưa có, `docker compose up -d db`, đợi tối đa 40 giây tới khi `pg_isready` báo sẵn sàng |
| `run_postgres.ps1` | Windows PowerShell | Giống trên |

```bash
./scripts/run_postgres.sh        # Linux/macOS (lần đầu: chmod +x scripts/run_postgres.sh)
.\scripts\run_postgres.ps1       # Windows
```

PostgreSQL chạy ở `localhost:5432`, database `drone`, user `drone`, mật khẩu = `DB_PASSWORD` trong `.env`. Khớp với connection string trong `src/appsettings.Development.json`.

Chạy toàn bộ (DB + build + API + test đăng nhập) trên Windows: `chay-thu.bat` ở thư mục gốc.
