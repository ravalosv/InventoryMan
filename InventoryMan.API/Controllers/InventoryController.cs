using MediatR;
using Microsoft.AspNetCore.Mvc;
using InventoryMan.Application.Inventory.Commands.UpdateStock;
using InventoryMan.Application.Inventory.Queries.GetLowStockItems;

namespace InventoryMan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InventoryController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet("alerts")]
        public async Task<IActionResult> GetLowStockItems()
        {
            var result = await _mediator.Send(new GetLowStockItemsQuery());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("update-stock")]
        public async Task<IActionResult> UpdateStock([FromBody] UpdateStockCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("update-min-stock")]
        public async Task<IActionResult> UpdateMinStock([FromBody] UpdateMinStockCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferStockCommand command)
        {
            var result = await _mediator.Send(command);
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

    }
}

