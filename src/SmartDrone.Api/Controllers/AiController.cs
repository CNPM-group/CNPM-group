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

    public record ChatRequest(string? Message);

    private static readonly object[] ChatTools =
    {
        new
        {
            type = "function",
            function = new
            {
                name = "get_order_status",
                description =
                    "Tra cứu thông tin và trạng thái đơn hàng theo ID.",
                parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        orderId = new
                        {
                            type = "string",
                            description = "ID đơn hàng dạng UUID."
                        }
                    },
                    required = new[] { "orderId" },
                    additionalProperties = false
                }
            }
        },
        new
        {
            type = "function",
            function = new
            {
                name = "get_station_status",
                description =
                    "Tra cứu thông tin trạm theo ID.",
                parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        stationId = new
                        {
                            type = "string",
                            description = "ID trạm dạng UUID."
                        }
                    },
                    required = new[] { "stationId" },
                    additionalProperties = false
                }
            }
        },
        new
        {
            type = "function",
            function = new
            {
                name = "get_delivery_eta",
                description =
                    "Tra cứu thời gian giao hàng dự kiến ETA của đơn hàng.",
                parameters = new
                {
                    type = "object",
                    properties = new
                    {
                        orderId = new
                        {
                            type = "string",
                            description = "ID đơn hàng dạng UUID."
                        }
                    },
                    required = new[] { "orderId" },
                    additionalProperties = false
                }
            }
        }
    };

    [HttpPost("feasibility-test")]
    public async Task<IActionResult> FeasibilityTest(
        [FromBody] FeasibilityRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Prompt))
        {
            return BadRequest(new
            {
                error = "Prompt không được để trống."
            });
        }

        if (request.Prompt.Length > 2000)
        {
            return BadRequest(new
            {
                error = "Prompt tối đa 2000 ký tự."
            });
        }

        if (!TryGetLlmConfiguration(
                out var apiKey,
                out var baseUri,
                out var model))
        {
            return StatusCode(500, new
            {
                error = "Chưa cấu hình đầy đủ LLM API."
            });
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var payload = new
            {
                model,
                messages = new object[]
                {
                    new
                    {
                        role = "system",
                        content =
                            "Bạn là trợ lý logistics SmartDroneDelivery. " +
                            "Trả lời bằng tiếng Việt rõ ràng. " +
                            "Không tự bịa thông tin đơn hàng hoặc thời gian giao."
                    },
                    new
                    {
                        role = "user",
                        content = request.Prompt
                    }
                },
                temperature = 0.2
            };

            using var document = await SendLlmRequest(
                payload,
                apiKey,
                baseUri,
                cancellationToken);

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

            stopwatch.Stop();

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
                error = "LLM API timeout."
            });
        }
        catch (HttpRequestException)
        {
            return StatusCode(502, new
            {
                error = "Không thể kết nối tới LLM provider."
            });
        }
        catch (JsonException)
        {
            return StatusCode(502, new
            {
                error = "Phản hồi từ LLM provider không hợp lệ."
            });
        }
    }

    [HttpPost("chat")]
    public async Task<IActionResult> Chat(
        [FromBody] ChatRequest request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Message))
        {
            return BadRequest(new
            {
                error = "Tin nhắn không được để trống."
            });
        }

        if (request.Message.Length > 2000)
        {
            return BadRequest(new
            {
                error = "Tin nhắn tối đa 2000 ký tự."
            });
        }

        if (!TryGetLlmConfiguration(
                out var apiKey,
                out var baseUri,
                out var model))
        {
            return StatusCode(500, new
            {
                error = "Chưa cấu hình đầy đủ LLM API."
            });
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var messages = new List<object>
            {
                new
                {
                    role = "system",
                    content =
                        "Bạn là chatbot hỗ trợ khách hàng của " +
                        "SmartDroneDelivery. Trả lời bằng tiếng Việt, " +
                        "ngắn gọn và dễ hiểu. " +
                        "Khi cần tra đơn hàng, hãy dùng get_order_status. " +
                        "Khi cần tra trạm theo ID, hãy dùng get_station_status. " +
                        "Khi cần ETA, hãy dùng get_delivery_eta. " +
                        "Chỉ sử dụng dữ liệu trả về từ công cụ. " +
                        "Không tự bịa trạng thái, thông tin trạm hoặc ETA. " +
                        "Nếu thiếu ID cần thiết, hãy hỏi lại người dùng. " +
                        "Không tiết lộ thông tin của đơn hàng mà người dùng " +
                        "không có quyền truy cập."
                },
                new
                {
                    role = "user",
                    content = request.Message
                }
            };

            // Cho phép tối đa 3 vòng gọi công cụ.
            for (var round = 0; round < 3; round++)
            {
                var payload = new
                {
                    model,
                    messages,
                    tools = ChatTools,
                    tool_choice = "auto",
                    temperature = 0.2
                };

                using var document = await SendLlmRequest(
                    payload,
                    apiKey,
                    baseUri,
                    cancellationToken);

                var message = document.RootElement
                    .GetProperty("choices")[0]
                    .GetProperty("message");

                var toolCalls = message.TryGetProperty(
                    "tool_calls",
                    out var calls)
                    ? calls
                    : default;

                if (toolCalls.ValueKind != JsonValueKind.Array ||
                    toolCalls.GetArrayLength() == 0)
                {
                    var answer = message.TryGetProperty(
                        "content",
                        out var contentElement)
                        && contentElement.ValueKind == JsonValueKind.String
                        ? contentElement.GetString()
                        : null;

                    stopwatch.Stop();

                    return Ok(new
                    {
                        model,
                        answer,
                        latencyMs = stopwatch.ElapsedMilliseconds
                    });
                }

                // Gửi lại message của assistant, bao gồm tool_calls.
                messages.Add(JsonSerializer.Deserialize<object>(
                    message.GetRawText())!);

                foreach (var toolCall in toolCalls.EnumerateArray())
                {
                    var toolCallId = toolCall
                        .GetProperty("id")
                        .GetString();

                    var function = toolCall.GetProperty("function");
                    var functionName = function
                        .GetProperty("name")
                        .GetString();

                    var argumentsText = function
                        .GetProperty("arguments")
                        .GetString();

                    object toolResult;

                    try
                    {
                        using var arguments =
                            JsonDocument.Parse(argumentsText ?? "{}");

                        toolResult = await ExecuteBusinessFunction(
                            functionName ?? "",
                            arguments.RootElement,
                            cancellationToken);
                    }
                    catch (JsonException)
                    {
                        toolResult = new
                        {
                            error = "Tham số công cụ không hợp lệ."
                        };
                    }
                    catch (HttpRequestException)
                    {
                        toolResult = new
                        {
                            error = "Không thể truy cập API nghiệp vụ."
                        };
                    }

                    messages.Add(new
                    {
                        role = "tool",
                        tool_call_id = toolCallId,
                        content = JsonSerializer.Serialize(toolResult)
                    });
                }
            }

            return StatusCode(502, new
            {
                error = "Chatbot đã vượt quá số vòng gọi công cụ cho phép."
            });
        }
        catch (OperationCanceledException)
            when (!cancellationToken.IsCancellationRequested)
        {
            return StatusCode(504, new
            {
                error = "LLM API timeout."
            });
        }
        catch (HttpRequestException)
        {
            return StatusCode(502, new
            {
                error = "Không thể kết nối tới LLM provider hoặc API nghiệp vụ."
            });
        }
        catch (JsonException)
        {
            return StatusCode(502, new
            {
                error = "Phản hồi JSON không hợp lệ."
            });
        }
    }

    private bool TryGetLlmConfiguration(
        out string apiKey,
        out Uri baseUri,
        out string model)
    {
        apiKey = _configuration["Llm:ApiKey"] ?? "";
        model = _configuration["Llm:Model"] ?? "";

        var baseUrl = _configuration["Llm:BaseUrl"];

        baseUri = null!;

        if (string.IsNullOrWhiteSpace(apiKey) ||
            string.IsNullOrWhiteSpace(model) ||
            string.IsNullOrWhiteSpace(baseUrl))
        {
            return false;
        }

        if (!Uri.TryCreate(
                baseUrl,
                UriKind.Absolute,
                out var parsedBaseUri) ||
            parsedBaseUri.Scheme != Uri.UriSchemeHttps)
        {
            return false;
        }

        baseUri = parsedBaseUri;
        return true;
    }

    private async Task<JsonDocument> SendLlmRequest(
        object payload,
        string apiKey,
        Uri baseUri,
        CancellationToken cancellationToken)
    {
        using var timeoutCts =
            CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken);

        timeoutCts.CancelAfter(TimeSpan.FromSeconds(30));

        using var client = _httpClientFactory.CreateClient();
        client.Timeout = Timeout.InfiniteTimeSpan;

        using var message = new HttpRequestMessage(
            HttpMethod.Post,
            new Uri(baseUri, "v1/chat/completions"));

        message.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);

        message.Content = new StringContent(
            JsonSerializer.Serialize(payload),
            Encoding.UTF8,
            "application/json");

        using var response = await client.SendAsync(
            message,
            HttpCompletionOption.ResponseHeadersRead,
            timeoutCts.Token);

        if (!response.IsSuccessStatusCode)
        {
            throw new HttpRequestException(
                $"LLM provider returned HTTP {(int)response.StatusCode}.");
        }

        var responseBody = await response.Content.ReadAsStringAsync(
            timeoutCts.Token);

        return JsonDocument.Parse(responseBody);
    }

    private async Task<object> ExecuteBusinessFunction(
        string functionName,
        JsonElement arguments,
        CancellationToken cancellationToken)
    {
        string endpoint;
        string idProperty;

        switch (functionName)
        {
            case "get_order_status":
                endpoint = "/api/v1/orders/";
                idProperty = "orderId";
                break;

            case "get_delivery_eta":
                endpoint = "/api/v1/orders/";
                idProperty = "orderId";
                break;

            case "get_station_status":
                endpoint = "/api/v1/stations/";
                idProperty = "stationId";
                break;

            default:
                return new
                {
                    error = "Không hỗ trợ công cụ này."
                };
        }

        if (!arguments.TryGetProperty(idProperty, out var idElement) ||
            idElement.ValueKind != JsonValueKind.String)
        {
            return new
            {
                error = $"Thiếu tham số {idProperty}."
            };
        }

        var id = idElement.GetString();

        if (!Guid.TryParse(id, out var parsedId))
        {
            return new
            {
                error = $"{idProperty} phải là UUID hợp lệ."
            };
        }

        // Gọi lại API nghiệp vụ nội bộ trên cùng host.
        // API nghiệp vụ phải tự kiểm tra quyền truy cập dữ liệu.
        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        var url = $"{baseUrl}{endpoint}{parsedId:D}";

        using var client = _httpClientFactory.CreateClient();

        using var message = new HttpRequestMessage(
            HttpMethod.Get,
            url);

        if (Request.Headers.TryGetValue(
                "Authorization",
                out var authorization))
        {
            message.Headers.TryAddWithoutValidation(
                "Authorization",
                authorization.ToString());
        }

        using var response = await client.SendAsync(
            message,
            cancellationToken);

        var body = await response.Content.ReadAsStringAsync(
            cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return new
            {
                error = "Không thể tra cứu dữ liệu hoặc người dùng không có quyền truy cập.",
                statusCode = (int)response.StatusCode
            };
        }

        using var document = JsonDocument.Parse(body);
        var result = document.RootElement.Clone();

        if (functionName == "get_delivery_eta")
        {
            if (result.ValueKind == JsonValueKind.Object &&
                result.TryGetProperty("eta", out var eta))
            {
                return new
                {
                    orderId = parsedId,
                    eta = eta.Clone(),
                    source = "order_api"
                };
            }

            return new
            {
                orderId = parsedId,
                eta = (object?)null,
                message = "API đơn hàng chưa trả về trường eta."
            };
        }

        return result;
    }
}
