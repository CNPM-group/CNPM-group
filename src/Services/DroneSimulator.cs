using Microsoft.Extensions.Options;
using SmartDrone.Domain.Models;

namespace SmartDrone.Services;

// Chạy nền cùng API: cứ vài giây dời các drone đang bay một đoạn và báo vị trí qua SignalR.
public sealed class DroneSimulator(
    IServiceScopeFactory scopes,
    IOrderEvents events,
    IOptions<SimulatorOptions> options,
    ILogger<DroneSimulator> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var settings = options.Value;
        if (!settings.Enabled)
            return;

        using var timer = new PeriodicTimer(TimeSpan.FromSeconds(Math.Max(1, settings.IntervalSeconds)));
        while (await timer.WaitForNextTickAsync(stoppingToken))
        {
            try
            {
                using var scope = scopes.CreateScope();
                var tracking = scope.ServiceProvider.GetRequiredService<TrackingService>();
                foreach (var snapshot in await tracking.MoveFlyingDronesAsync(stoppingToken))
                    await events.DroneMovedAsync(snapshot, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "Giả lập drone lỗi ở lượt này, lượt sau chạy lại.");
            }
        }
    }
}
