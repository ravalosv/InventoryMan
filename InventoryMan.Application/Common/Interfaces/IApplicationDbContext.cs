using InventoryMan.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventoryMan.Application.Common.Interfaces
{
    public interface IApplicationDbContext
    {
        DbSet<Product> Products { get; }
        DbSet<Core.Entities.Inventory> Inventories { get; }
        DbSet<Movement> Movements { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
