#!/usr/bin/env bash
set -euo pipefail
cd "$(dirname "$0")/.."

[ -f .env ] || cp .env.example .env
docker compose up -d db

for i in $(seq 1 20); do
  if docker compose exec -T db pg_isready -U drone -d drone >/dev/null 2>&1; then
    echo "PostgreSQL san sang o localhost:5432 (db=drone, user=drone)"
    exit 0
  fi
  sleep 2
done

echo "PostgreSQL chua len sau 40s, xem: docker compose logs db" >&2
exit 1
