using System.Net;
using Dumps.Application.Command.Promo;
using Dumps.Domain.Common.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dumps.API.Controllers
{
    [Authorize(Roles = SD.Role_Admin)]
    public class PromoCodeController : BaseController
    {
        /// <summary>
        /// Create a new promo code (Admin only).
        /// </summary>
        /// <param name="command">Promo code details.</param>
        /// <returns>APIResponse with the created promo code ID.</returns>
        [HttpPost("create")]
        [ProducesResponseType(typeof(APIResponse<Guid>), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.Conflict)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreatePromoCode([FromBody] CreatePromoCodeCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }
    }
}
