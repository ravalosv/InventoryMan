using InventoryMan.Core.Interfaces;
using InventoryMan.Infrastructure.Data.Context;
using InventoryMan.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore.Storage;

namespace InventoryMan.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly InventoryDbContext _context;
        private IProductRepository _productRepository;
        private IInventoryRepository _inventoryRepository;
        private IMovementRepository _movementRepository;
        private ITestRepository _testRepository;
        private IDbContextTransaction _currentTransaction;

        public UnitOfWork(InventoryDbContext context)
        {
            _context = context;
        }

        public IProductRepository Products =>
            _productRepository ??= new ProductRepository(_context);

        public IInventoryRepository Inventories =>
            _inventoryRepository ??= new InventoryRepository(_context);

        public IMovementRepository Movements =>
            _movementRepository ??= new MovementRepository(_context);

        public ITestRepository Tests =>
            _testRepository ??= new TestRepository(_context);

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            _currentTransaction = await _context.Database.BeginTransactionAsync();
            return _currentTransaction;
        }

        public async Task CommitTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.CommitAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public async Task RollbackTransactionAsync()
        {
            if (_currentTransaction != null)
            {
                await _currentTransaction.RollbackAsync();
                await _currentTransaction.DisposeAsync();
                _currentTransaction = null;
            }
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}

