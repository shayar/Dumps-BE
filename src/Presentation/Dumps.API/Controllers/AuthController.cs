using Dumps.Application.DTO.Response.Auth;
using Dumps.Application.Query;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dumps.API.Controllers;

public class AuthController: BaseController
{
    [HttpPost("Login")]
    public async Task<ActionResult<LoginResponse>> LoginAsync(Login.LoginQuery query)
    {
        return await Mediator.Send(query);
    }
}
