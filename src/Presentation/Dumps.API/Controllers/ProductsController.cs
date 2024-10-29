﻿using Dumps.Application.APIResponse;
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
        public async Task<ActionResult<APIResponse<IList<ProductResponse>>>> GetProducts()
        {
            return await Mediator.Send(new GetAllProducts.GetAllProductsQuery());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<APIResponse<ProductResponse>>> GetProduct(Guid id)
        {
            return await Mediator.Send(request: new GetProductById.GetProductByIdQuery { Id = id });
        }
    }
}