namespace SmartDrone.Domain.Models;

public static class OrderStateMachine
{
    private static readonly Dictionary<OrderStatus, OrderStatus[]> Transitions = new()
    {
        [OrderStatus.Pending] = new[] { OrderStatus.Approved, OrderStatus.Rejected, OrderStatus.Cancelled },
        [OrderStatus.Approved] = new[] { OrderStatus.Scheduled },
        [OrderStatus.Scheduled] = new[] { OrderStatus.InTransit },
        [OrderStatus.InTransit] = new[] { OrderStatus.ArrivedAtStation, OrderStatus.Failed },
        [OrderStatus.ArrivedAtStation] = new[] { OrderStatus.Completed, OrderStatus.Failed },
        [OrderStatus.Failed] = new[] { OrderStatus.Scheduled, OrderStatus.Cancelled }
    };

    public static bool CanTransition(string from, OrderStatus to) =>
        Enum.TryParse<OrderStatus>(from, out var current) &&
        Transitions.TryGetValue(current, out var next) &&
        next.Contains(to);

    public static void EnsureTransition(string from, OrderStatus to)
    {
        if (!CanTransition(from, to))
            throw new ConflictException($"Không thể chuyển đơn từ {from} sang {to}.");
    }
}
