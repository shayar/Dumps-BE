using Dumps.Application.Command.RegisterUser;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Dumps.API.Controllers
{
    public class AccountController : BaseController
    {
      

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterUserCommand command)
        {
            var result = await Mediator.Send(command);

            if (result.Succeeded)
            {
                return Ok("User registered successfully.");
            }

            return BadRequest(result.Errors.Select(e => e.Description));
        }
    }
}
