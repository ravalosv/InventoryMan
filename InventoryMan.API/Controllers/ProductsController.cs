using MediatR;
using Microsoft.AspNetCore.Mvc;
using InventoryMan.Application.Common.DTOs;
using InventoryMan.Application.Common.Models;
using InventoryMan.Application.Products.Commands.CreateProduct;
using InventoryMan.Application.Products.Commands.DeleteProduct;
using InventoryMan.Application.Products.Commands.UpdateProduct;
using InventoryMan.Application.Products.Queries.GetProductById;
using InventoryMan.Application.Products.Queries.GetProducts;

namespace InventoryMan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<PagedList<ProductDto>>> GetProducts(
        [FromQuery] string? category,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] int? minStock,
        [FromQuery] int? maxStock,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? sortBy = null,
        [FromQuery] bool sortDesc = false)
        {
            var query = new GetProductsQuery
            {
                Category = category,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                MinStock = minStock,
                MaxStock = maxStock,
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDesc = sortDesc
            };

            var result = await _mediator.Send(query);

            if (result.IsSuccess)
                return Ok(result);

            return BadRequest(result);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var result = await _mediator.Send(new GetProductByIdQuery(id));
            return result.IsSuccess ? Ok(result) : NotFound(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess
                ? CreatedAtAction(nameof(GetById), new { id = result.Data }, result)
                : BadRequest(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProductCommand command)
        {
            if (id != command.Id)
                return BadRequest("ID mismatch");

            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var result = await _mediator.Send(new DeleteProductCommand { Id = id });
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}
