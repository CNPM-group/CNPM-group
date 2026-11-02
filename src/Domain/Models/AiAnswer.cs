namespace SmartDrone.Domain.Models;

public sealed record AiAnswer(string Model, string? Answer, long LatencyMs, object? Usage = null);
