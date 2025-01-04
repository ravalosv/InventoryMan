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

    /// <summary>
    /// Tipos de movimiento de inventario
    /// </summary>
    public enum MovementType
    {
        /// <summary>
        /// Entrada de inventario
        /// </summary>
        IN,

        /// <summary>
        /// Salida de inventario
        /// </summary>
        OUT,
        /// <summary>
        /// Transferencia entre tiendas
        /// </summary>
        TRANSFER
    }

}
