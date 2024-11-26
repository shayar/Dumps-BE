using System.Net;
using Dumps.Application.Command.Bundles;
using Dumps.Application.DTO.Request.Bundles;
using Dumps.Application.DTO.Response.Bundles;
using Dumps.Application.Query.Bundles;
using Dumps.Domain.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dumps.API.Controllers
{
    public class BundlesController : BaseController
    {
        /// <summary>
        /// Create a new bundle (Admin only)
        /// </summary>
        /// <param name="command">The bundle creation command</param>
        /// <returns>APIResponse containing the created bundle's ID</returns>
        [HttpPost]
        [Authorize(Roles = SD.Role_Admin)]
        [ProducesResponseType(typeof(APIResponse<CreateBundleResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.BadRequest)]
        public async Task<ActionResult<CreateBundleResponse>> CreateBundle([FromForm] CreateBundleCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);
        }

        /// <summary>
        /// Get all bundles
        /// </summary>
        /// <returns>APIResponse containing a list of all bundles</returns>
        [HttpGet]
        [ProducesResponseType(typeof(APIResponse<IList<CreateBundleResponse>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.InternalServerError)]
        [Authorize]
        public async Task<IActionResult> GetAllBundles([FromQuery] string? sort,
            [FromQuery] int page = 1, [FromQuery] int pageSize  = 10, [FromQuery] string? search = null)
        {
            var result = await Mediator.Send(new GetAllBundles.GetAllBundlesQuery(sort, page, pageSize, search));
            return Ok(result);
        }

        /// <summary>
        /// Get a specific bundle by Id
        /// </summary>
        /// <param name="id">The bundle Id</param>
        /// <returns>APIResponse containing the bundle details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(APIResponse<CreateBundleResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetBundleById(Guid id)
        {
            var result = await Mediator.Send(new GetBundleById.GetBundleByIdQuery { Id = id });
            return Ok(result);
        }

        /// <summary>
        /// Update an existing bundle (Admin only)
        /// </summary>
        /// <param name="id">The bundle Id</param>
        /// <param name="request">The bundle update request</param>
        /// <returns>APIResponse containing the updated bundle's ID</returns>
        [HttpPatch("{id}")]
        // [Authorize(Roles = SD.Role_Admin)]
        [ProducesResponseType(typeof(APIResponse<Guid>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdateBundle(Guid id, [FromBody] UpdateBundleCommand command)
        {
            command.UpdatedBy = User.Identity?.Name ?? SD.Role_Admin;

            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
