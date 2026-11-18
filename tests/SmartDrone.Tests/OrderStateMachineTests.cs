using SmartDrone.Domain;
using SmartDrone.Domain.Models;
using Xunit;

namespace SmartDrone.Tests;

// Bảng chuyển trạng thái ở mục IV.3.1 của báo cáo.
public class OrderStateMachineTests
{
    [Theory]
    [InlineData("Pending", OrderStatus.Approved)]
    [InlineData("Pending", OrderStatus.Rejected)]
    [InlineData("Pending", OrderStatus.Cancelled)]
    [InlineData("Approved", OrderStatus.Scheduled)]
    [InlineData("Scheduled", OrderStatus.InTransit)]
    [InlineData("InTransit", OrderStatus.ArrivedAtStation)]
    [InlineData("InTransit", OrderStatus.Failed)]
    [InlineData("ArrivedAtStation", OrderStatus.Completed)]
    [InlineData("ArrivedAtStation", OrderStatus.Failed)]
    [InlineData("Failed", OrderStatus.Scheduled)]
    [InlineData("Failed", OrderStatus.Cancelled)]
    public void Cho_phep_dung_bang_chuyen(string from, OrderStatus to) =>
        Assert.True(OrderStateMachine.CanTransition(from, to));

    [Theory]
    [InlineData("InTransit", OrderStatus.Approved)]
    [InlineData("Completed", OrderStatus.Failed)]
    [InlineData("Cancelled", OrderStatus.Pending)]
    [InlineData("Approved", OrderStatus.Cancelled)]
    [InlineData("Rejected", OrderStatus.Approved)]
    [InlineData("KhongCo", OrderStatus.Approved)]
    public void Chan_chuyen_sai(string from, OrderStatus to)
    {
        Assert.False(OrderStateMachine.CanTransition(from, to));
        Assert.Throws<ConflictException>(() => OrderStateMachine.EnsureTransition(from, to));
    }
}
