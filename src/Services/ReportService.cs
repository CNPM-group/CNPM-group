using System.Globalization;
using System.Text;
using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Services;

public sealed class ReportService(IReportRepository reports)
{
    private const int MaxRangeDays = 366;

    public async Task<OrderReport> SummaryAsync(DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var (fromUtc, toUtc) = Range(from, to);
        var rows = await reports.OrdersInRangeAsync(fromUtc, toUtc, ct);

        var total = rows.Count;
        var completed = rows.Count(r => r.Status == nameof(OrderStatus.Completed));
        var failed = rows.Count(r => r.Status == nameof(OrderStatus.Failed));
        var finished = completed + failed;

        var durations = rows
            .Where(r => r.CompletedAtUtc is not null)
            .Select(r => (r.CompletedAtUtc!.Value - r.CreatedAtUtc).TotalMinutes)
            .ToList();

        return new OrderReport(
            fromUtc,
            toUtc,
            total,
            completed,
            failed,
            finished == 0 ? 0 : Math.Round(completed * 100.0 / finished, 1),
            durations.Count == 0 ? null : Math.Round(durations.Average(), 1),
            rows.GroupBy(r => r.Status).Select(g => new StatusCount(g.Key, g.Count())).OrderByDescending(s => s.Count).ToList(),
            rows.GroupBy(r => DateOnly.FromDateTime(r.CreatedAtUtc)).Select(g => new DailyCount(g.Key, g.Count())).OrderBy(d => d.Day).ToList());
    }

    // FR11: hiệu quả từng trạm. Gửi = số đơn đi từ trạm, Nhận = số đơn tới trạm;
    // hoàn tất / thất bại / tỷ lệ / thời gian TB tính theo trạm nhận (nơi khách nhập OTP).
    public async Task<IReadOnlyList<StationReport>> StationsAsync(DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var (fromUtc, toUtc) = Range(from, to);
        var rows = await reports.ExportRowsAsync(fromUtc, toUtc, ct);

        var names = rows.Select(r => r.OriginStation).Concat(rows.Select(r => r.DestinationStation)).Distinct();

        return names
            .Select(name =>
            {
                var received = rows.Where(r => r.DestinationStation == name).ToList();
                var completed = received.Count(r => r.Status == nameof(OrderStatus.Completed));
                var failed = received.Count(r => r.Status == nameof(OrderStatus.Failed));
                var minutes = received
                    .Where(r => r.CompletedAtUtc is not null)
                    .Select(r => (r.CompletedAtUtc!.Value - r.CreatedAtUtc).TotalMinutes)
                    .ToList();

                return new StationReport(
                    name,
                    rows.Count(r => r.OriginStation == name),
                    received.Count,
                    completed,
                    failed,
                    completed + failed == 0 ? 0 : Math.Round(completed * 100.0 / (completed + failed), 1),
                    minutes.Count == 0 ? null : Math.Round(minutes.Average(), 1));
            })
            .OrderByDescending(s => s.Sent + s.Received)
            .ToList();
    }

    public async Task<string> ExportCsvAsync(DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var (fromUtc, toUtc) = Range(from, to);
        var rows = await reports.ExportRowsAsync(fromUtc, toUtc, ct);

        var csv = new StringBuilder();
        csv.AppendLine("tracking_code,status,origin_station,destination_station,weight_kg,created_at_utc,completed_at_utc,failure_reason");
        foreach (var r in rows)
        {
            csv.AppendLine(string.Join(",",
                Cell(r.TrackingCode),
                Cell(r.Status),
                Cell(r.OriginStation),
                Cell(r.DestinationStation),
                r.WeightKg.ToString(CultureInfo.InvariantCulture),
                r.CreatedAtUtc.ToString("o", CultureInfo.InvariantCulture),
                r.CompletedAtUtc?.ToString("o", CultureInfo.InvariantCulture) ?? "",
                Cell(r.FailureReason ?? "")));
        }

        return csv.ToString();
    }

    private static (DateTime From, DateTime To) Range(DateTime? from, DateTime? to)
    {
        var toUtc = (to ?? DateTime.UtcNow.Date.AddDays(1)).ToUniversalTime();
        var fromUtc = (from ?? toUtc.AddDays(-30)).ToUniversalTime();

        if (fromUtc >= toUtc)
            throw new ValidationException("Ngày bắt đầu phải trước ngày kết thúc.");
        if ((toUtc - fromUtc).TotalDays > MaxRangeDays)
            throw new ValidationException($"Khoảng thời gian tối đa {MaxRangeDays} ngày.");

        return (fromUtc, toUtc);
    }

    private static string Cell(string value) =>
        value.IndexOfAny(new[] { ',', '"', '\n', '\r' }) >= 0
            ? $"\"{value.Replace("\"", "\"\"")}\""
            : value;
}
