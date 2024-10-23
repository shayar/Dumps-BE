using Dumps.Application.Command.Products;
using Microsoft.AspNetCore.Mvc;

namespace Dumps.API.Controllers
{
    public class ProductsController : BaseController
    {
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromForm] CreateProductCommand command)
        {
            var result = await Mediator.Send(command);

            if (result.Succeeded)
            {
                return Ok(new { message = "Product created successfully.", productId = result.ProductId });
            }

            return BadRequest(result.Errors);
        }
    }
}
