using System.Net;
using Dumps.Application.Command.Cart;
using Dumps.Application.DTO.Response.Cart;
using Dumps.Application.Query.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Dumps.API.Controllers
{
    [Authorize]
    public class CartController : BaseController
    {
        /// <summary>
        /// Add multiple products or bundles to the cart.
        /// </summary>
        /// <param name="command">The AddToCartCommand with a list of items.</param>
        /// <returns>APIResponse with the cart ID.</returns>
        [HttpPost("add")]
        [ProducesResponseType(typeof(APIResponse<Guid>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }


        [HttpGet("getByUserId")]
        public async Task<ActionResult<APIResponse<CartResponse>>> GetCartItemsByUserId()
        {
            var result = await Mediator.Send(new GetCartItemByUserId.GetCartItemsByUserIdQuery());
            return Ok(result);
        }

        /// <summary>
        /// Clear all items from the user's cart.
        /// </summary>
        /// <returns>APIResponse indicating the result of the operation.</returns>
        [HttpDelete("clear")]
        [ProducesResponseType(typeof(APIResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ClearCart()
        {
            var result = await Mediator.Send(new ClearCartCommand());
            return Ok(result);
        }

        [HttpDelete("remove")]
        public async Task<IActionResult> RemoveCartItem([FromBody] RemoveCartItemCommand command)
        {
            var response = await Mediator.Send(command);
            return Ok(response);

        }

        [HttpPost("apply-promo")]
        [ProducesResponseType(typeof(APIResponse<decimal>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ApplyPromoCode([FromBody] ApplyPromoCodeCommand command)
        {
            var result = await Mediator.Send(command);
            return Ok(result);
        }

        [HttpPost("remove-promo")]
        [ProducesResponseType(typeof(APIResponse<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(APIResponse<object>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> RemovePromoCode()
        {
            var result = await Mediator.Send(new RemovePromoCodeCommand());
            return Ok(result);
        }
    }
}
