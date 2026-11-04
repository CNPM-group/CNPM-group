using System.Diagnostics;
using System.Text.Json;
using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Services;

public sealed class AiService(ILlmClient llm, OrderService orders, StationService stations)
{
    private const int MaxToolRounds = 3;

    private const string AssistantPrompt =
        "Bạn là trợ lý logistics SmartDroneDelivery. Trả lời bằng tiếng Việt rõ ràng. " +
        "Không tự bịa thông tin đơn hàng hoặc thời gian giao.";

    private const string ChatbotPrompt =
        "Bạn là chatbot hỗ trợ khách hàng của SmartDroneDelivery. Trả lời bằng tiếng Việt, ngắn gọn và dễ hiểu. " +
        "Khi cần tra đơn hàng, hãy dùng get_order_status. Khi cần tra trạm theo ID, hãy dùng get_station_status. " +
        "Khi cần ETA, hãy dùng get_delivery_eta. Chỉ sử dụng dữ liệu trả về từ công cụ. " +
        "Không tự bịa trạng thái, thông tin trạm hoặc ETA. Nếu thiếu ID cần thiết, hãy hỏi lại người dùng. " +
        "Không tiết lộ thông tin của đơn hàng mà người dùng không có quyền truy cập.";

    private static readonly object[] Tools =
    {
        Tool("get_order_status", "Tra cứu thông tin và trạng thái đơn hàng theo ID.", "orderId", "ID đơn hàng dạng UUID."),
        Tool("get_station_status", "Tra cứu thông tin trạm theo ID.", "stationId", "ID trạm dạng UUID."),
        Tool("get_delivery_eta", "Tra cứu thời gian giao hàng dự kiến ETA của đơn hàng.", "orderId", "ID đơn hàng dạng UUID.")
    };

    public async Task<AiAnswer> AskAsync(string? prompt, CancellationToken ct = default)
    {
        EnsureText(prompt, "Prompt");
        var watch = Stopwatch.StartNew();

        using var doc = await llm.ChatAsync(new
        {
            model = llm.Model,
            messages = new object[]
            {
                new { role = "system", content = AssistantPrompt },
                new { role = "user", content = prompt }
            },
            temperature = 0.2
        }, ct);

        var message = ReadMessage(doc);
        object? usage = doc.RootElement.TryGetProperty("usage", out var u) ? (object)u.Clone() : null;

        return new AiAnswer(llm.Model, ReadContent(message), watch.ElapsedMilliseconds, usage);
    }

    public async Task<AiAnswer> ChatAsync(string? text, Guid? customerScope, CancellationToken ct = default)
    {
        EnsureText(text, "Tin nhắn");
        var watch = Stopwatch.StartNew();

        var messages = new List<object>
        {
            new { role = "system", content = ChatbotPrompt },
            new { role = "user", content = text }
        };

        for (var round = 0; round < MaxToolRounds; round++)
        {
            using var doc = await llm.ChatAsync(new
            {
                model = llm.Model,
                messages,
                tools = Tools,
                tool_choice = "auto",
                temperature = 0.2
            }, ct);

            var message = ReadMessage(doc);

            if (!message.TryGetProperty("tool_calls", out var calls) ||
                calls.ValueKind != JsonValueKind.Array ||
                calls.GetArrayLength() == 0)
            {
                return new AiAnswer(llm.Model, ReadContent(message), watch.ElapsedMilliseconds);
            }

            messages.Add(message.Clone());

            foreach (var call in calls.EnumerateArray())
            {
                var function = call.GetProperty("function");
                var result = await RunToolAsync(
                    function.GetProperty("name").GetString() ?? "",
                    function.GetProperty("arguments").GetString(),
                    customerScope,
                    ct);

                messages.Add(new
                {
                    role = "tool",
                    tool_call_id = call.GetProperty("id").GetString(),
                    content = JsonSerializer.Serialize(result)
                });
            }
        }

        throw new ExternalServiceException("Chatbot đã vượt quá số vòng gọi công cụ cho phép.");
    }

    private async Task<object> RunToolAsync(string name, string? argumentsJson, Guid? customerScope, CancellationToken ct)
    {
        var idKey = name switch
        {
            "get_order_status" or "get_delivery_eta" => "orderId",
            "get_station_status" => "stationId",
            _ => null
        };

        if (idKey is null)
            return new { error = "Không hỗ trợ công cụ này." };

        Guid id;
        try
        {
            using var args = JsonDocument.Parse(argumentsJson ?? "{}");
            if (!args.RootElement.TryGetProperty(idKey, out var value) ||
                value.ValueKind != JsonValueKind.String ||
                !Guid.TryParse(value.GetString(), out id))
            {
                return new { error = $"{idKey} phải là UUID hợp lệ." };
            }
        }
        catch (JsonException)
        {
            return new { error = "Tham số công cụ không hợp lệ." };
        }

        try
        {
            switch (name)
            {
                case "get_station_status":
                    var s = await stations.GetAsync(id, ct);
                    return new { s.Id, s.Name, s.Latitude, s.Longitude, Status = s.Status.ToString() };

                case "get_delivery_eta":
                    var e = await orders.GetAsync(customerScope, id, ct);
                    return new { orderId = e.Id, eta = e.EtaUtc, e.Status };

                default:
                    var o = await orders.GetAsync(customerScope, id, ct);
                    return new { o.Id, o.TrackingCode, o.Status, o.OriginStationId, o.DestinationStationId, eta = o.EtaUtc, o.CreatedAtUtc };
            }
        }
        catch (NotFoundException)
        {
            return new { error = "Không tìm thấy dữ liệu hoặc người dùng không có quyền truy cập." };
        }
    }

    private static void EnsureText(string? text, string field)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ValidationException($"{field} không được để trống.");

        if (text.Length > ApiConstants.MaxPromptLength)
            throw new ValidationException($"{field} tối đa {ApiConstants.MaxPromptLength} ký tự.");
    }

    private static JsonElement ReadMessage(JsonDocument doc)
    {
        if (doc.RootElement.TryGetProperty("choices", out var choices) &&
            choices.ValueKind == JsonValueKind.Array &&
            choices.GetArrayLength() > 0 &&
            choices[0].TryGetProperty("message", out var message))
        {
            return message.Clone();
        }

        throw new ExternalServiceException("Phản hồi từ LLM provider không hợp lệ.");
    }

    private static string? ReadContent(JsonElement message) =>
        message.TryGetProperty("content", out var c) && c.ValueKind == JsonValueKind.String ? c.GetString() : null;

    private static object Tool(string name, string description, string idKey, string idDescription) => new
    {
        type = "function",
        function = new
        {
            name,
            description,
            parameters = new
            {
                type = "object",
                properties = new Dictionary<string, object>
                {
                    [idKey] = new { type = "string", description = idDescription }
                },
                required = new[] { idKey },
                additionalProperties = false
            }
        }
    };
}
