using Microsoft.EntityFrameworkCore;
using InventoryMan.Core.Entities;
using InventoryMan.Infrastructure.Data.Context;
using InventoryMan.Infrastructure.Repositories;

namespace InventoryMan.Infrastructure.UnitTests.Repositories
{
    public class InventoryRepositoryTests : IDisposable
    {
        private readonly DbContextOptions<InventoryDbContext> _options;
        private readonly InventoryDbContext _context;
        private readonly InventoryRepository _repository;

        public InventoryRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new InventoryDbContext(_options);
            _repository = new InventoryRepository(_context);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnInventoryWithProduct()
        {
            // Arrange
            var product = new Product { Id = "prod-001", Name = "Test Product", Description = "Product Description", Sku = "Sku-001" };
            var inventory = new Inventory
            {
                Id = "inv-001",
                ProductId = product.Id,
                StoreId = "store-001",
                Quantity = 100,
                MinStock = 10,
                Product = product
            };

            await _context.Products.AddAsync(product);
            await _context.Inventories.AddAsync(inventory);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync("inv-001");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("inv-001", result.Id);
            Assert.NotNull(result.Product);
            Assert.Equal("Test Product", result.Product.Name);
        }

        [Fact]
        public async Task GetByStoreIdAsync_ShouldReturnInventoriesForStore()
        {
            // Arrange
            var product1 = new Product { Id = "prod-001", Name = "Product 1", Description = "Product Description 01", Sku = "Sku-001" };
            var product2 = new Product { Id = "prod-002", Name = "Product 2", Description = "Product Description 02", Sku = "Sku-002" };

            var inventories = new List<Inventory>
        {
            new Inventory
            {
                Id = "inv-001",
                ProductId = product1.Id,
                StoreId = "store-001",
                Product = product1
            },
            new Inventory
            {
                Id = "inv-002",
                ProductId = product2.Id,
                StoreId = "store-001",
                Product = product2
            },
            new Inventory
            {
                Id = "inv-003",
                ProductId = product1.Id,
                StoreId = "store-002",
                Product = product1
            }
        };

            await _context.Products.AddRangeAsync(product1, product2);
            await _context.Inventories.AddRangeAsync(inventories);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByStoreIdAsync("store-001");

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, inv => Assert.Equal("store-001", inv.StoreId));
        }

        [Fact]
        public async Task GetLowStockItemsAsync_ShouldReturnItemsBelowMinStock()
        {
            // Arrange
            var product = new Product {Id = "prod-001", Name = "Test Product", Description = "Product Description", Sku = "Sku-001" };
            var inventories = new List<Inventory>
        {
            new Inventory
            {
                Id = "inv-001",
                ProductId = product.Id,
                StoreId = "store-001",
                Quantity = 5,
                MinStock = 10,
                Product = product
            },
            new Inventory
            {
                Id = "inv-002",
                ProductId = product.Id,
                StoreId = "store-001",
                Quantity = 15,
                MinStock = 10,
                Product = product
            }
        };

            await _context.Products.AddAsync(product);
            await _context.Inventories.AddRangeAsync(inventories);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetLowStockItemsAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("inv-001", result.First().Id);
        }

        [Fact]
        public async Task AddAsync_ShouldAddNewInventory()
        {
            // Arrange
            var product = new Product { Id = "prod-001", Name = "Test Product" };
            var inventory = new Inventory
            {
                Id = "inv-001",
                ProductId = product.Id,
                StoreId = "store-001",
                Quantity = 100,
                MinStock = 10
            };

            // Act
            await _repository.AddAsync(inventory);
            await _context.SaveChangesAsync();

            // Assert
            var savedInventory = await _context.Inventories.FindAsync("inv-001");
            Assert.NotNull(savedInventory);
            Assert.Equal(100, savedInventory.Quantity);
        }

        [Fact]
        public async Task UpdateAsync_ShouldUpdateExistingInventory()
        {
            // Arrange
            var product = new Product {Id = "prod-001", Name = "Test Product", Description = "Product Description", Sku = "Sku-001" };
            var inventory = new Inventory
            {
                Id = "inv-001",
                StoreId = "store-001",
                ProductId = product.Id,
                Quantity = 100
            };

            await _context.Products.AddAsync(product);
            await _context.Inventories.AddAsync(inventory);
            await _context.SaveChangesAsync();

            // Act
            inventory.Quantity = 150;
            await _repository.UpdateAsync(inventory);
            await _context.SaveChangesAsync();

            // Assert
            var updatedInventory = await _context.Inventories.FindAsync("inv-001");
            Assert.NotNull(updatedInventory);
            Assert.Equal(150, updatedInventory.Quantity);
        }


        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }
}
