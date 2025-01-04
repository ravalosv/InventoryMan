using InventoryMan.Core.Entities;

namespace InventoryMan.Core.Interfaces
{
    public interface IInventoryRepository : IRepository<Inventory>
    {
        Task<IEnumerable<Inventory>> GetByStoreIdAsync(string storeId);
        Task<IEnumerable<Inventory>> GetLowStockItemsAsync();
        Task<IEnumerable<Inventory>> GetByStoreIdAndProductIdAsync(string storeId, string productId);


    }
}
