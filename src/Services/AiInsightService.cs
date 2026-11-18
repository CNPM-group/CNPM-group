using System.Globalization;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using SmartDrone.Domain;
using SmartDrone.Domain.Models;

namespace SmartDrone.Services;

// AI viết tóm tắt chuyến giao (KAN-62) và nhận xét vận hành cho quản lý.
// Dữ liệu luôn lấy từ DB rồi mới đưa cho LLM, LLM chỉ diễn đạt lại, không tự tra gì thêm.
// Không cấu hình LLM hoặc LLM lỗi → server tự ghép câu theo luật, nên demo không có API key vẫn chạy.
public sealed class AiInsightService(
    ILlmClient llm,
    OrderService orders,
    IDeliveryRepository deliveries,
    IDroneRepository drones,
    ReportService reports,
    IReportRepository reportRows)
{
    private const string SummaryPrompt =
        "Bạn viết tóm tắt một chuyến giao hàng bằng drone cho nhân viên và khách của SmartDroneDelivery. " +
        "Viết tiếng Việt, 3 đến 4 câu, văn xuôi, không dùng markdown, không gạch đầu dòng. " +
        "Chỉ dùng dữ liệu trong JSON người dùng gửi; trường nào không có thì không nhắc tới, không đoán. " +
        "Mọi mốc giờ trong dữ liệu đã là giờ Việt Nam. " +
        "Nếu giao trễ so với dự kiến hoặc thất bại thì nói rõ, kèm lý do nếu dữ liệu có.";

    private const string InsightPrompt =
        "Bạn là chuyên viên phân tích vận hành của SmartDroneDelivery (giao hàng bằng drone giữa các trạm). " +
        "Từ số liệu JSON người dùng gửi, viết 3 đến 5 nhận xét bằng tiếng Việt cho quản lý. " +
        "Mỗi nhận xét 1 dòng, bắt đầu bằng \"- \", gồm điều thấy được trong số liệu và 1 việc nên làm. " +
        "Chỉ dựa vào số liệu được gửi, không bịa con số. Ít dữ liệu thì nói thẳng là chưa đủ để kết luận.";

    // Giữ nguyên chữ tiếng Việt trong JSON gửi LLM (mặc định bị đổi thành \uXXXX, tốn token).
    private static readonly JsonSerializerOptions FactsJson = new()
    {
        Encoder = JavaScriptEncoder.Create(UnicodeRanges.All)
    };

    private static readonly Dictionary<string, string> StatusVi = new()
    {
        ["Pending"] = "chờ duyệt",
        ["Approved"] = "đã duyệt",
        ["Rejected"] = "bị từ chối",
        ["Scheduled"] = "đã xếp lịch bay",
        ["InTransit"] = "đang bay",
        ["ArrivedAtStation"] = "đã tới trạm nhận",
        ["Completed"] = "hoàn tất",
        ["Failed"] = "giao thất bại",
        ["Cancelled"] = "đã hủy"
    };

    // ---------- Tóm tắt chuyến giao ----------

    public async Task<AiSummary> SummarizeOrderAsync(Guid? customerScope, Guid orderId, CancellationToken ct = default)
    {
        var detail = await orders.GetDetailAsync(customerScope, orderId, ct);
        var history = await orders.GetHistoryAsync(customerScope, orderId, ct);
        var flight = await deliveries.GetLatestFlightAsync(orderId, ct);
        var attempts = await deliveries.CountFlightsAsync(orderId, ct);
        var drone = flight is null ? null : await drones.GetByIdAsync(flight.DroneId, ct);

        var facts = SummaryFacts.From(detail, history, flight, attempts, drone);
        var (text, source, note) = await WriteAsync(
            SummaryPrompt, facts, 400, () => RuleSummary(facts), ct);

        return new AiSummary(detail.Order.Id, detail.Order.TrackingCode, text.Trim(), source,
            source == AiSource.Llm ? llm.Model : null, DateTime.UtcNow, note);
    }

    internal static string RuleSummary(SummaryFacts f)
    {
        var parts = new List<string>
        {
            $"Đơn {f.TrackingCode} gửi kiện {f.WeightKg.ToString("0.##", CultureInfo.InvariantCulture)} kg" +
            (string.IsNullOrWhiteSpace(f.Description) ? "" : $" ({f.Description})") +
            $" từ {f.Origin} đến {f.Destination}, tạo lúc {f.CreatedAt}, hiện {StatusText(f.Status)}."
        };

        if (f.Status == nameof(OrderStatus.Rejected))
            parts.Add("Đơn bị điều phối từ chối" + Reason(f.LastNote) + ".");

        if (f.DepartedAt is not null)
        {
            var leg = $"Drone {f.DroneCode ?? "(không rõ mã)"} cất cánh lúc {f.DepartedAt}";
            if (f.ArrivedAt is not null) leg += $", tới trạm lúc {f.ArrivedAt}";
            if (f.EtaAt is not null) leg += $" (dự kiến {f.EtaAt})";
            if (f.LateMinutes is > 0) leg += $", trễ {f.LateMinutes} phút";
            else if (f.LateMinutes is < 0) leg += $", sớm {-f.LateMinutes} phút";
            parts.Add(leg + ".");
        }
        else if (f.EtaAt is not null)
        {
            parts.Add($"Dự kiến tới trạm nhận lúc {f.EtaAt}.");
        }

        if (f.Attempts > 1)
            parts.Add($"Đơn đã được giao {f.Attempts} lần.");

        if (f.FailureReason is not null && f.Status is nameof(OrderStatus.Failed) or nameof(OrderStatus.Cancelled))
            parts.Add($"Lý do thất bại: {f.FailureReason}.");

        if (f.CompletedAt is not null)
            parts.Add($"Khách xác nhận nhận hàng bằng OTP lúc {f.CompletedAt}" +
                      (f.TotalMinutes is null ? "." : $", tổng cộng {f.TotalMinutes} phút từ lúc tạo đơn."));

        return string.Join(" ", parts);
    }

    // ---------- Nhận xét vận hành ----------

    public async Task<AiInsights> InsightsAsync(DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var report = await reports.SummaryAsync(from, to, ct);
        var rows = await reportRows.ExportRowsAsync(report.FromUtc, report.ToUtc, ct);
        var facts = InsightFacts.Build(report, rows);

        var (text, source, note) = await WriteAsync(
            InsightPrompt, facts, 600, () => string.Join("\n", RuleInsights(facts).Select(p => "- " + p)), ct);

        var points = text.Split('\n')
            .Select(l => l.Trim().TrimStart('-', '•', '*').Trim())
            .Where(l => l.Length > 0)
            .ToList();

        return new AiInsights(report.FromUtc, report.ToUtc, points, source,
            source == AiSource.Llm ? llm.Model : null, DateTime.UtcNow, note);
    }

    internal static List<string> RuleInsights(InsightFacts f)
    {
        if (f.TotalOrders == 0)
            return new List<string> { $"Từ {f.From} đến {f.To} chưa có đơn nào, chưa đủ dữ liệu để nhận xét." };

        var points = new List<string>
        {
            $"Từ {f.From} đến {f.To} có {f.TotalOrders} đơn: {f.Completed} hoàn tất, {f.Failed} thất bại" +
            (f.Completed + f.Failed == 0 ? ", chưa đơn nào giao xong." : $", tỷ lệ thành công {f.SuccessRatePct.ToString("0.#", CultureInfo.InvariantCulture)} %.")
        };

        if (f.Completed + f.Failed >= 5 && f.SuccessRatePct < 90)
            points.Add("Tỷ lệ thành công dưới 90 %. " +
                       (f.TopFailureReasons.Count > 0
                           ? $"Lý do thất bại gặp nhiều nhất: \"{f.TopFailureReasons[0].Reason}\" ({f.TopFailureReasons[0].Count} lần), nên xem lại các đơn này trước."
                           : "Nên xem lại các đơn thất bại để tìm nguyên nhân chung."));
        else if (f.TopFailureReasons.Count > 0)
            points.Add($"Lý do thất bại gặp nhiều nhất: \"{f.TopFailureReasons[0].Reason}\" ({f.TopFailureReasons[0].Count} lần).");

        if (f.TopRoutes.Count > 0)
            points.Add($"Tuyến nhiều đơn nhất: {f.TopRoutes[0].Route} ({f.TopRoutes[0].Count} đơn), nên để sẵn drone rảnh ở trạm đi của tuyến này.");

        if (f.Pending > 0)
            points.Add($"Còn {f.Pending} đơn chờ duyệt, điều phối nên xử lý để khách không phải chờ.");

        if (f.AvgDeliveryMinutes is not null)
            points.Add($"Thời gian trung bình từ lúc tạo đơn tới lúc khách nhận: {f.AvgDeliveryMinutes.Value.ToString("0.#", CultureInfo.InvariantCulture)} phút.");

        if (f.PeakDay is not null)
            points.Add($"Ngày nhiều đơn nhất: {f.PeakDay} ({f.PeakDayCount} đơn).");

        return points.Take(5).ToList();
    }

    // ---------- Gọi LLM, lỗi thì dùng luật ----------

    private async Task<(string Text, string Source, string? Note)> WriteAsync(
        string systemPrompt, object facts, int maxTokens, Func<string> rule, CancellationToken ct)
    {
        if (!llm.IsConfigured)
            return (rule(), AiSource.Rule, "Chưa cấu hình LLM API, nội dung do server tự ghép từ dữ liệu.");

        try
        {
            using var doc = await llm.ChatAsync(new
            {
                model = llm.Model,
                messages = new object[]
                {
                    new { role = "system", content = systemPrompt },
                    new { role = "user", content = JsonSerializer.Serialize(facts, FactsJson) }
                },
                temperature = 0.2,
                max_tokens = maxTokens
            }, ct);

            var text = ReadContent(doc);
            if (!string.IsNullOrWhiteSpace(text))
                return (text, AiSource.Llm, null);

            return (rule(), AiSource.Rule, "LLM trả về nội dung rỗng, đã dùng bản tự ghép.");
        }
        catch (ExternalServiceException e)
        {
            return (rule(), AiSource.Rule, $"Không gọi được LLM ({e.Message}), đã dùng bản tự ghép.");
        }
    }

    private static string? ReadContent(JsonDocument doc)
    {
        if (doc.RootElement.TryGetProperty("choices", out var choices) &&
            choices.ValueKind == JsonValueKind.Array &&
            choices.GetArrayLength() > 0 &&
            choices[0].TryGetProperty("message", out var message) &&
            message.TryGetProperty("content", out var content) &&
            content.ValueKind == JsonValueKind.String)
        {
            return content.GetString();
        }

        return null;
    }

    internal static string StatusText(string status) =>
        StatusVi.TryGetValue(status, out var vi) ? vi : status;

    private static string Reason(string? note) =>
        string.IsNullOrWhiteSpace(note) ? "" : $", lý do: {note.Trim().TrimEnd('.')}";

    // Giờ Việt Nam (UTC+7, không đổi giờ theo mùa).
    internal static string? Vn(DateTime? utc) =>
        utc?.AddHours(7).ToString("HH:mm dd/MM", CultureInfo.InvariantCulture);
}

// Dữ liệu 1 đơn gửi cho LLM (giờ đã đổi sang giờ Việt Nam). Không chứa OTP hay id người dùng.
public sealed record SummaryFacts(
    string TrackingCode,
    string Status,
    string Origin,
    string Destination,
    decimal WeightKg,
    string? Description,
    string? CreatedAt,
    string? ApprovedAt,
    string? DepartedAt,
    string? ArrivedAt,
    string? EtaAt,
    int? LateMinutes,
    string? CompletedAt,
    int? TotalMinutes,
    string? DroneCode,
    int Attempts,
    string? FailureReason,
    string? LastNote,
    IReadOnlyList<string> Timeline)
{
    public static SummaryFacts From(
        OrderDetail detail,
        IReadOnlyList<OrderStatusHistory> history,
        DroneFlight? flight,
        int attempts,
        Drone? drone)
    {
        var o = detail.Order;
        var arrivedUtc = flight?.ActualArrivalAtUtc;
        int? late = arrivedUtc is not null && o.EtaUtc is not null
            ? (int)Math.Round((arrivedUtc.Value - o.EtaUtc.Value).TotalMinutes)
            : null;
        int? total = o.CompletedAtUtc is not null
            ? (int)Math.Round((o.CompletedAtUtc.Value - o.CreatedAtUtc).TotalMinutes)
            : null;

        return new SummaryFacts(
            o.TrackingCode,
            o.Status,
            detail.Origin.Name,
            detail.Destination.Name,
            detail.Package.WeightKg,
            detail.Package.Description,
            AiInsightService.Vn(o.CreatedAtUtc),
            AiInsightService.Vn(o.ApprovedAtUtc),
            AiInsightService.Vn(flight?.ActualDepartureAtUtc),
            AiInsightService.Vn(arrivedUtc),
            AiInsightService.Vn(o.EtaUtc),
            late,
            AiInsightService.Vn(o.CompletedAtUtc),
            total,
            drone?.DroneCode,
            attempts,
            o.FailureReason,
            history.Count == 0 ? null : history[history.Count - 1].Note,
            history.Select(h =>
                $"{AiInsightService.Vn(h.ChangedAtUtc)}: {AiInsightService.StatusText(h.ToStatus)}" +
                (string.IsNullOrWhiteSpace(h.Note) ? "" : $" – {h.Note}")).ToList());
    }
}

public sealed record ReasonCount(string Reason, int Count);

public sealed record RouteCount(string Route, int Count);

// Số liệu vận hành gửi cho LLM.
public sealed record InsightFacts(
    string From,
    string To,
    int TotalOrders,
    int Completed,
    int Failed,
    int Pending,
    double SuccessRatePct,
    double? AvgDeliveryMinutes,
    string? PeakDay,
    int PeakDayCount,
    IReadOnlyList<StatusCount> ByStatus,
    IReadOnlyList<ReasonCount> TopFailureReasons,
    IReadOnlyList<RouteCount> TopRoutes)
{
    public static InsightFacts Build(OrderReport r, IReadOnlyList<OrderExportRow> rows)
    {
        var peak = r.ByDay.OrderByDescending(d => d.Count).FirstOrDefault();

        return new InsightFacts(
            r.FromUtc.AddHours(7).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
            r.ToUtc.AddHours(7).ToString("dd/MM/yyyy", CultureInfo.InvariantCulture),
            r.TotalOrders,
            r.CompletedOrders,
            r.FailedOrders,
            r.ByStatus.Where(s => s.Status == nameof(OrderStatus.Pending)).Sum(s => s.Count),
            r.SuccessRatePct,
            r.AvgDeliveryMinutes,
            peak?.Day.ToString("dd/MM", CultureInfo.InvariantCulture),
            peak?.Count ?? 0,
            r.ByStatus,
            rows.Where(x => !string.IsNullOrWhiteSpace(x.FailureReason))
                .GroupBy(x => x.FailureReason!.Trim())
                .Select(g => new ReasonCount(g.Key, g.Count()))
                .OrderByDescending(x => x.Count)
                .Take(3)
                .ToList(),
            rows.GroupBy(x => $"{x.OriginStation} → {x.DestinationStation}")
                .Select(g => new RouteCount(g.Key, g.Count()))
                .OrderByDescending(x => x.Count)
                .Take(3)
                .ToList());
    }
}
