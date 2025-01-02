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
                .Where(i => i.Quantity <= i.MinStock)
                .ToListAsync();
        }

        public override async Task<Core.Entities.Inventory?> GetByIdAsync(string id)
        {
            return await _dbSet
                .Include(i => i.Product)
                .FirstOrDefaultAsync(i => i.Id == id);
        }

        public override async Task<IEnumerable<Core.Entities.Inventory>> GetAllAsync()
        {
            return await _dbSet
                .Include(i => i.Product)
                .ToListAsync();
        }
    }

}
