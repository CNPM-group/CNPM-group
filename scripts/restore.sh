#!/usr/bin/env bash
# Khôi phục CSDL từ file sao lưu. GHI ĐÈ dữ liệu hiện tại.
# Dùng: ./scripts/restore.sh backups/drone_20261116_120000.sql
set -euo pipefail
cd "$(dirname "$0")/.."
file="${1:?Truyền đường dẫn file .sql, vd: ./scripts/restore.sh backups/drone_20261116_120000.sql}"
[ -f "$file" ] || { echo "Không thấy file $file" >&2; exit 1; }
read -r -p "Ghi đè toàn bộ dữ liệu hiện tại bằng $file (y/N)? " ans
[[ "$ans" =~ ^[Yy]$ ]] || { echo "Đã hủy."; exit 0; }
docker compose exec -T db psql -U drone -d drone -v ON_ERROR_STOP=1 < "$file"
echo "Đã khôi phục từ $file. Khởi động lại API: docker compose restart api"
