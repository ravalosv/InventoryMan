namespace InventoryMan.Core.Entities
{
    public class Inventory
    {
        public string Id { get; set; } = default!;
        public string ProductId { get; set; } = default!;
        public string StoreId { get; set; } = default!;
        public int Quantity { get; set; }
        public int MinStock { get; set; }

        public Product? Product { get; set; }
        public Store? Store { get; set; }
    }
}
