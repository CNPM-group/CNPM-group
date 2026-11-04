using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDrone.Api.Schemas;
using SmartDrone.Domain;
using SmartDrone.Services;

namespace SmartDrone.Api.Controllers;

[ApiController]
[Route(ApiConstants.Prefix + "/users")]
[Authorize(Roles = Roles.Admin)]
public class UsersController(UserService users) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll() =>
        Ok((await users.ListAsync()).Select(UserResponse.From).ToList());

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(CreateUserRequest request) =>
        Ok(UserResponse.From(await users.CreateAsync(request.Email, request.Password, request.FullName, request.Role)));

    [HttpPut("{id:guid}/roles")]
    public async Task<ActionResult<UserResponse>> SetRoles(Guid id, SetRolesRequest request) =>
        Ok(UserResponse.From(await users.SetRolesAsync(User.UserId(), id, request.Roles)));

    [HttpPut("{id:guid}/active")]
    public async Task<ActionResult<UserResponse>> SetActive(Guid id, SetActiveRequest request) =>
        Ok(UserResponse.From(await users.SetActiveAsync(User.UserId(), id, request.IsActive)));
}
