using Dumps.Application.Command;
using Microsoft.AspNetCore.Mvc;

namespace Dumps.API.Controllers
{
    public class ContactUsController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> Post([FromForm] CreateContactUsCommand command)
        {
            var id = await Mediator.Send(command);
            return CreatedAtAction(nameof(Post), new { id });
        }
    }
}
