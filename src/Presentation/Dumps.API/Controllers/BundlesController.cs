using Dumps.Application.Command.Bundles;
using Dumps.Application.DTO.Request.Bundles;
using Dumps.Application.DTO.Response.Bundles;
using Dumps.Domain.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dumps.API.Controllers
{
    public class BundlesController : BaseController
    {
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        public async Task<ActionResult<CreateBundleResponse>> CreateBundle([FromForm] CreateBundleCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }
    }
}
