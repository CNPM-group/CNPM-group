using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace SmartDrone.Api;

public static class Responses
{
    public static IMvcBuilder AddApiResponses(this IMvcBuilder mvc)
    {
        mvc.AddJsonOptions(o => o.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

        mvc.ConfigureApiBehaviorOptions(o => o.InvalidModelStateResponseFactory = ctx =>
            new BadRequestObjectResult(new ValidationProblemDetails(ctx.ModelState)
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Dữ liệu không hợp lệ",
                Instance = ctx.HttpContext.Request.Path
            }));

        return mvc;
    }
}
