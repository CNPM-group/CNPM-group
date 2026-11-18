namespace SmartDrone.Domain.Models;

public sealed record StoredFile(Stream Content, string ContentType);

public interface IFileStorage
{
    Task<string> SaveAsync(Stream content, long length, string contentType, string key, CancellationToken ct = default);
    Task<StoredFile?> OpenAsync(string key, CancellationToken ct = default);
}
