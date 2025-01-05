using MediatR;
using Microsoft.AspNetCore.Mvc;
using InventoryMan.Application.Inventory.Commands.UpdateStock;
using InventoryMan.Application.Inventory.Queries.GetLowStockItems;
using InventoryMan.API.Extensions;


namespace InventoryMan.API.Controllers
{
    /// <summary>
    /// Controlador para la gestión de inventario.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class InventoryController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<InventoryController> _logger;

        public InventoryController(IMediator mediator, ILogger<InventoryController> logger)
        {
            _mediator = mediator;
            this._logger = logger;
        }

        /// <summary>
        /// Obtiene una lista de productos con stock bajo
        /// </summary>
        /// <remarks>
        /// # Descripción
        /// Este endpoint devuelve todos los productos que están por debajo de su nivel mínimo de stock establecido.
        /// 
        /// ## Detalles de la respuesta
        /// La respuesta incluye:
        /// * Id del inventario
        /// * Id del producto
        /// * Nombre del producto
        /// * Id de la tienda
        /// * Cantidad actual
        /// * Stock mínimo permitido
        /// 
        /// ## Ejemplo de respuesta exitosa
        /// ```json
        /// {
        ///     "isSuccess": true,
        ///     "value": [
        ///         {
        ///             "id": 1,
        ///             "productId": 100,
        ///             "productName": "Producto Ejemplo",
        ///             "storeId": 1,
        ///             "quantity": 5,
        ///             "minStock": 10
        ///         }
        ///     ],
        ///     "error": null
        /// }
        /// ```
        /// 
        /// ## Ejemplo de respuesta fallida
        /// ```json
        /// {
        ///     "isSuccess": false,
        ///     "value": null,
        ///     "error": "Error getting low stock items: [Detalle del error]"
        /// }
        /// ```
        /// 
        /// ## Notas importantes
        /// * Los productos se consideran con stock bajo cuando `quantity` &lt; `minStock`
        /// </remarks>
        /// <returns>Una lista de productos que tienen stock por debajo del mínimo establecido</returns>
        /// <response code="200">Retorna la lista de productos con stock bajo</response>
        /// <response code="400">Si ocurre un error durante la consulta</response>
        [HttpGet("alerts")]
        public async Task<IActionResult> GetLowStockItems()
        {
            try
            {
                _logger.LogInventoryOperation("GetLowStockItems", "Started", new
                {
                    TraceId = HttpContext.TraceIdentifier,
                    RequestedBy = User?.Identity?.Name ?? "anonymous"
                });

                var result = await _mediator.Send(new GetLowStockItemsQuery());

                if (result.IsSuccess)
                {
                    _logger.LogInventoryOperation("GetLowStockItems", "Completed", new
                    {
                        AlertCount = result.Data?.Count() ?? 0,
                        TraceId = HttpContext.TraceIdentifier,
                        Items = result.Data?.Select(item => new
                        {
                            item.ProductId,
                            item.Quantity,
                            item.MinStock
                        })
                    });
                    return Ok(result);
                }

                _logger.LogInventoryOperation("GetLowStockItems", "Failed", new
                {
                    Error = result.Error,
                    TraceId = HttpContext.TraceIdentifier
                }, level: LogLevel.Warning);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogInventoryOperation("GetLowStockItems", "Error", new
                {
                    TraceId = HttpContext.TraceIdentifier
                }, ex, LogLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// Actualiza el stock de un producto en una tienda específica
        /// </summary>
        /// <remarks>
        /// # Descripción
        /// Este endpoint permite actualizar el inventario de un producto, registrando movimientos de entrada (IN) o salida (OUT).
        /// 
        /// ## Detalles de la petición
        /// El cuerpo de la petición debe incluir:
        /// * ProductId: Identificador del producto
        /// * StoreId: Identificador de la tienda
        /// * Quantity: Cantidad a modificar (debe ser mayor que 0)
        /// * MovementType: Tipo de movimiento (IN/OUT)
        /// 
        /// ## Ejemplo de petición
        /// ```json
        /// {
        ///     "productId": "PROD001",
        ///     "storeId": "STORE001",
        ///     "quantity": 10,
        ///     "movementType": "IN"
        /// }
        /// ```
        /// 
        /// ## Ejemplo de respuesta exitosa
        /// ```json
        /// {
        ///     "isSuccess": true,
        ///     "value": true,
        ///     "error": null
        /// }
        /// ```
        /// 
        /// ## Ejemplo de respuesta fallida
        /// ```json
        /// {
        ///     "isSuccess": false,
        ///     "value": false,
        ///     "error": "Error updating product stock: There is not enough inventory"
        /// }
        /// ```
        /// 
        /// ## Reglas de negocio
        /// * Para movimientos tipo OUT, se valida que haya suficiente inventario disponible
        /// * Si no existe inventario previo:
        ///   - Solo se permiten movimientos tipo IN
        ///   - Se crea un nuevo registro de inventario
        /// * Cada movimiento genera un registro en la tabla de movimientos
        /// * La operación es transaccional
        /// 
        /// ## Validaciones
        /// * ProductId: Requerido
        /// * StoreId: Requerido
        /// * Quantity: Debe ser mayor que 0
        /// * MovementType: Debe ser IN o OUT
        /// </remarks>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">La actualización se realizó correctamente</response>
        /// <response code="400">Si ocurre un error durante la actualización o las validaciones fallan</response>
        [HttpPost("update-stock")]
        public async Task<IActionResult> UpdateStock([FromBody] UpdateStockCommand command)
        {
            try
            {
                _logger.LogStockUpdate("Started", command);

                var result = await _mediator.Send(command);

                if (result.IsSuccess)
                {
                    _logger.LogStockUpdate("Completed", command, new
                    {
                        Status = "Success",
                        ResultData = result.Data
                    });
                    return Ok(result);
                }

                _logger.LogStockUpdate("Failed", command, new
                {
                    Status = "Failed",
                    Error = result.Error
                }, level: LogLevel.Warning);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogStockUpdate("Error", command, exception: ex, level: LogLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// Actualiza el stock mínimo de un producto en una tienda específica
        /// </summary>
        /// <remarks>
        /// # Descripción
        /// Este endpoint permite actualizar el nivel mínimo de stock permitido para un producto en una tienda específica.
        /// Si el producto no existe en el inventario, se crea un nuevo registro con cantidad 0.
        /// 
        /// ## Detalles de la petición
        /// La petición requiere:
        /// * ProductId: Identificador del producto
        /// * StoreId: Identificador de la tienda
        /// * MinStock: Cantidad mínima permitida
        /// 
        /// ## Ejemplo de petición
        /// ```json
        /// {
        ///     "productId": "PROD001",
        ///     "storeId": "STORE001",
        ///     "minStock": 5
        /// }
        /// ```
        /// 
        /// ## Ejemplo de respuesta exitosa
        /// ```json
        /// {
        ///     "isSuccess": true,
        ///     "value": true,
        ///     "error": null
        /// }
        /// ```
        /// 
        /// ## Ejemplo de respuesta fallida
        /// ```json
        /// {
        ///     "isSuccess": false,
        ///     "value": false,
        ///     "error": "Error updating product min stock: [Detalle del error]"
        /// }
        /// ```
        /// 
        /// ## Reglas de negocio
        /// * Si el producto no existe en el inventario:
        ///   - Se crea un nuevo registro
        ///   - La cantidad inicial es 0
        ///   - Se establece el stock mínimo especificado
        /// * Si el producto existe:
        ///   - Se actualiza solo el valor de stock mínimo
        /// * La operación es transaccional
        /// </remarks>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">La actualización se realizó correctamente</response>
        /// <response code="400">Si ocurre un error durante la actualización</response>
        [HttpPost("update-min-stock")]
        public async Task<IActionResult> UpdateMinStock([FromBody] UpdateMinStockCommand command)
        {
            try
            {
                _logger.LogMinStockUpdate("Started", command);

                var result = await _mediator.Send(command);

                if (result.IsSuccess)
                {
                    _logger.LogMinStockUpdate("Completed", command, new
                    {
                        Status = "Success",
                        ResultData = result.Data
                    });
                    return Ok(result);
                }

                _logger.LogMinStockUpdate("Failed", command, new
                {
                    Status = "Failed",
                    Error = result.Error
                }, level: LogLevel.Warning);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogMinStockUpdate("Error", command, exception: ex, level: LogLevel.Error);
                throw;
            }
        }


        /// <summary>
        /// Transfiere stock de un producto entre dos tiendas
        /// </summary>
        /// <remarks>
        /// # Descripción
        /// Este endpoint permite transferir una cantidad específica de un producto desde una tienda origen hacia una tienda destino.
        /// 
        /// ## Detalles de la petición
        /// La petición requiere:
        /// * ProductId: Identificador del producto a transferir
        /// * SourceStoreId: Identificador de la tienda origen
        /// * TargetStoreId: Identificador de la tienda destino
        /// * Quantity: Cantidad a transferir
        /// 
        /// ## Ejemplo de petición
        /// ```json
        /// {
        ///     "productId": "PROD001",
        ///     "sourceStoreId": "STORE001",
        ///     "targetStoreId": "STORE002",
        ///     "quantity": 5
        /// }
        /// ```
        /// 
        /// ## Ejemplo de respuesta exitosa
        /// ```json
        /// {
        ///     "isSuccess": true,
        ///     "value": true,
        ///     "error": null
        /// }
        /// ```
        /// 
        /// ## Ejemplo de respuesta fallida
        /// ```json
        /// {
        ///     "isSuccess": false,
        ///     "value": false,
        ///     "error": "Error updating product stock: There is not enough inventory"
        /// }
        /// ```
        /// 
        /// ## Reglas de negocio
        /// * La tienda origen debe tener suficiente stock disponible
        /// * Las tiendas origen y destino deben ser diferentes
        /// * Si la tienda destino no tiene el producto:
        ///   - Se crea un nuevo registro de inventario
        ///   - Se establece MinStock en 0
        /// * Se registra un movimiento tipo TRANSFER
        /// * La operación es transaccional
        /// </remarks>
        /// <returns>Resultado de la operación</returns>
        /// <response code="200">La transferencia se realizó correctamente</response>
        /// <response code="400">Si ocurre un error durante la transferencia</response>
        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferStockCommand command)
        {
            try
            {
                _logger.LogStockTransfer("Started", command);

                var result = await _mediator.Send(command);

                if (result.IsSuccess)
                {
                    _logger.LogStockTransfer("Completed", command, new
                    {
                        Status = "Success",
                        ResultData = result.Data
                    });
                    return Ok(result);
                }

                _logger.LogStockTransfer("Failed", command, new
                {
                    Status = "Failed",
                    Error = result.Error
                }, level: LogLevel.Warning);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogStockTransfer("Error", command, exception: ex, level: LogLevel.Error);
                throw;
            }
        }


    }
}

