namespace SmartDrone.Domain;

public static class ApiConstants
{
    public const string Prefix = "api/v1";
    public const int DefaultPageSize = 20;
    public const int MaxPageSize = 100;
    public const int MaxPromptLength = 2000;
}

public static class Roles
{
    public const string Customer = "Customer";
    public const string Dispatcher = "Dispatcher";
    public const string StationOperator = "StationOperator";
    public const string Manager = "Manager";
    public const string Admin = "Admin";

    public const string AdminOrManager = "Admin,Manager";
    public const string DispatchStaff = "Dispatcher,Manager,Admin";
    public const string OperationsStaff = "Dispatcher,StationOperator,Manager,Admin";

    public static readonly string[] All = { Customer, Dispatcher, StationOperator, Manager, Admin };
    public static readonly string[] Staff = { Dispatcher, Manager, Admin };
}

public enum OrderStatus
{
    Pending,
    Approved,
    Rejected,
    Scheduled,
    InTransit,
    ArrivedAtStation,
    Completed,
    Failed,
    Cancelled
}

public enum StationStatus
{
    Available,
    Occupied,
    Maintenance
}

public enum DroneStatus
{
    Idle,
    Assigned,
    InFlight,
    Charging,
    Maintenance
}
