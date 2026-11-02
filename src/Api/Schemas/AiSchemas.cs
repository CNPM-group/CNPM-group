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
