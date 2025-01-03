using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using InventoryMan.Infrastructure.Data.Context;

namespace InventoryMan.Infrastructure.Repositories
{
    public class MovementRepository : BaseRepository<Movement>, IMovementRepository
    {
        public MovementRepository(InventoryDbContext context) : base(context)
        {
        }
    }

}
