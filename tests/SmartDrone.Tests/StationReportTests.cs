using SmartDrone.Domain.Models;
using SmartDrone.Services;
using Xunit;

namespace SmartDrone.Tests;

// FR11: ReportService.StationsAsync tính hiệu quả theo trạm từ dữ liệu giả.
public class StationReportTests
{
    private sealed class FakeReports(IReadOnlyList<OrderExportRow> rows) : IReportRepository
    {
        public Task<IReadOnlyList<(string Status, DateTime CreatedAtUtc, DateTime? CompletedAtUtc)>> OrdersInRangeAsync(
            DateTime fromUtc, DateTime toUtc, CancellationToken ct = default) =>
            Task.FromResult<IReadOnlyList<(string, DateTime, DateTime?)>>(
                rows.Select(r => (r.Status, r.CreatedAtUtc, r.CompletedAtUtc)).ToList());

        public Task<IReadOnlyList<OrderExportRow>> ExportRowsAsync(DateTime fromUtc, DateTime toUtc, CancellationToken ct = default) =>
            Task.FromResult(rows);
    }

    private static readonly DateTime T = DateTime.UtcNow.AddDays(-1);

    [Fact]
    public async Task Tinh_theo_tram_nhan()
    {
        var rows = new List<OrderExportRow>
        {
            new("SDD-1", "Completed", "A", "B", 1, T, T.AddMinutes(20), null),
            new("SDD-2", "Completed", "A", "B", 1, T, T.AddMinutes(10), null),
            new("SDD-3", "Failed", "C", "B", 1, T, null, "Gió mạnh"),
            new("SDD-4", "Pending", "B", "A", 1, T, null, null),
        };

        var report = await new ReportService(new FakeReports(rows)).StationsAsync(null, null);
        var b = report.Single(s => s.Station == "B");

        Assert.Equal(1, b.Sent);
        Assert.Equal(3, b.Received);
        Assert.Equal(2, b.Completed);
        Assert.Equal(1, b.Failed);
        Assert.Equal(66.7, b.SuccessRatePct);
        Assert.Equal(15, b.AvgDeliveryMinutes);
        Assert.Equal(0, report.Single(s => s.Station == "C").Received);
    }
}
