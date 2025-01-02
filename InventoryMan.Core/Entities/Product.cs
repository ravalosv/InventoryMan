namespace InventoryMan.Core.Entities
{
    public class Product
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public int CategoryId { get; set; } = default!;
        public decimal Price { get; set; }
        public string Sku { get; set; } = default!;

        public ProductCategory Category { get; set; }
        public ICollection<Inventory> Inventories { get; set; } = new List<Inventory>();
    }
}
