using SmartDrone.Domain.Models;

namespace SmartDrone.Api.Schemas;

public sealed record PromptRequest(string? Prompt);

public sealed record ChatRequest(string? Message);

public sealed record PromptResponse(string Model, string? Prompt, string? Answer, long LatencyMs, object? Usage)
{
    public static PromptResponse From(string? prompt, AiAnswer a) => new(a.Model, prompt, a.Answer, a.LatencyMs, a.Usage);
}

public sealed record ChatResponse(string Model, string? Answer, long LatencyMs)
{
    public static ChatResponse From(AiAnswer a) => new(a.Model, a.Answer, a.LatencyMs);
}

// source = "llm" (LLM viết) hoặc "rule" (server tự ghép từ dữ liệu khi không gọi được LLM); note giải thích vì sao.
public sealed record AiSummaryResponse(
    Guid OrderId, string TrackingCode, string Text, string Source, string? Model, DateTime GeneratedAtUtc, string? Note)
{
    public static AiSummaryResponse From(AiSummary s) =>
        new(s.OrderId, s.TrackingCode, s.Text, s.Source, s.Model, s.GeneratedAtUtc, s.Note);
}

public sealed record AiInsightsResponse(
    DateTime FromUtc, DateTime ToUtc, IReadOnlyList<string> Points, string Source, string? Model, DateTime GeneratedAtUtc, string? Note)
{
    public static AiInsightsResponse From(AiInsights i) =>
        new(i.FromUtc, i.ToUtc, i.Points, i.Source, i.Model, i.GeneratedAtUtc, i.Note);
}
