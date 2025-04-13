﻿using Dumps.Application.Command.Account;
using Dumps.Application.Command.RegisterUser;
using Dumps.Application.DTO.Response.Auth;
using Dumps.Application.Query;
using Microsoft.AspNetCore.Mvc;

namespace Dumps.API.Controllers
{
    public class AccountController : BaseController
    {
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterUserCommand command)
        {
            var result = await Mediator.Send(command);

            if (result.Success)
            {
                return Ok(new {result.Message, result.Data});
            }

            return BadRequest(result.Message);
        }

        [HttpPost("Login")]
        public async Task<ActionResult<APIResponse<LoginResponse>>> LoginAsync([FromBody] LoginQuery query)
        {
            return await Mediator.Send(query);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
