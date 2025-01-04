using MediatR;
using Microsoft.AspNetCore.Mvc;
using InventoryMan.Application.Inventory.Commands.UpdateStock;
using InventoryMan.Application.Inventory.Queries.GetLowStockItems;
using InventoryMan.Application.Test.Query;

namespace InventoryMan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TestController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet()]
        public async Task<IActionResult> Test()
        {
            var result = await _mediator.Send(new TestCommand());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }
    }
}

