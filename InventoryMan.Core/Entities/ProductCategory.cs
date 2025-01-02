namespace InventoryMan.Core.Entities
{
    public class ProductCategory
    {
        public int Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
