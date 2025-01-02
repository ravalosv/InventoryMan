using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMan.Core.Entities
{
    public class Movement
    {
        public string Id { get; set; } = default!;
        public string ProductId { get; set; } = default!;
        public string? SourceStoreId { get; set; } = default!;
        public string? TargetStoreId { get; set; } = default!;
        public int Quantity { get; set; }
        public DateTime Timestamp { get; set; }
        public MovementType Type { get; set; }

        public Product? Product { get; set; }
    }

    public enum MovementType
    {
        IN,
        OUT,
        TRANSFER
    }

}
