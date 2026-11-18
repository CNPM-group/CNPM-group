using SmartDrone.Services;
using Xunit;

namespace SmartDrone.Tests;

// Công thức % đã bay của bộ giả lập (TrackingService.Progress).
public class TrackingProgressTests
{
    private static readonly DateTime Start = new(2026, 11, 16, 2, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime Eta = Start.AddMinutes(10);

    [Fact]
    public void Bay_duoc_nua_duong_la_50_phan_tram() =>
        Assert.Equal(0.5, TrackingService.Progress("InTransit", Start, Eta, Start.AddMinutes(5)), 3);

    [Fact]
    public void Qua_ETA_thi_dung_o_100_phan_tram() =>
        Assert.Equal(1, TrackingService.Progress("InTransit", Start, Eta, Start.AddMinutes(30)));

    [Fact]
    public void Chua_cat_canh_la_0() =>
        Assert.Equal(0, TrackingService.Progress("Scheduled", null, Eta, Start));

    [Fact]
    public void Da_toi_tram_la_100() =>
        Assert.Equal(1, TrackingService.Progress("ArrivedAtStation", Start, Eta, Start));

    [Fact]
    public void TimeScale_10_thi_1_phut_bang_10_phut() =>
        Assert.Equal(1, TrackingService.Progress("InTransit", Start, Eta, Start.AddMinutes(1), timeScale: 10), 3);
}
