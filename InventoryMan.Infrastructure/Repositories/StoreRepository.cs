using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using InventoryMan.Infrastructure.Data.Context;

namespace InventoryMan.Infrastructure.Repositories
{
    public class StoreRepository : BaseRepository<Store>, IStoreRepository
    {
        public StoreRepository(InventoryDbContext context) : base(context)
        {
        }
    }
}
