using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Options;
using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Infrastructure.Services;

public sealed class LlmClient(HttpClient http, IOptions<LlmOptions> options) : ILlmClient
{
    private readonly LlmOptions _opt = options.Value;

    public string Model => _opt.Model;

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_opt.ApiKey) &&
        !string.IsNullOrWhiteSpace(_opt.Model) &&
        Uri.TryCreate(_opt.BaseUrl, UriKind.Absolute, out var uri) &&
        uri.Scheme == Uri.UriSchemeHttps;

    public async Task<JsonDocument> ChatAsync(object payload, CancellationToken ct = default)
    {
        if (!IsConfigured)
            throw new ExternalServiceException("Chưa cấu hình đầy đủ LLM API.", 503);

        using var timeout = CancellationTokenSource.CreateLinkedTokenSource(ct);
        timeout.CancelAfter(TimeSpan.FromSeconds(_opt.TimeoutSeconds));

        using var request = new HttpRequestMessage(HttpMethod.Post, new Uri(new Uri(_opt.BaseUrl), "v1/chat/completions"));
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _opt.ApiKey);
        request.Content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        try
        {
            using var response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, timeout.Token);
            if (!response.IsSuccessStatusCode)
                throw new ExternalServiceException($"LLM provider trả về HTTP {(int)response.StatusCode}.");

            var body = await response.Content.ReadAsStringAsync(timeout.Token);
            return JsonDocument.Parse(body);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            throw new ExternalServiceException("LLM API timeout.", 504);
        }
        catch (HttpRequestException)
        {
            throw new ExternalServiceException("Không thể kết nối tới LLM provider.");
        }
        catch (JsonException)
        {
            throw new ExternalServiceException("Phản hồi từ LLM provider không hợp lệ.");
        }
    }
}
