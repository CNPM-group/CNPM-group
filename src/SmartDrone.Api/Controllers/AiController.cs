
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartDrone.Api.Controllers;

[ApiController]
[Route("api/ai")]
[Authorize]
public class AiController : ControllerBase
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;

    public AiController(
        IHttpClientFactory httpClientFactory,
        IConfiguration configuration)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
    }

    public record FeasibilityRequest(string? Prompt);

    [HttpPost("feasibility-test")]
    public async Task<IActionResult> FeasibilityTest(
        [FromBody] FeasibilityRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
            return BadRequest(new { error = "Prompt không được để trống." });

        if (request.Prompt.Length > 2000)
            return BadRequest(new { error = "Prompt tối đa 2000 ký tự." });

        var apiKey = _configuration["Llm:ApiKey"];
        var baseUrl = _configuration["Llm:BaseUrl"];
        var model = _configuration["Llm:Model"];

        if (string.IsNullOrWhiteSpace(apiKey) ||
            string.IsNullOrWhiteSpace(baseUrl) ||
            string.IsNullOrWhiteSpace(model))
        {
            return StatusCode(500, new
            {
                error = "Chưa cấu hình đầy đủ LLM API."
            });
        }

        if (!Uri.TryCreate(baseUrl, UriKind.Absolute, out var uri) ||
            uri.Scheme != Uri.UriSchemeHttps)
        {
            return StatusCode(500, new
            {
                error = "Llm:BaseUrl phải là URL HTTPS hợp lệ."
            });
        }

        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(30);

        using var timeoutCts =
            CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken);

        timeoutCts.CancelAfter(TimeSpan.FromSeconds(30));

        var stopwatch = Stopwatch.StartNew();

        try
        {
            using var message = new HttpRequestMessage(
                HttpMethod.Post,
                new Uri(uri, "v1/chat/completions"));

            message.Headers.Authorization =
                new AuthenticationHeaderValue("Bearer", apiKey);

            message.Content = new StringContent(
                JsonSerializer.Serialize(new
                {
                    model,
                    messages = new[]
                    {
                        new
                        {
                            role = "system",
                            content = "Bạn là trợ lý logistics. " +
                                      "Hãy trả lời bằng tiếng Việt rõ ràng."
                        },
                        new
                        {
                            role = "user",
                            content = request.Prompt
                        }
                    },
                    temperature = 0.2
                }),
                Encoding.UTF8,
                "application/json");

            using var response = await client.SendAsync(
                message,
                HttpCompletionOption.ResponseHeadersRead,
                timeoutCts.Token);

            var responseBody = await response.Content.ReadAsStringAsync(
                timeoutCts.Token);

            stopwatch.Stop();

            if (!response.IsSuccessStatusCode)
            {
                return StatusCode(502, new
                {
                    error = "LLM provider trả về lỗi.",
                    providerStatus = (int)response.StatusCode,
                    latencyMs = stopwatch.ElapsedMilliseconds
                });
            }

            using var document = JsonDocument.Parse(responseBody);
            var root = document.RootElement;

            var answer = root
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            object? usage = null;

            if (root.TryGetProperty("usage", out var usageElement))
            {
                usage = usageElement.Clone();
            }

            return Ok(new
            {
                model,
                prompt = request.Prompt,
                answer,
                latencyMs = stopwatch.ElapsedMilliseconds,
                usage
            });
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            return StatusCode(504, new
            {
                error = "LLM API timeout sau 30 giây.",
                latencyMs = stopwatch.ElapsedMilliseconds
            });
        }
        catch (HttpRequestException)
        {
            return StatusCode(502, new
            {
                error = "Không thể kết nối tới LLM provider.",
                latencyMs = stopwatch.ElapsedMilliseconds
            });
        }
        catch (JsonException)
        {
            return StatusCode(502, new
            {
                error = "Phản hồi từ LLM provider không hợp lệ.",
                latencyMs = stopwatch.ElapsedMilliseconds
            });
        }
    }
}
