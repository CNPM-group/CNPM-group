using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDrone.Api.Schemas;
using SmartDrone.Domain;
using SmartDrone.Services;

namespace SmartDrone.Api.Controllers;

[ApiController]
[Route(ApiConstants.Prefix + "/reports")]
[Authorize(Roles = Roles.AdminOrManager)]
public class ReportsController(ReportService reports) : ControllerBase
{
    [HttpGet("orders")]
    public async Task<ActionResult<ReportResponse>> Orders([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct) =>
        Ok(ReportResponse.From(await reports.SummaryAsync(from, to, ct)));

    // FR11: hiệu quả từng trạm trong khoảng from–to.
    [HttpGet("stations")]
    public async Task<ActionResult<IReadOnlyList<StationReportResponse>>> Stations(
        [FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct) =>
        Ok((await reports.StationsAsync(from, to, ct)).Select(StationReportResponse.From).ToList());

    // AI nhận xét số liệu vận hành trong khoảng from–to (mặc định 30 ngày gần nhất).
    [HttpGet("ai-insights")]
    public async Task<ActionResult<AiInsightsResponse>> AiInsights(
        [FromQuery] DateTime? from, [FromQuery] DateTime? to, [FromServices] AiInsightService ai, CancellationToken ct) =>
        Ok(AiInsightsResponse.From(await ai.InsightsAsync(from, to, ct)));

    [HttpGet("orders.csv")]
    public async Task<IActionResult> OrdersCsv([FromQuery] DateTime? from, [FromQuery] DateTime? to, CancellationToken ct)
    {
        var csv = await reports.ExportCsvAsync(from, to, ct);
        var bytes = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csv)).ToArray();
        return File(bytes, "text/csv", $"orders_{DateTime.UtcNow:yyyyMMdd}.csv");
    }
}
