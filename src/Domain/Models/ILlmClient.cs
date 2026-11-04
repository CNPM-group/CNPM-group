using System.Text.Json;

namespace SmartDrone.Domain.Models;

public interface ILlmClient
{
    string Model { get; }
    bool IsConfigured { get; }
    Task<JsonDocument> ChatAsync(object payload, CancellationToken ct = default);
}
