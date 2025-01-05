using MediatR;
using Microsoft.AspNetCore.Mvc;
using InventoryMan.Application.Common.DTOs;
using InventoryMan.Application.Common.Models;
using InventoryMan.Application.Products.Commands.CreateProduct;
using InventoryMan.Application.Products.Commands.DeleteProduct;
using InventoryMan.Application.Products.Commands.UpdateProduct;
using InventoryMan.Application.Products.Queries.GetProductById;
using InventoryMan.Application.Products.Queries.GetProducts;
using InventoryMan.API.Extensions;

namespace InventoryMan.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
        {
            _mediator = mediator;
            this._logger = logger;
        }

        /// <summary>
        /// Obtiene un listado paginado y filtrado de productos
        /// </summary>
        /// <remarks>
        /// # Descripción
        /// Este endpoint permite obtener un listado de productos con opciones de:
        /// * Filtrado por categoría, precio y stock
        /// * Paginación
        /// * Ordenamiento
        /// 
        /// ## Parámetros de filtrado
        /// * category: Filtro por nombre de categoría (búsqueda parcial, no sensible a mayúsculas)
        /// * minPrice: Precio mínimo
        /// * maxPrice: Precio máximo
        /// * minStock: Stock mínimo en inventario
        /// * maxStock: Stock máximo en inventario
        /// 
        /// ## Parámetros de paginación
        /// * pageNumber: Número de página (default: 1)
        /// * pageSize: Elementos por página (default: 10)
        /// 
        /// ## Parámetros de ordenamiento
        /// * sortBy: Campo de ordenamiento ("name", "price", "category")
        /// * sortDesc: true para orden descendente, false para ascendente
        /// 
        /// ## Ejemplo de respuesta exitosa
        /// ```json
        /// {
        ///     "isSuccess": true,
        ///     "value": {
        ///         "items": [
        ///             {
        ///                 "id": "PROD001",
        ///                 "name": "Producto 1",
        ///                 "price": 100.00,
        ///                 "category": "Categoría 1"
        ///             }
        ///         ],
        ///         "pageNumber": 1,
        ///         "pageSize": 10,
        ///         "totalPages": 1,
        ///         "totalCount": 1
        ///     },
        ///     "error": null
        /// }
        /// ```
        /// </remarks>
        /// <param name="category">Filtro por categoría</param>
        /// <param name="minPrice">Precio mínimo</param>
        /// <param name="maxPrice">Precio máximo</param>
        /// <param name="minStock">Stock mínimo</param>
        /// <param name="maxStock">Stock máximo</param>
        /// <param name="pageNumber">Número de página</param>
        /// <param name="pageSize">Elementos por página</param>
        /// <param name="sortBy">Campo de ordenamiento</param>
        /// <param name="sortDesc">Orden descendente</param>
        /// <returns>Lista paginada de productos</returns>
        /// <response code="200">Retorna la lista de productos</response>
        /// <response code="400">Si ocurre un error durante la consulta</response>
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
            try
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

                _logger.LogProductOperation("GetProducts", "Started", new
                {
                    Filters = query,
                    TraceId = HttpContext.TraceIdentifier
                });

                var result = await _mediator.Send(query);

                if (result.IsSuccess)
                {
                    _logger.LogProductOperation("GetProducts", "Completed", new
                    {
                        TotalCount = result.Data.TotalCount,
                        PageSize = result.Data.PageSize,
                        PageNumber = result.Data.PageNumber,
                        TotalPages = result.Data.TotalPages
                    });
                    return Ok(result);
                }

                _logger.LogProductOperation("GetProducts", "Failed", new { Error = result.Error },
                    level: LogLevel.Warning);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogProductOperation("GetProducts", "Error", new { }, ex, LogLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// Obtiene un producto por su identificador
        /// </summary>
        /// <remarks>
        /// # Descripción
        /// Este endpoint permite obtener los detalles de un producto específico utilizando su identificador único.
        /// 
        /// ## Ejemplo de respuesta exitosa
        /// ```json
        /// {
        ///     "isSuccess": true,
        ///     "value": {
        ///         "id": "PROD001",
        ///         "name": "Producto Ejemplo",
        ///         "description": "Descripción del producto",
        ///         "categoryId": "CAT001",
        ///         "categoryName": "Categoría Ejemplo",
        ///         "price": 199.99,
        ///         "sku": "SKU001"
        ///     },
        ///     "error": null
        /// }
        /// ```
        /// 
        /// ## Ejemplo de respuesta no encontrada
        /// ```json
        /// {
        ///     "isSuccess": false,
        ///     "value": null,
        ///     "error": "Product not found"
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">Identificador único del producto</param>
        /// <returns>Detalles del producto solicitado</returns>
        /// <response code="200">Retorna el producto encontrado</response>
        /// <response code="404">Si el producto no existe</response>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            try
            {
                _logger.LogProductOperation("GetById", "Started", new { ProductId = id });

                var result = await _mediator.Send(new GetProductByIdQuery(id));

                if (result.IsSuccess)
                {
                    _logger.LogProductOperation("GetById", "Completed", new { ProductId = id });
                    return Ok(result);
                }

                _logger.LogProductOperation("GetById", "NotFound", new { ProductId = id },
                    level: LogLevel.Warning);
                return NotFound(result);
            }
            catch (Exception ex)
            {
                _logger.LogProductOperation("GetById", "Error", new { ProductId = id }, ex, LogLevel.Error);
                throw;
            }
        }
        /// <summary>
        /// Crea un nuevo producto
        /// </summary>
        /// <remarks>
        /// # Descripción
        /// Este endpoint permite crear un nuevo producto en el sistema.
        /// 
        /// ## Reglas de validación
        /// * Id: Requerido, máximo 30 caracteres
        /// * Nombre: Requerido, máximo 100 caracteres
        /// * Precio: Debe ser mayor a 0
        /// * SKU: Requerido, máximo 25 caracteres
        /// * CategoryId: Debe ser mayor a 0
        /// 
        /// ## Ejemplo de petición
        /// ```json
        /// {
        ///     "id": "PROD001",
        ///     "name": "Nuevo Producto",
        ///     "description": "Descripción del producto",
        ///     "categoryId": 1,
        ///     "price": 199.99,
        ///     "sku": "SKU001"
        /// }
        /// ```
        /// 
        /// ## Ejemplo de respuesta exitosa
        /// ```json
        /// {
        ///     "isSuccess": true,
        ///     "data": "PROD001",
        ///     "error": null
        /// }
        /// ```
        /// 
        /// ## Ejemplo de respuesta con error
        /// ```json
        /// {
        ///     "isSuccess": false,
        ///     "data": null,
        ///     "error": "El nombre es requerido"
        /// }
        /// ```
        /// </remarks>
        /// <returns>Identificador del producto creado</returns>
        /// <response code="201">Retorna el id del producto creado</response>
        /// <response code="400">Si los datos de entrada son inválidos</response>
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
        {
            try
            {
                _logger.LogProductOperation("Create", "Started", new
                {
                    Product = new
                    {
                        command.Name,
                        command.CategoryId,
                        command.Price,
                        RequestedBy = User?.Identity?.Name ?? "anonymous"
                    }
                });

                var result = await _mediator.Send(command);

                if (result.IsSuccess)
                {
                    _logger.LogProductOperation("Create", "Completed", new
                    {
                        ProductId = result.Data,
                        command.Name,
                        command.CategoryId
                    });
                    return CreatedAtAction(nameof(GetById), new { id = result.Data }, result);
                }

                _logger.LogProductOperation("Create", "Failed", new { Error = result.Error },
                    level: LogLevel.Warning);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogProductOperation("Create", "Error", command, ex, LogLevel.Error);
                throw;
            }
        }
        /// <summary>
        /// Actualiza un producto existente
        /// </summary>
        /// <remarks>
        /// # Descripción
        /// Este endpoint permite actualizar la información de un producto existente.
        /// 
        /// ## Reglas de validación
        /// * Nombre: Requerido, máximo 100 caracteres
        /// * Precio: Debe ser mayor a 0
        /// * SKU: Requerido, máximo 25 caracteres
        /// * CategoryId: Debe ser mayor a 0
        /// 
        /// ## Ejemplo de petición
        /// ```json
        /// {
        ///     "id": "PROD001",
        ///     "name": "Producto Actualizado",
        ///     "description": "Nueva descripción del producto",
        ///     "sku": "SKU001",
        ///     "categoryId": 1,
        ///     "price": 299.99
        /// }
        /// ```
        /// 
        /// ## Ejemplo de respuesta exitosa
        /// ```json
        /// {
        ///     "isSuccess": true,
        ///     "data": true,
        ///     "error": null
        /// }
        /// ```
        /// 
        /// ## Ejemplo de respuesta con error
        /// ```json
        /// {
        ///     "isSuccess": false,
        ///     "data": false,
        ///     "error": "Product not found"
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">Identificador del producto a actualizar</param>
        /// <returns>Resultado de la operación de actualización</returns>
        /// <response code="200">Si el producto se actualizó correctamente</response>
        /// <response code="400">Si los datos son inválidos o hay discrepancia en el ID</response>
        /// <response code="404">Si el producto no existe</response>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, [FromBody] UpdateProductCommand command)
        {
            try
            {
                if (id != command.Id)
                {
                    _logger.LogProductOperation("Update", "IdMismatch", new
                    {
                        PathId = id,
                        CommandId = command.Id
                    }, level: LogLevel.Warning);
                    return BadRequest("ID mismatch");
                }

                _logger.LogProductOperation("Update", "Started", new
                {
                    ProductId = id,
                    Updates = new
                    {
                        command.Name,
                        command.CategoryId,
                        command.Price,
                        RequestedBy = User?.Identity?.Name ?? "anonymous"
                    }
                });

                var result = await _mediator.Send(command);

                if (result.IsSuccess)
                {
                    _logger.LogProductOperation("Update", "Completed", new { ProductId = id });
                    return Ok(result);
                }

                _logger.LogProductOperation("Update", "Failed", new
                {
                    ProductId = id,
                    Error = result.Error
                }, level: LogLevel.Warning);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogProductOperation("Update", "Error", new { ProductId = id }, ex, LogLevel.Error);
                throw;
            }
        }

        /// <summary>
        /// Elimina un producto existente
        /// </summary>
        /// <remarks>
        /// # Descripción
        /// Este endpoint permite eliminar un producto del sistema mediante su identificador.
        /// 
        /// ## Reglas de validación
        /// * Id: Requerido
        /// 
        /// ## Ejemplo de respuesta exitosa
        /// ```json
        /// {
        ///     "isSuccess": true,
        ///     "data": true,
        ///     "error": null
        /// }
        /// ```
        /// 
        /// ## Ejemplo de respuesta con error
        /// ```json
        /// {
        ///     "isSuccess": false,
        ///     "data": false,
        ///     "error": "Product with id PROD001 not found"
        /// }
        /// ```
        /// </remarks>
        /// <param name="id">Identificador del producto a eliminar</param>
        /// <returns>Resultado de la operación de eliminación</returns>
        /// <response code="200">Si el producto se eliminó correctamente</response>
        /// <response code="400">Si el ID es inválido</response>
        /// <response code="404">Si el producto no existe</response>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                _logger.LogProductOperation("Delete", "Started", new
                {
                    ProductId = id,
                    RequestedBy = User?.Identity?.Name ?? "anonymous"
                });

                var result = await _mediator.Send(new DeleteProductCommand { Id = id });

                if (result.IsSuccess)
                {
                    _logger.LogProductOperation("Delete", "Completed", new { ProductId = id });
                    return Ok(result);
                }

                _logger.LogProductOperation("Delete", "Failed", new
                {
                    ProductId = id,
                    Error = result.Error
                }, level: LogLevel.Warning);
                return BadRequest(result);
            }
            catch (Exception ex)
            {
                _logger.LogProductOperation("Delete", "Error", new { ProductId = id }, ex, LogLevel.Error);
                throw;
            }
        }
    }
}
