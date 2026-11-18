#!/usr/bin/env bash
# Sao lưu CSDL PostgreSQL ra file .sql trong thư mục backups/ (NFR Sao lưu, v0.11).
# Dùng: ./scripts/backup.sh            → backups/drone_YYYYMMDD_HHMMSS.sql
set -euo pipefail
cd "$(dirname "$0")/.."
mkdir -p backups
file="backups/drone_$(date +%Y%m%d_%H%M%S).sql"
docker compose exec -T db pg_dump -U drone -d drone --clean --if-exists --no-owner > "$file"
echo "Đã sao lưu: $file ($(wc -c < "$file") byte)"
