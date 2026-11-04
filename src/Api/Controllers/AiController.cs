using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDrone.Api.Schemas;
using SmartDrone.Domain;
using SmartDrone.Services;

namespace SmartDrone.Api.Controllers;

[ApiController]
[Route(ApiConstants.Prefix + "/ai")]
[Authorize]
public class AiController(AiService ai) : ControllerBase
{
    [HttpPost("feasibility-test")]
    public async Task<ActionResult<PromptResponse>> FeasibilityTest(PromptRequest request, CancellationToken ct) =>
        Ok(PromptResponse.From(request.Prompt, await ai.AskAsync(request.Prompt, ct)));

    [HttpPost("chat")]
    public async Task<ActionResult<ChatResponse>> Chat(ChatRequest request, CancellationToken ct) =>
        Ok(ChatResponse.From(await ai.ChatAsync(request.Message, User.CustomerScope(), ct)));
}
