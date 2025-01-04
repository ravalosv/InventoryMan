using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMan.Application.Common.DTOs
{
    /// <summary>
    /// DTO para la representación detallada de un producto
    /// </summary>
    public class ProductDto
    {
        /// <summary>
        /// Identificador único del producto
        /// </summary>
        /// <example>PROD001</example>
        public string Id { get; set; } = default!;

        /// <summary>
        /// Nombre del producto
        /// </summary>
        /// <example>Laptop HP Pavilion</example>
        public string Name { get; set; } = default!;

        /// <summary>
        /// Descripción detallada del producto
        /// </summary>
        /// <example>Laptop HP Pavilion con procesador Intel i5, 8GB RAM, 256GB SSD</example>
        public string Description { get; set; } = default!;

        /// <summary>
        /// Identificador de la categoría del producto
        /// </summary>
        /// <example>CAT001</example>
        public int CategoryId { get; set; } = default!;

        /// <summary>
        /// Nombre de la categoría del producto
        /// </summary>
        /// <example>Electrónicos</example>
        public string? CategoryName { get; set; }

        /// <summary>
        /// Precio del producto
        /// </summary>
        /// <example>999.99</example>
        public decimal Price { get; set; }

        /// <summary>
        /// Código SKU del producto
        /// </summary>
        /// <example>SKU001</example>
        public string Sku { get; set; } = default!;
    }

}
