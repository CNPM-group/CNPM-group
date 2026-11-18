using SmartDrone.Domain.Models;
using SmartDrone.Services;
using Xunit;

namespace SmartDrone.Tests;

// Bản tự ghép khi không có LLM (AiInsightService.RuleSummary / RuleInsights).
public class AiRuleTextTests
{
    private static SummaryFacts Facts(string status, int? lateMinutes = null, string? failure = null) => new(
        "SDD-7f3a21c9e04b4d1a", status, "Trạm Thư viện", "Trạm KTX Khu A", 1.5m, "Sách",
        "09:00 16/11", "09:05 16/11", "09:12 16/11", "09:24 16/11", "09:26 16/11", lateMinutes,
        status == "Completed" ? "09:31 16/11" : null, status == "Completed" ? 31 : null,
        "DRONE-002", 1, failure, null, Array.Empty<string>());

    [Fact]
    public void Tom_tat_don_hoan_tat_co_ma_drone_va_som_tre()
    {
        var text = AiInsightService.RuleSummary(Facts("Completed", lateMinutes: -2));
        Assert.Contains("SDD-7f3a21c9e04b4d1a", text);
        Assert.Contains("DRONE-002", text);
        Assert.Contains("sớm 2 phút", text);
        Assert.Contains("hoàn tất", text);
        Assert.Contains("31 phút", text);
    }

    [Fact]
    public void Tom_tat_don_that_bai_co_ly_do() =>
        Assert.Contains("Gió mạnh", AiInsightService.RuleSummary(Facts("Failed", failure: "Gió mạnh")));

    [Fact]
    public void Khong_co_don_thi_noi_chua_du_du_lieu()
    {
        var facts = new InsightFacts("16/10/2026", "16/11/2026", 0, 0, 0, 0, 0, null, null, 0,
            Array.Empty<StatusCount>(), Array.Empty<ReasonCount>(), Array.Empty<RouteCount>());
        var points = AiInsightService.RuleInsights(facts);
        Assert.Single(points);
        Assert.Contains("chưa đủ dữ liệu", points[0]);
    }

    [Fact]
    public void Ty_le_thap_thi_canh_bao_kem_ly_do()
    {
        var facts = new InsightFacts("16/10/2026", "16/11/2026", 10, 6, 4, 0, 60, 15, "12/11", 4,
            Array.Empty<StatusCount>(), new[] { new ReasonCount("Gió mạnh", 3) }, new[] { new RouteCount("A → B", 5) });
        var points = AiInsightService.RuleInsights(facts);
        Assert.Contains(points, p => p.Contains("dưới 90 %") && p.Contains("Gió mạnh"));
        Assert.Contains(points, p => p.Contains("A → B"));
        Assert.True(points.Count <= 5);
    }
}
