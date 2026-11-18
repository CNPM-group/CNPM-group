namespace SmartDrone.Domain.Models;

// Nguồn của đoạn văn AI trả về: LLM viết, hoặc server tự ghép theo luật khi không gọi được LLM.
public static class AiSource
{
    public const string Llm = "llm";
    public const string Rule = "rule";
}

// Tóm tắt 1 chuyến giao (KAN-62).
public sealed record AiSummary(
    Guid OrderId,
    string TrackingCode,
    string Text,
    string Source,
    string? Model,
    DateTime GeneratedAtUtc,
    string? Note);

// Nhận xét vận hành trong 1 khoảng thời gian, cho quản lý.
public sealed record AiInsights(
    DateTime FromUtc,
    DateTime ToUtc,
    IReadOnlyList<string> Points,
    string Source,
    string? Model,
    DateTime GeneratedAtUtc,
    string? Note);
