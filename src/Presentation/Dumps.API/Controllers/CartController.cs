﻿
using System.Net;
using Dumps.Application.Command.Cart;
using Dumps.Application.DTO.Response.Cart;
using Dumps.Application.DTO.Response.Products;
using Dumps.Application.Query.Cart;
using Dumps.Application.Query.Products;
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
            command.UserId = User.FindFirst("sub")?.Value; // Assuming "sub" contains the UserId

            var result = await Mediator.Send(command);
            return Ok(result);
        }


        [HttpGet("{userId}")]
        public async Task<ActionResult<APIResponse<CartResponse>>> GetCartItemsByUserId(string userId)
        {
            return await Mediator.Send(new GetCartItemByUserId.GetCartItemsByUserIdQuery { UserId = userId });
        }
    }
}
