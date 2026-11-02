
# Analytics KPI Definitions

## 1. Overview
This document defines the key performance indicators (KPIs) for SmartDroneDelivery.

## 2. KPI List

| KPI | Description | Calculation |
|---|---|---|
| Total Orders | Total delivery orders | COUNT(order_id) |
| Completed Orders | Successfully completed orders | COUNT(status = 'Completed') |
| Failed Orders | Failed delivery orders | COUNT(status = 'Failed') |
| Pending Orders | Orders waiting for processing | COUNT(status = 'Pending') |
| Delivery Success Rate | Successful delivery percentage | Completed Orders / Total Orders * 100 |
| Average Delivery Time | Average time to complete delivery | AVG(completed_at - created_at) |
| Total Drones | Total registered drones | COUNT(drone_id) |

## 3. Data Sources
- Delivery orders
- Drone information
- Customer information
- Drone flight information

## 4. Notes
All KPIs should be calculated using actual database records and the project's seed data.
