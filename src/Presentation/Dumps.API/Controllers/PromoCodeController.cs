using System.Net;
using Dumps.Application.Command.Promo;
using Dumps.Application.DTO.Request.PromoCode;
using Dumps.Application.DTO.Response.PromoCode;
using Dumps.Application.Query.PromoCode;
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

        /// <summary>
        /// Delete an existing promo code (Admin only).
        /// </summary>
        /// <param name="promoCodeId">The promo code ID.</param>
        /// <returns>APIResponse confirming deletion.</returns>
        [HttpDelete("delete/{promoCodeId}")]
        [ProducesResponseType(typeof(APIResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeletePromoCode(Guid promoCodeId)
        {
            var result = await Mediator.Send(new DeletePromoCodeCommand { PromoCodeId = promoCodeId });
            return Ok(result);
        }

        /// <summary>
        /// Get all promo codes with filtering options (Admin only).
        /// By default, only active promo codes are shown.
        /// </summary>
        /// <param name="showAll">Pass true to show all, false to show only inactive, null to show active.</param>
        /// <returns>APIResponse containing the list of promo codes.</returns>
        [HttpGet("get-all")]
        [ProducesResponseType(typeof(APIResponse<List<PromoCodeResponse>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.Forbidden)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetPromoCodes([FromQuery] bool? showAll)
        {
            var result = await Mediator.Send(new GetPromoCodesQuery { ShowAll = showAll });
            return Ok(result);
        }

        /// <summary>
        /// Update an existing promo code (Admin only).
        /// </summary>
        /// <param name="id">The ID of the promo code to update.</param>
        /// <param name="command">The updated promo code details.</param>
        /// <returns>APIResponse with the updated promo code details.</returns>
        [HttpPut("update/{id}")]
        [ProducesResponseType(typeof(APIResponse<PromoCodeResponse>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> UpdatePromoCode(Guid id, [FromBody] UpdatePromoCodeCommand command)
        {
            command.Id = id; // Ensure ID is passed in the request
            var result = await Mediator.Send(command);
            return Ok(result);
        }

    }
}
