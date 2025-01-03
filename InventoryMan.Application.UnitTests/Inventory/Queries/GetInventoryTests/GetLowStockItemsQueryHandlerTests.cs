using Moq;
using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using InventoryMan.Application.Inventory.Queries.GetLowStockItems;

namespace InventoryMan.Application.UnitTests.Inventory.Queries.GetInventoryTests
{
    public class GetLowStockItemsQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IInventoryRepository> _mockInventoryRepository;
        private readonly GetLowStockItemsQueryHandler _handler;

        public GetLowStockItemsQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockInventoryRepository = new Mock<IInventoryRepository>();

            _mockUnitOfWork
                .Setup(x => x.Inventories)
                .Returns(_mockInventoryRepository.Object);

            _handler = new GetLowStockItemsQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WhenLowStockItemsExist_ShouldReturnInventoryDtos()
        {
            // Arrange
            var lowStockItems = new List<Core.Entities.Inventory>
        {
            new Core.Entities.Inventory
            {
                Id = "inv1",
                ProductId = "prod1",
                Product = new Product { Name = "Product 1" },
                StoreId = "store1",
                Quantity = 3,
                MinStock = 5
            },
            new Core.Entities.Inventory
            {
                Id = "inv2",
                ProductId = "prod2",
                Product = new Product { Name = "Product 2" },
                StoreId = "store2",
                Quantity = 2,
                MinStock = 10
            }
        };

            _mockInventoryRepository
                .Setup(x => x.GetLowStockItemsAsync())
                .ReturnsAsync(lowStockItems);

            // Act
            var result = await _handler.Handle(new GetLowStockItemsQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);

            var inventoryDtos = result.Data.ToList();
            Assert.Equal(2, inventoryDtos.Count);

            // Verificar primer item con bajo stock
            Assert.Equal("inv1", inventoryDtos[0].Id);
            Assert.Equal("prod1", inventoryDtos[0].ProductId);
            Assert.Equal("Product 1", inventoryDtos[0].ProductName);
            Assert.Equal("store1", inventoryDtos[0].StoreId);
            Assert.Equal(3, inventoryDtos[0].Quantity);
            Assert.Equal(5, inventoryDtos[0].MinStock);

            // Verificar segundo item con bajo stock
            Assert.Equal("inv2", inventoryDtos[1].Id);
            Assert.Equal("prod2", inventoryDtos[1].ProductId);
            Assert.Equal("Product 2", inventoryDtos[1].ProductName);
            Assert.Equal("store2", inventoryDtos[1].StoreId);
            Assert.Equal(2, inventoryDtos[1].Quantity);
            Assert.Equal(10, inventoryDtos[1].MinStock);
        }

        [Fact]
        public async Task Handle_WhenNoLowStockItems_ShouldReturnEmptyList()
        {
            // Arrange
            _mockInventoryRepository
                .Setup(x => x.GetLowStockItemsAsync())
                .ReturnsAsync(new List<Core.Entities.Inventory>());

            // Act
            var result = await _handler.Handle(new GetLowStockItemsQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task Handle_WhenLowStockItemWithoutProduct_ShouldReturnUnknownProductName()
        {
            // Arrange
            var lowStockItems = new List<Core.Entities.Inventory>
        {
            new Core.Entities.Inventory
            {
                Id = "inv1",
                ProductId = "prod1",
                Product = null, // Producto no existe
                StoreId = "store1",
                Quantity = 3,
                MinStock = 5
            }
        };

            _mockInventoryRepository
                .Setup(x => x.GetLowStockItemsAsync())
                .ReturnsAsync(lowStockItems);

            // Act
            var result = await _handler.Handle(new GetLowStockItemsQuery(), CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);

            var inventoryDto = result.Data.First();
            Assert.Equal("Unknown", inventoryDto.ProductName);
            Assert.Equal(3, inventoryDto.Quantity);
            Assert.Equal(5, inventoryDto.MinStock);
        }

        [Fact]
        public async Task Handle_ShouldCallGetLowStockItemsAsyncOnce()
        {
            // Arrange
            _mockInventoryRepository
                .Setup(x => x.GetLowStockItemsAsync())
                .ReturnsAsync(new List<Core.Entities.Inventory>());

            // Act
            await _handler.Handle(new GetLowStockItemsQuery(), CancellationToken.None);

            // Assert
            _mockInventoryRepository.Verify(x => x.GetLowStockItemsAsync(), Times.Once);
        }
    }


}

