        bool includeTools,
        CancellationToken cancellationToken)
    {
        var payload = new JsonObject
        {
            ["model"] = model,
            ["messages"] = JsonNode.Parse(messages.ToJsonString()),
            ["temperature"] = 0.2
        };

        if (includeTools)
        {
            payload["tools"] = JsonSerializer.SerializeToNode(ChatTools);
            payload["tool_choice"] = "auto";
        }

        using var message = new HttpRequestMessage(
            HttpMethod.Post, new Uri(baseUri, "v1/chat/completions"));
        message.Headers.Authorization =
            new AuthenticationHeaderValue("Bearer", apiKey);
        message.Content = new StringContent(
            payload.ToJsonString(), Encoding.UTF8, "application/json");

        using var response = await client.SendAsync(message, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new HttpRequestException(
                $"LLM provider trả về HTTP {(int)response.StatusCode}.");

        return JsonDocument.Parse(body);
    }

    private async Task<string> ExecuteBusinessFunction(
        string functionName,
        JsonElement arguments,
        HttpClient client,
        string authorization,
        CancellationToken cancellationToken)
    {
        var idField = functionName == "get_station_status"
            ? "stationId"
            : "orderId";

        if (!arguments.TryGetProperty(idField, out var idElement) ||
            idElement.ValueKind != JsonValueKind.String ||
            !Guid.TryParse(idElement.GetString(), out var id))
        {
            return JsonSerializer.Serialize(new
            {
                error = $"Tham số {idField} không hợp lệ; cần UUID."
            });
        }

        var isStation = functionName == "get_station_status";
        var path = isStation
            ? $"/api/v1/stations/{id}"
            : $"/api/v1/orders/{id}";

        // Uses the current API host. If API routes are hosted elsewhere,
        // replace this with a configured internal API base URL.
        var apiUrl = $"{Request.Scheme}://{Request.Host}{path}";
        using var apiRequest = new HttpRequestMessage(HttpMethod.Get, apiUrl);
        apiRequest.Headers.TryAddWithoutValidation("Authorization", authorization);

        using var response = await client.SendAsync(apiRequest, cancellationToken);
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            return JsonSerializer.Serialize(new
            {
                error = response.StatusCode switch
                {
                    System.Net.HttpStatusCode.NotFound => "Không tìm thấy dữ liệu.",
                    System.Net.HttpStatusCode.Unauthorized => "Yêu cầu xác thực lại.",
                    System.Net.HttpStatusCode.Forbidden => "Bạn không có quyền xem dữ liệu này.",
                    _ => "Không thể lấy dữ liệu từ API nghiệp vụ."
                },
                statusCode = (int)response.StatusCode
            });
        }

        using var document = JsonDocument.Parse(body);
        var root = document.RootElement;

        if (functionName != "get_delivery_eta")
            return root.GetRawText();

        if (!root.TryGetProperty("eta", out var eta) ||
            eta.ValueKind == JsonValueKind.Null ||
            eta.ValueKind == JsonValueKind.Undefined)
        {
            return JsonSerializer.Serialize(new
            {
                message = "ETA hiện chưa có dữ liệu."
            });
        }

        return JsonSerializer.Serialize(new
        {
            eta = eta.Clone(),
            status = root.TryGetProperty("status", out var status)
                ? status.Clone()
                : (JsonElement?)null
        });
    }
}
