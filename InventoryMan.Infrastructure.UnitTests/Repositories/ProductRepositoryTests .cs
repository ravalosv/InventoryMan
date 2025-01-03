using Microsoft.EntityFrameworkCore;
using InventoryMan.Core.Entities;
using InventoryMan.Infrastructure.Data.Context;
using InventoryMan.Infrastructure.Repositories;

namespace InventoryMan.Infrastructure.UnitTests.Repositories
{

    public class ProductRepositoryTests : IDisposable
    {
        private readonly DbContextOptions<InventoryDbContext> _options;
        private readonly InventoryDbContext _context;
        private readonly ProductRepository _repository;

        public ProductRepositoryTests()
        {
            _options = new DbContextOptionsBuilder<InventoryDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _context = new InventoryDbContext(_options);
            _repository = new ProductRepository(_context);
        }

        [Fact]
        public async Task GetByIdAsync_ShouldReturnProductWithCategory()
        {
            // Arrange
            var category = new ProductCategory { Id = 1, Name = "Test Category" };
            var product = new Product
            {
                Id = "prod-001",
                Name = "Test Product",
                CategoryId = category.Id,
                Category = category,
                Description = "Product Description",
                Sku = "Sku-001"
            };

            await _context.ProductCategories.AddAsync(category);
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByIdAsync("prod-001");

            // Assert
            Assert.NotNull(result);
            Assert.Equal("prod-001", result.Id);
            Assert.NotNull(result.Category);
            Assert.Equal("Test Category", result.Category.Name);
        }

        [Fact]
        public async Task GetByCategoryAsync_ShouldReturnProductsInCategory()
        {
            // Arrange
            var category1 = new ProductCategory { Id = 1, Name = "Category 1" };
            var category2 = new ProductCategory { Id = 2, Name = "Category 2" };

            var products = new List<Product>
        {
            new Product
            {
                Id = "prod-001",
                Name = "Product 1",
                CategoryId = category1.Id,
                Description = "Product Description", 
                Sku = "Sku-001"
            },
            new Product
            {
                Id = "prod-002",
                Name = "Product 2",
                CategoryId = category1.Id,
                Description = "Product Description", 
                Sku = "Sku-002"
            },
            new Product
            {
                Id = "prod-003",
                Name = "Product 3",
                CategoryId = category2.Id,
                Description = "Product Description", 
                Sku = "Sku-003"
            }
        };

            await _context.ProductCategories.AddRangeAsync(category1, category2);
            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByCategoryAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            Assert.All(result, prod => Assert.Equal(1, prod.CategoryId));
        }

        [Fact]
        public async Task GetByPriceRangeAsync_ShouldReturnProductsInPriceRange()
        {
            // Arrange
            var products = new List<Product>
        {
            new Product
            {
                Id = "prod-001",
                Name = "Cheap Product",
                Price = 10.00m,
                Description = "Product Description", 
                Sku = "Sku-001"
            },
            new Product
            {
                Id = "prod-002",
                Name = "Medium Product",
                Price = 50.00m,
                Description = "Product Description",
                Sku = "Sku-002"
            },
            new Product
            {
                Id = "prod-003",
                Name = "Expensive Product",
                Price = 100.00m,
                Description = "Product Description",
                Sku = "Sku-003"
            }
        };

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetByPriceRangeAsync(20.00m, 80.00m);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("prod-002", result.First().Id);
            Assert.Equal(50.00m, result.First().Price);
        }

        [Fact]
        public async Task GetAllAsync_ShouldReturnAllProducts()
        {
            // Arrange
            var products = new List<Product>
        {
            new Product { Id = "prod-001", Name = "Product 1", Description = "Product Description", Sku = "Sku-001" },
            new Product { Id = "prod-002", Name = "Product 2", Description = "Product Description", Sku = "Sku-002"},
            new Product { Id = "prod-003", Name = "Product 3", Description = "Product Description", Sku = "Sku-003" }
        };

            await _context.Products.AddRangeAsync(products);
            await _context.SaveChangesAsync();

            // Act
            var result = await _repository.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Count());
        }

        public void Dispose()
        {
            _context.Database.EnsureDeleted();
            _context.Dispose();
        }
    }


}
