using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMan.Application.Common.DTOs
{
    public record ProductDto
    {
        public string Id { get; init; } = default!;
        public string Name { get; init; } = default!;
        public string Description { get; init; } = default!;
        public int CategoryId { get; init; } = default!;
        public string CategoryName { get; set; } = default!;
        public decimal Price { get; init; }
        public string Sku { get; init; } = default!;
    }
}
