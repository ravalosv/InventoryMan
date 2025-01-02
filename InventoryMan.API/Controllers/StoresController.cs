using InventoryMan.Application.Inventory.Queries.GetInventoryByStore;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace InventoryMan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StoresController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StoresController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{storeId}/inventory")]
        public async Task<IActionResult> GetByStore(string storeId)
        {
            var result = await _mediator.Send(new GetInventoryByStoreQuery(storeId));
            return result.IsSuccess ? Ok(result.Data) : BadRequest(result.Error);
        }
    }
}

