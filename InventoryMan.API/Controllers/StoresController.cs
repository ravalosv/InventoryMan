using InventoryMan.API.Extensions;
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
        private readonly ILogger<StoresController> _logger;

        public StoresController(IMediator mediator, ILogger<StoresController> logger)
        {
            _mediator = mediator;
            this._logger = logger;
        }

        /// <summary>
        /// Obtiene el inventario de una tienda específica
        /// </summary>
        /// <remarks>
        /// # Descripción
        /// Este endpoint retorna la lista de productos en inventario para una tienda específica.
        /// Valida la existencia de la tienda antes de retornar el inventario.
        /// 
        /// ## Ejemplo de respuesta exitosa
        /// ```json
        /// {
        ///     "isSuccess": true,
        ///     "data": [
        ///         {
        ///             "id": "INV001",
        ///             "productId": "PROD001",
        ///             "productName": "Laptop HP Pavilion",
        ///             "storeId": "STORE001",
        ///             "quantity": 10,
        ///             "minStock": 5
        ///         },
        ///         {
        ///             "id": "INV002",
        ///             "productId": "PROD002",
        ///             "productName": "Mouse Inalámbrico",
        ///             "storeId": "STORE001",
        ///             "quantity": 25,
        ///             "minStock": 8
        ///         }
        ///     ],
        ///     "error": null
        /// }
        /// ```
        /// 
        /// ## Ejemplo de respuesta con error
        /// ```json
        /// {
        ///     "isSuccess": false,
        ///     "data": null,
        ///     "error": "Store with ID STORE001 not found"
        /// }
        /// ```
        /// </remarks>
        /// <param name="storeId">Identificador de la tienda</param>
        /// <returns>Resultado que contiene la lista de productos en inventario de la tienda</returns>
        /// <response code="200">Si se obtiene el inventario correctamente</response>
        /// <response code="400">Si el ID de la tienda es inválido o la tienda no existe</response>
        [HttpGet("{storeId}/inventory")]
        public async Task<IActionResult> GetByStore(string storeId)
        {
            try
            {
                _logger.LogInventoryOperation("GetByStore", "Started", new
                {
                    StoreId = storeId,
                    TraceId = HttpContext.TraceIdentifier,
                    RequestedBy = User?.Identity?.Name ?? "anonymous"
                }, level: LogLevel.Information);

                var result = await _mediator.Send(new GetInventoryByStoreQuery(storeId));

                if (result.IsSuccess)
                {
                    _logger.LogInventoryOperation("GetByStore", "Completed", new
                    {
                        StoreId = storeId,
                        ItemsCount = result.Data?.Count() ?? 0,
                        TraceId = HttpContext.TraceIdentifier
                    }, level: LogLevel.Information);
                    return Ok(result);
                }

                _logger.LogInventoryOperation("GetByStore", "Failed", new
                {
                    StoreId = storeId,
                    Error = result.Error,
                    TraceId = HttpContext.TraceIdentifier
                }, level: LogLevel.Warning);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogInventoryOperation("GetByStore", "Error", new
                {
                    StoreId = storeId,
                    TraceId = HttpContext.TraceIdentifier
                }, ex, LogLevel.Error);
                throw;
            }
        }

    }
}

