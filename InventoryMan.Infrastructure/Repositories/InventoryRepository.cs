using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using InventoryMan.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace InventoryMan.Infrastructure.Repositories
{
    public class InventoryRepository : BaseRepository<Core.Entities.Inventory>, IInventoryRepository
    {
        public InventoryRepository(InventoryDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Core.Entities.Inventory>> GetByStoreIdAndProductIdAsync(string storeId, string productId)
        {
            return await _dbSet
            .Include(i => i.Product)
            .Where(i => i.StoreId == storeId && i.ProductId == productId)
            .ToListAsync();
        }

        public async Task<IEnumerable<Core.Entities.Inventory>> GetByStoreIdAsync(string storeId)
        {
            return await _dbSet
                .Include(i => i.Product)
                .Where(i => i.StoreId == storeId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Core.Entities.Inventory>> GetLowStockItemsAsync()
        {
            return await _dbSet
                .Include(i => i.Product)
                .Where(i => i.Quantity < i.MinStock)
                .ToListAsync();
        }
    }

}
