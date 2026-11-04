using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using SmartDrone.Api.Schemas;
using SmartDrone.Domain;
using SmartDrone.Services;

namespace SmartDrone.Api.Controllers;

[ApiController]
[Route(ApiConstants.Prefix + "/orders")]
[Authorize]
public class OrdersController(OrderService orders, EtaService eta) : ControllerBase
{
    [HttpGet("eta")]
    public async Task<ActionResult<EtaResponse>> EstimateEta(
        [FromQuery] Guid originStationId,
        [FromQuery] Guid destinationStationId,
        [FromQuery] decimal weightKg,
        CancellationToken ct) =>
        Ok(EtaResponse.From(await eta.EstimateAsync(originStationId, destinationStationId, weightKg, ct)));

    [HttpPost]
    [Authorize(Roles = Roles.Customer)]
    public async Task<ActionResult<OrderResponse>> Create(OrderRequest request, CancellationToken ct)
    {
        var order = await orders.CreateAsync(User.UserId(), request.ToOrder(), request.ToPackage(), ct);
        return CreatedAtAction(nameof(GetById), new { id = order.Id }, OrderResponse.From(order));
    }

    [HttpGet]
    public async Task<ActionResult<OrderPageResponse>> GetPage(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = ApiConstants.DefaultPageSize,
        [FromQuery] string? status = null,
        CancellationToken ct = default) =>
        Ok(OrderPageResponse.From(await orders.PageAsync(User.CustomerScope(), status, page, pageSize, ct)));

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<OrderResponse>> GetById(Guid id, CancellationToken ct) =>
        Ok(OrderResponse.From(await orders.GetAsync(User.CustomerScope(), id, ct)));

    [HttpPut("{id:guid}")]
    [Authorize(Roles = Roles.Customer)]
    public async Task<ActionResult<OrderResponse>> Update(Guid id, OrderRequest request, CancellationToken ct) =>
        Ok(OrderResponse.From(await orders.UpdateAsync(User.UserId(), id, request.ToOrder(), request.ToPackage(), ct)));

    [HttpPost("{id:guid}/cancel")]
    [Authorize(Roles = Roles.Customer)]
    public async Task<ActionResult<OrderResponse>> Cancel(Guid id, CancellationToken ct) =>
        Ok(OrderResponse.From(await orders.CancelAsync(User.UserId(), id, ct)));

    [HttpPost("{id:guid}/approve")]
    [Authorize(Roles = Roles.DispatchStaff)]
    public async Task<ActionResult<OrderResponse>> Approve(
        Guid id,
        [FromBody(EmptyBodyBehavior = EmptyBodyBehavior.Allow)] NoteRequest? request,
        CancellationToken ct) =>
        Ok(OrderResponse.From(await orders.ApproveAsync(User.UserId(), id, request?.Note, ct)));

    [HttpPost("{id:guid}/reject")]
    [Authorize(Roles = Roles.DispatchStaff)]
    public async Task<ActionResult<OrderResponse>> Reject(Guid id, RejectRequest request, CancellationToken ct) =>
        Ok(OrderResponse.From(await orders.RejectAsync(User.UserId(), id, request.Reason, ct)));

    [HttpGet("{id:guid}/history")]
    public async Task<ActionResult<IReadOnlyList<OrderStatusHistoryResponse>>> GetHistory(Guid id, CancellationToken ct) =>
        Ok((await orders.GetHistoryAsync(User.CustomerScope(), id, ct)).Select(OrderStatusHistoryResponse.From).ToList());
}
