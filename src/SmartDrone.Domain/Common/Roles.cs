namespace SmartDrone.Domain.Common;

public static class Roles
{
    public const string Customer = "Customer";
    public const string Dispatcher = "Dispatcher";
    public const string StationOperator = "StationOperator";
    public const string Manager = "Manager";
    public const string Admin = "Admin";

    // Dùng cho [Authorize(Roles = ...)]
    public const string AdminOrManager = "Admin,Manager";

    public static readonly string[] All = { Customer, Dispatcher, StationOperator, Manager, Admin };
}
