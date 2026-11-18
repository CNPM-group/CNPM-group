namespace SmartDrone.Domain.Models;

public sealed record OrderDetail(DeliveryOrder Order, Package Package, LandingStation Origin, LandingStation Destination);
