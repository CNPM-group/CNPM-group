
-- SmartDroneDelivery Analytics
-- PostgreSQL

-- 1. Total delivery orders
SELECT COUNT(*) AS total_orders
FROM delivery_orders;

-- 2. Orders by status
SELECT status, COUNT(*) AS total_orders
FROM delivery_orders
GROUP BY status;

-- 3. Completed deliveries
SELECT COUNT(*) AS completed_orders
FROM delivery_orders
WHERE status = 'Completed';

-- 4. Failed deliveries
SELECT COUNT(*) AS failed_orders
FROM delivery_orders
WHERE status = 'Failed';

-- 5. Pending deliveries
SELECT COUNT(*) AS pending_orders
FROM delivery_orders
WHERE status = 'Pending';

-- 6. Delivery success rate
SELECT
    ROUND(
        COUNT(*) FILTER (WHERE status = 'Completed') * 100.0
        / NULLIF(COUNT(*), 0), 2
    ) AS success_rate
FROM delivery_orders;
