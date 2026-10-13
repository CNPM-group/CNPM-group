-- =====================================================================
-- SmartDroneDelivery - PostgreSQL schema v2 (PostgreSQL 13+)
--
-- LƯU Ý QUAN TRỌNG:
--  * File này là BẢN VẼ (ERD/tài liệu) và để dựng DB thử nhanh bằng psql.
--  * Trong project thật, EF Core migrations mới là nguồn sự thật. KHÔNG chạy
--    file này song song với migration trên cùng một database.
--  * Bảng `users` ở đây tương ứng AspNetUsers (ASP.NET Identity) trong code.
--    Role lưu thành chuỗi PascalCase giống Roles.cs: Customer, Dispatcher,
--    StationOperator, Manager, Admin. Status cũng PascalCase giống enum C#.
--
-- Chạy thử:
--   psql -h localhost -U drone -d drone -f schema.sql
-- Reset sạch (CHỈ dev):
--   DROP SCHEMA public CASCADE; CREATE SCHEMA public;
-- =====================================================================

BEGIN;

-- ---------------------------------------------------------------------
-- 0. Hàm dùng chung
-- ---------------------------------------------------------------------

-- Tự cập nhật cột updated_at
CREATE OR REPLACE FUNCTION fn_set_updated_at() RETURNS trigger
LANGUAGE plpgsql AS $$
BEGIN
    NEW.updated_at := now();
    RETURN NEW;
END;
$$;

-- Khoảng cách đường chim bay (km) giữa 2 tọa độ. Dùng cho Station_Routes và ETA.
CREATE OR REPLACE FUNCTION fn_haversine_km(
    lat1 double precision, lon1 double precision,
    lat2 double precision, lon2 double precision
) RETURNS numeric
LANGUAGE sql IMMUTABLE AS $$
    SELECT round((2 * 6371 * asin(sqrt(
        power(sin(radians(lat2 - lat1) / 2), 2) +
        cos(radians(lat1)) * cos(radians(lat2)) *
        power(sin(radians(lon2 - lon1) / 2), 2)
    )))::numeric, 3);
$$;

-- ---------------------------------------------------------------------
-- 1. USERS & ĐỊA CHỈ
-- ---------------------------------------------------------------------

CREATE TABLE users (
    user_id        uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    email          varchar(255) NOT NULL UNIQUE,
    password_hash  varchar(255) NOT NULL,
    full_name      varchar(100) NOT NULL,
    phone          varchar(20),
    role           varchar(30)  NOT NULL DEFAULT 'Customer',
    is_active      boolean      NOT NULL DEFAULT true,
    created_at     timestamptz  NOT NULL DEFAULT now(),
    CONSTRAINT ck_users_role CHECK (role IN
        ('Customer', 'Dispatcher', 'StationOperator', 'Manager', 'Admin'))
);

CREATE TABLE customer_addresses (
    address_id     uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    customer_id    uuid NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    label          varchar(50)  NOT NULL,           -- "KTX", "Nhà", "Công ty"
    address_line   varchar(255) NOT NULL,
    latitude       numeric(9,6),
    longitude      numeric(9,6),
    is_default     boolean      NOT NULL DEFAULT false,
    created_at     timestamptz  NOT NULL DEFAULT now(),
    CONSTRAINT ck_addr_lat CHECK (latitude  IS NULL OR latitude  BETWEEN -90  AND 90),
    CONSTRAINT ck_addr_lng CHECK (longitude IS NULL OR longitude BETWEEN -180 AND 180)
);
CREATE INDEX ix_addresses_customer ON customer_addresses (customer_id);
-- Mỗi khách chỉ có tối đa 1 địa chỉ mặc định
CREATE UNIQUE INDEX ux_addresses_one_default
    ON customer_addresses (customer_id) WHERE is_default;

-- ---------------------------------------------------------------------
-- 2. TRẠM ĐÁP, TUYẾN, OPERATOR
-- ---------------------------------------------------------------------

CREATE TABLE landing_stations (
    station_id     uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    station_name   varchar(100) NOT NULL UNIQUE,
    latitude       numeric(9,6) NOT NULL,
    longitude      numeric(9,6) NOT NULL,
    capacity       smallint     NOT NULL DEFAULT 2,  -- số drone đậu cùng lúc
    status         varchar(20)  NOT NULL DEFAULT 'Available',
    created_at     timestamptz  NOT NULL DEFAULT now(),
    CONSTRAINT ck_station_status CHECK (status IN ('Available', 'Occupied', 'Maintenance')),
    CONSTRAINT ck_station_lat    CHECK (latitude  BETWEEN -90  AND 90),
    CONSTRAINT ck_station_lng    CHECK (longitude BETWEEN -180 AND 180),
    CONSTRAINT ck_station_cap    CHECK (capacity > 0)
);
CREATE INDEX ix_stations_status ON landing_stations (status);

-- Cạnh của đồ thị (có hướng). Cần cho Dijkstra và ETA nhiều chặng.
CREATE TABLE station_routes (
    route_id         uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    from_station_id  uuid NOT NULL REFERENCES landing_stations(station_id) ON DELETE CASCADE,
    to_station_id    uuid NOT NULL REFERENCES landing_stations(station_id) ON DELETE CASCADE,
    distance_km      numeric(8,3) NOT NULL,
    is_active        boolean      NOT NULL DEFAULT true,
    CONSTRAINT uq_route_pair   UNIQUE (from_station_id, to_station_id),
    CONSTRAINT ck_route_diff   CHECK (from_station_id <> to_station_id),
    CONSTRAINT ck_route_dist   CHECK (distance_km > 0)
);
CREATE INDEX ix_routes_to ON station_routes (to_station_id);

-- Operator phụ trách trạm nào (nhiều-nhiều)
CREATE TABLE station_operators (
    station_id  uuid NOT NULL REFERENCES landing_stations(station_id) ON DELETE CASCADE,
    user_id     uuid NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    PRIMARY KEY (station_id, user_id)
);
CREATE INDEX ix_station_operators_user ON station_operators (user_id);

-- ---------------------------------------------------------------------
-- 3. DRONE
-- ---------------------------------------------------------------------

CREATE TABLE drones (
    drone_id           uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    drone_code         varchar(30)  NOT NULL UNIQUE,
    model              varchar(100) NOT NULL,
    max_payload_kg     numeric(5,2) NOT NULL,
    max_range_km       numeric(6,2) NOT NULL,
    cruise_speed_kmh   numeric(5,1) NOT NULL,
    battery_pct        smallint     NOT NULL DEFAULT 100,
    status             varchar(20)  NOT NULL DEFAULT 'Idle',
    current_station_id uuid REFERENCES landing_stations(station_id) ON DELETE SET NULL,
    created_at         timestamptz  NOT NULL DEFAULT now(),
    updated_at         timestamptz  NOT NULL DEFAULT now(),
    CONSTRAINT ck_drone_status  CHECK (status IN ('Idle', 'Assigned', 'InFlight', 'Charging', 'Maintenance')),
    CONSTRAINT ck_drone_battery CHECK (battery_pct BETWEEN 0 AND 100),
    CONSTRAINT ck_drone_payload CHECK (max_payload_kg > 0),
    CONSTRAINT ck_drone_range   CHECK (max_range_km > 0),
    CONSTRAINT ck_drone_speed   CHECK (cruise_speed_kmh > 0)
);
CREATE INDEX ix_drones_status  ON drones (status);
CREATE INDEX ix_drones_station ON drones (current_station_id);
CREATE TRIGGER trg_drones_updated_at BEFORE UPDATE ON drones
    FOR EACH ROW EXECUTE FUNCTION fn_set_updated_at();

-- ---------------------------------------------------------------------
-- 4. KIỆN HÀNG
-- ---------------------------------------------------------------------

CREATE TABLE packages (
    package_id   uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    description  varchar(255),
    weight_kg    numeric(6,2) NOT NULL,
    length_cm    numeric(6,1) NOT NULL,
    width_cm     numeric(6,1) NOT NULL,
    height_cm    numeric(6,1) NOT NULL,
    image_key    varchar(255),            -- object key trong MinIO, KHÔNG lưu URL cứng
    created_at   timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT ck_pkg_weight CHECK (weight_kg > 0),
    CONSTRAINT ck_pkg_dims   CHECK (length_cm > 0 AND width_cm > 0 AND height_cm > 0)
);

-- ---------------------------------------------------------------------
-- 5. ĐƠN HÀNG
-- ---------------------------------------------------------------------

CREATE TABLE delivery_orders (
    order_id               uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    tracking_code          varchar(20) NOT NULL UNIQUE
                           DEFAULT ('DR' || upper(substr(md5(gen_random_uuid()::text), 1, 8))),
    customer_id            uuid NOT NULL REFERENCES users(user_id) ON DELETE RESTRICT,
    package_id             uuid NOT NULL UNIQUE REFERENCES packages(package_id) ON DELETE RESTRICT,
    origin_station_id      uuid NOT NULL REFERENCES landing_stations(station_id) ON DELETE RESTRICT,
    destination_station_id uuid NOT NULL REFERENCES landing_stations(station_id) ON DELETE RESTRICT,
    delivery_address_id    uuid REFERENCES customer_addresses(address_id) ON DELETE SET NULL,
    status                 varchar(20) NOT NULL DEFAULT 'Pending',
    approved_by            uuid REFERENCES users(user_id) ON DELETE SET NULL,   -- Dispatcher duyệt
    approved_at            timestamptz,
    scheduled_at           timestamptz,
    eta                    timestamptz,                                         -- thuật toán/AI cập nhật
    completed_at           timestamptz,
    failure_reason         text,
    notes                  text,
    created_at             timestamptz NOT NULL DEFAULT now(),
    updated_at             timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT ck_order_status CHECK (status IN
        ('Pending', 'Approved', 'Rejected', 'Scheduled', 'InTransit', 'ArrivedAtStation',
         'Completed', 'Failed', 'Cancelled')),
    CONSTRAINT ck_order_stations CHECK (origin_station_id <> destination_station_id),
    CONSTRAINT ck_order_failure  CHECK (status <> 'Failed' OR failure_reason IS NOT NULL)
);
-- Postgres KHÔNG tự tạo index cho FK, phải tự thêm
CREATE INDEX ix_orders_customer    ON delivery_orders (customer_id, created_at DESC);
CREATE INDEX ix_orders_status      ON delivery_orders (status);
CREATE INDEX ix_orders_created     ON delivery_orders (created_at DESC);
CREATE INDEX ix_orders_origin      ON delivery_orders (origin_station_id);
CREATE INDEX ix_orders_destination ON delivery_orders (destination_station_id);
CREATE TRIGGER trg_orders_updated_at BEFORE UPDATE ON delivery_orders
    FOR EACH ROW EXECUTE FUNCTION fn_set_updated_at();

-- Lịch sử đổi trạng thái: nguồn dữ liệu cho timeline tracking và AI summarization
CREATE TABLE order_status_history (
    history_id   bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    order_id     uuid NOT NULL REFERENCES delivery_orders(order_id) ON DELETE CASCADE,
    from_status  varchar(20),
    to_status    varchar(20) NOT NULL,
    changed_by   uuid REFERENCES users(user_id) ON DELETE SET NULL,
    note         text,
    changed_at   timestamptz NOT NULL DEFAULT now()
);
CREATE INDEX ix_status_history_order ON order_status_history (order_id, changed_at);

-- ---------------------------------------------------------------------
-- 6. CHUYẾN BAY (1 đơn có thể nhiều chặng nếu đi qua trạm trung gian)
-- ---------------------------------------------------------------------

CREATE TABLE drone_flights (
    flight_id              uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id               uuid NOT NULL REFERENCES delivery_orders(order_id) ON DELETE CASCADE,
    drone_id               uuid NOT NULL REFERENCES drones(drone_id) ON DELETE RESTRICT,
    leg_index              smallint NOT NULL DEFAULT 1,
    departure_station_id   uuid NOT NULL REFERENCES landing_stations(station_id) ON DELETE RESTRICT,
    arrival_station_id     uuid NOT NULL REFERENCES landing_stations(station_id) ON DELETE RESTRICT,
    scheduled_departure_at timestamptz,
    actual_departure_at    timestamptz,
    actual_arrival_at      timestamptz,
    current_latitude       numeric(9,6),     -- snapshot, ghi mỗi vài giây, đừng ghi mỗi frame
    current_longitude      numeric(9,6),
    flight_status          varchar(20) NOT NULL DEFAULT 'Scheduled',
    created_at             timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT uq_flight_leg      UNIQUE (order_id, leg_index),
    CONSTRAINT ck_flight_status   CHECK (flight_status IN ('Scheduled', 'Flying', 'Landed', 'Aborted')),
    CONSTRAINT ck_flight_stations CHECK (departure_station_id <> arrival_station_id),
    CONSTRAINT ck_flight_leg      CHECK (leg_index > 0)
);
CREATE INDEX ix_flights_order     ON drone_flights (order_id);
CREATE INDEX ix_flights_drone     ON drone_flights (drone_id);
CREATE INDEX ix_flights_status    ON drone_flights (flight_status);
CREATE INDEX ix_flights_departure ON drone_flights (departure_station_id);
CREATE INDEX ix_flights_arrival   ON drone_flights (arrival_station_id);

-- Lịch sử vị trí (tuỳ chọn, chỉ cần khi làm bản đồ/replay)
CREATE TABLE flight_telemetry (
    telemetry_id bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    flight_id    uuid NOT NULL REFERENCES drone_flights(flight_id) ON DELETE CASCADE,
    latitude     numeric(9,6) NOT NULL,
    longitude    numeric(9,6) NOT NULL,
    altitude_m   numeric(6,1),
    battery_pct  smallint,
    recorded_at  timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT ck_telemetry_battery CHECK (battery_pct IS NULL OR battery_pct BETWEEN 0 AND 100)
);
CREATE INDEX ix_telemetry_flight_time ON flight_telemetry (flight_id, recorded_at DESC);

-- ---------------------------------------------------------------------
-- 7. XÁC NHẬN NHẬN HÀNG (OTP/QR)
-- ---------------------------------------------------------------------

CREATE TABLE delivery_confirmations (
    confirmation_id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    order_id        uuid NOT NULL UNIQUE REFERENCES delivery_orders(order_id) ON DELETE CASCADE,
    code_hash       varchar(255) NOT NULL,           -- lưu HASH của OTP, không lưu OTP gốc
    method          varchar(10)  NOT NULL DEFAULT 'OTP',
    expires_at      timestamptz  NOT NULL,
    failed_attempts smallint     NOT NULL DEFAULT 0,
    verified_at     timestamptz,                     -- lúc Operator xác nhận khách đã nhận
    verified_by     uuid REFERENCES users(user_id) ON DELETE SET NULL,
    created_at      timestamptz  NOT NULL DEFAULT now(),
    CONSTRAINT ck_confirm_method CHECK (method IN ('OTP', 'QR')),
    CONSTRAINT ck_confirm_attempts CHECK (failed_attempts >= 0)
);

-- ---------------------------------------------------------------------
-- 8. THÔNG BÁO, AUDIT, AI, API NGOÀI
-- ---------------------------------------------------------------------

CREATE TABLE notifications (
    notification_id uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id         uuid NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    order_id        uuid REFERENCES delivery_orders(order_id) ON DELETE CASCADE,
    title           varchar(150) NOT NULL,
    body            text,
    is_read         boolean     NOT NULL DEFAULT false,
    created_at      timestamptz NOT NULL DEFAULT now()
);
CREATE INDEX ix_notifications_user ON notifications (user_id, is_read, created_at DESC);

CREATE TABLE audit_logs (
    audit_id     bigint GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id      uuid REFERENCES users(user_id) ON DELETE SET NULL,
    action       varchar(50)  NOT NULL,        -- Create / Update / Delete / Login ...
    entity_name  varchar(100) NOT NULL,
    entity_id    varchar(64),
    old_values   jsonb,
    new_values   jsonb,
    ip_address   varchar(45),
    created_at   timestamptz NOT NULL DEFAULT now()
);
CREATE INDEX ix_audit_entity  ON audit_logs (entity_name, entity_id);
CREATE INDEX ix_audit_user    ON audit_logs (user_id);
CREATE INDEX ix_audit_created ON audit_logs (created_at DESC);

CREATE TABLE ai_chat_messages (
    message_id  uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    user_id     uuid NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    session_id  uuid NOT NULL,
    sender      varchar(10) NOT NULL,
    content     text        NOT NULL,
    created_at  timestamptz NOT NULL DEFAULT now(),
    CONSTRAINT ck_chat_sender CHECK (sender IN ('User', 'Assistant'))
);
CREATE INDEX ix_chat_session ON ai_chat_messages (user_id, session_id, created_at);

-- API key cho hệ thống bên ngoài tích hợp (đề yêu cầu RESTful cho external integration)
CREATE TABLE api_keys (
    api_key_id   uuid PRIMARY KEY DEFAULT gen_random_uuid(),
    name         varchar(100) NOT NULL,
    key_hash     varchar(255) NOT NULL UNIQUE,   -- chỉ lưu hash, key gốc hiện 1 lần khi tạo
    is_active    boolean     NOT NULL DEFAULT true,
    created_at   timestamptz NOT NULL DEFAULT now(),
    last_used_at timestamptz
);

-- ---------------------------------------------------------------------
-- 9. DỮ LIỆU MẪU (dev). Users do app seed vì cần password hash đúng chuẩn.
-- ---------------------------------------------------------------------

INSERT INTO landing_stations (station_name, latitude, longitude, capacity) VALUES
    ('Trạm KTX Khu A',     10.883000, 106.807000, 3),
    ('Trạm Thư viện',      10.881500, 106.805500, 2),
    ('Trạm Nhà điều hành', 10.882200, 106.808800, 2),
    ('Trạm Căng tin',      10.884100, 106.806200, 2),
    ('Trạm Cổng chính',    10.880500, 106.807900, 4)
ON CONFLICT (station_name) DO NOTHING;

-- Full-mesh 2 chiều giữa các trạm, khoảng cách tính bằng haversine
INSERT INTO station_routes (from_station_id, to_station_id, distance_km)
SELECT a.station_id, b.station_id,
       fn_haversine_km(a.latitude::double precision, a.longitude::double precision,
                       b.latitude::double precision, b.longitude::double precision)
FROM landing_stations a
CROSS JOIN landing_stations b
WHERE a.station_id <> b.station_id
ON CONFLICT (from_station_id, to_station_id) DO NOTHING;

INSERT INTO drones (drone_code, model, max_payload_kg, max_range_km, cruise_speed_kmh, current_station_id)
SELECT v.code, 'SD-Quad X1', 5.00, 15.00, 40.0, s.station_id
FROM (VALUES
        ('DRN-001', 'Trạm Cổng chính'),
        ('DRN-002', 'Trạm Cổng chính'),
        ('DRN-003', 'Trạm KTX Khu A')
     ) AS v(code, station_name)
JOIN landing_stations s ON s.station_name = v.station_name
ON CONFLICT (drone_code) DO NOTHING;

COMMIT;
