using InventoryMan.Application.Common.Models;
using Swashbuckle.AspNetCore.Filters;

namespace InventoryMan.API.Documentation
{
    /// <summary>
    /// Clase estática que contiene todas las descripciones de la API para Swagger
    /// </summary>
    public static class ApiDocumentation
    {
        #region Descripciones Generales
        public const string ApiDescription = @"API REST que proporciona servicios para la gestión de inventarios, productos y almacenes. 
        
## Funcionalidades Principales

### Gestión de Inventario
- Monitoreo de alertas por stock mínimo
- Actualización de niveles de stock
- Gestión de stock mínimo
- Transferencias entre tiendas

### Gestión de Productos
- Catálogo completo de productos
- Alta de nuevos productos
- Consulta de productos individuales
- Actualización de información de productos
- Eliminación de productos

### Gestión de Tiendas
- Consulta de inventario por tienda

## Estructura de la API

### Endpoints de Inventario
- POST /api/Inventory/alerts: Gestión de alertas por inventario mínimo
- POST /api/Inventory/update-stock: Actualización de niveles de stock
- POST /api/Inventory/update-min-stock: Actualización de stock mínimo
- POST /api/Inventory/transfer: Transferencias entre tienda

### Endpoints de Productos
- GET /api/Products: Obtener listado de productos
- POST /api/Products: Crear nuevo producto
- GET /api/Products/{id}: Obtener producto específico
- PUT /api/Products/{id}: Actualizar producto
- DELETE /api/Products/{id}: Eliminar producto

### Endpoints de Almacenes
- GET /api/Stores/{storeId}/inventory: Consultar inventario por tienda

## Notas Técnicas
- Ningún endpoint requiere autenticación
- Las respuestas se devuelven en formato JSON con la siguiente estructura:

        {
            ""isSuccess"": false,       
            ""data"": data,
            ""error"": ""Mensaje de error""
        }

   Donde:
    - isSuccess:  devuelve **true** o **false** indicando si la operación se realizó de forma satisfactoria o no.
    - data: si **isSuccess == true** entonces devuelve el resultado de la operación realizada, en caso contrario devuelve **null**.
    - error: si **isSuccess == false** entonces devuelve el mensaje del error encontrado, en caso contrario devuelve **null**.

";

        public const string ApiVersion = "v1";
        public const string ApiTitle = "Sistema de Gestión de Inventarios API";
        public const string ApiContactName = "Roberto Avalos V.";
        public const string ApiContactEmail = "ravalosv@gmail.com";
        #endregion
    }

}
