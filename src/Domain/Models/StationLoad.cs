namespace SmartDrone.Domain.Models;

// Tải của 1 trạm lúc này (v0.11).
// Incoming: đơn đã gán drone hoặc đang bay tới trạm. Waiting: kiện đã tới, chờ khách nhận.
// Outgoing: đơn sẽ cất cánh từ trạm (Pending, Approved, Scheduled).
public sealed record StationLoad(Guid StationId, int Incoming, int Waiting, int Outgoing);

// Hiệu quả 1 trạm trong khoảng thời gian (v0.11, FR11 "phân tích hiệu quả theo trạm").
public sealed record StationReport(
    string Station,
    int Sent,
    int Received,
    int Completed,
    int Failed,
    double SuccessRatePct,
    double? AvgDeliveryMinutes);
