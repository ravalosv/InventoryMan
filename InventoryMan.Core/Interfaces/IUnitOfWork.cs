using Microsoft.EntityFrameworkCore.Storage;

namespace InventoryMan.Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IProductRepository Products { get; }
        IInventoryRepository Inventories { get; }
        IMovementRepository Movements { get; }
        IStoreRepository Stores { get; }
        ITestRepository Tests { get; }
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
