using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMan.Application.Common.DTOs
{
    public record InventoryDto
    {
        public string Id { get; init; } = default!;
        public string ProductId { get; init; } = default!;
        public string ProductName { get; init; } = default!;
        public string StoreId { get; init; } = default!;
        public int Quantity { get; init; }
        public int MinStock { get; init; }
    }
}
