using Dumps.Application.Command.Products;
using Dumps.Application.DTO.Response.Products;
using Dumps.Application.Query.Products;
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

        [HttpGet]
        public async Task<ActionResult<APIResponse<IList<ProductResponse>>>> GetProducts([FromQuery] string? sort,
            [FromQuery] int page = 1, [FromQuery] int pageSize  = 10, [FromQuery] string? search = null)
        {
            return await Mediator.Send(new GetAllProducts.GetAllProductsQuery(sort, page, pageSize, search));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse<ProductResponse>>> GetProduct(Guid id)
        {
            return await Mediator.Send(request: new GetProductById.GetProductByIdQuery { Id = id });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(Guid id)
        {
            var result = await Mediator.Send(new DeleteProductCommand { ProductId = id });

            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProduct(Guid id, [FromForm] UpdateProductCommand command)
        {
            var result = await Mediator.Send(new UpdateProductCommand { ProductId = id });

            return Ok(result);
        }
    }
}
