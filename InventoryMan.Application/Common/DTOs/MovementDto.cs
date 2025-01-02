using InventoryMan.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMan.Application.Common.DTOs
{
    public record MovementDto
    {
        public string Id { get; init; } = default!;
        public string ProductId { get; init; } = default!;
        public string ProductName { get; init; } = default!;
        public int Quantity { get; init; }
        public MovementType Type { get; init; } = default!;
        public string? SourceStoreId { get; init; }
        public string? TargetStoreId { get; init; }
        public DateTime Timestamp { get; init; }
    }
}
