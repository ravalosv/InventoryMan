using MediatR;
using Microsoft.AspNetCore.Mvc;
using InventoryMan.Application.Test.Query;

namespace InventoryMan.API.Controllers
{
    /// <summary>
    /// Controlador para realizar pruebas y verificar el estado del sistema
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class TestsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public TestsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Realiza una prueba de conexión a la base de datos y recuperación de datos
        /// </summary>
        /// <remarks>
        /// # Descripción
        /// Este endpoint verifica la conexión a la base de datos y retorna una lista de elementos de prueba
        /// 
        /// ## Ejemplo de respuesta exitosa:
        /// ``` json
        /// {
        ///     "isSuccess": true,
        ///     "value": [
        ///         {
        ///             "id": 1,
        ///             "name": "Database connection successfully established"
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
        ///     "error": "Mensaje de error"
        /// }
        /// ```
        /// </remarks>
        /// <response code="200">La prueba se realizó correctamente</response>
        /// <response code="400">Error al realizar la prueba</response>
        /// <response code="500">Error interno del servidor</response>
        /// <returns>Lista de elementos de prueba</returns>
        [HttpGet("DBTest")]
        public async Task<IActionResult> Test()
        {
            var result = await _mediator.Send(new TestCommand());
            return result.IsSuccess ? Ok(result) : BadRequest(result);
        }

        /// <summary>
        /// Verifica el estado de la API
        /// </summary>
        /// <remarks>
        /// Endpoint simple que confirma que la API está en funcionamiento
        /// 
        /// Ejemplo de respuesta:
        /// "API is running"
        /// </remarks>
        /// <response code="200">La API está funcionando correctamente</response>
        [HttpGet("health")]
        public IActionResult Health()
        {
            return Ok("API is running");
        }

        /// <summary>
        /// Endpoint para probar el manejo de excepciones
        /// </summary>
        /// <remarks>
        /// Este endpoint lanza una excepción deliberadamente para probar el manejo de errores
        /// </remarks>
        /// <response code="500">Error interno del servidor (esperado)</response>
        [HttpGet("test-error")]
        public IActionResult TestError()
        {
            throw new Exception("Esta es una excepción de prueba");
        }
    }
}

