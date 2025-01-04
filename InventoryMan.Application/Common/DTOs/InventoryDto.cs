using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMan.Application.Common.DTOs
{
    /// <summary>
    /// DTO que representa la información de inventario
    /// </summary>
    public record InventoryDto
    {
        /// <summary>
        /// Identificador único del registro de inventario
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Identificador del producto
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Nombre del producto
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// Identificador de la tienda
        /// </summary>
        public string StoreId { get; set; }

        /// <summary>
        /// Cantidad actual en inventario
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Cantidad mínima permitida antes de generar una alerta
        /// </summary>
        public int MinStock { get; set; }
    }
}
