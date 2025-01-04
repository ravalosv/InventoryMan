using Moq;
using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using InventoryMan.Application.Inventory.Queries.GetInventoryByStore;

namespace InventoryMan.Application.UnitTests.Inventory.Queries.GetInventoryTests
{
    public class GetInventoryByStoreQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IInventoryRepository> _mockInventoryRepository;
        private readonly Mock<IStoreRepository> _mockStoreRepository;
        private readonly GetInventoryByStoreQueryHandler _handler;

        public GetInventoryByStoreQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockInventoryRepository = new Mock<IInventoryRepository>();
            _mockStoreRepository = new Mock<IStoreRepository>();

            _mockUnitOfWork
                .Setup(x => x.Inventories)
                .Returns(_mockInventoryRepository.Object);

            _mockUnitOfWork .Setup(x => x.Stores)
                .Returns(_mockStoreRepository.Object);

            _handler = new GetInventoryByStoreQueryHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WhenInventoriesExist_ShouldReturnInventoryDtos()
        {
            // Arrange
            var storeId = "store1";
            var query = new GetInventoryByStoreQuery(storeId);

            var store = new Store { Id = storeId, Name = "Store 1" };

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync(store);

            var inventories = new List<Core.Entities.Inventory>
        {
            new Core.Entities.Inventory
            {
                Id = "inv1",
                ProductId = "prod1",
                Product = new Product { Name = "Product 1" },
                StoreId = storeId,
                Quantity = 10,
                MinStock = 5
            },
            new Core.Entities.Inventory
            {
                Id = "inv2",
                ProductId = "prod2",
                Product = new Product { Name = "Product 2" },
                StoreId = storeId,
                Quantity = 20,
                MinStock = 8
            }
        };

            _mockInventoryRepository
                .Setup(x => x.GetByStoreIdAsync(storeId))
                .ReturnsAsync(inventories);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);

            var inventoryDtos = result.Data.ToList();
            Assert.Equal(2, inventoryDtos.Count);

            // Verificar primer inventario
            Assert.Equal("inv1", inventoryDtos[0].Id);
            Assert.Equal("prod1", inventoryDtos[0].ProductId);
            Assert.Equal("Product 1", inventoryDtos[0].ProductName);
            Assert.Equal(storeId, inventoryDtos[0].StoreId);
            Assert.Equal(10, inventoryDtos[0].Quantity);
            Assert.Equal(5, inventoryDtos[0].MinStock);

            // Verificar segundo inventario
            Assert.Equal("inv2", inventoryDtos[1].Id);
            Assert.Equal("prod2", inventoryDtos[1].ProductId);
            Assert.Equal("Product 2", inventoryDtos[1].ProductName);
            Assert.Equal(storeId, inventoryDtos[1].StoreId);
            Assert.Equal(20, inventoryDtos[1].Quantity);
            Assert.Equal(8, inventoryDtos[1].MinStock);
        }

        [Fact]
        public async Task Handle_WhenNoInventoriesExist_ShouldReturnEmptyList()
        {
            // Arrange
            var storeId = "store1";
            var query = new GetInventoryByStoreQuery(storeId);


            var store = new Store { Id = storeId, Name = "Store 1" };

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync(store);



            _mockInventoryRepository
                .Setup(x => x.GetByStoreIdAsync(storeId))
                .ReturnsAsync(new List<Core.Entities.Inventory>());

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Empty(result.Data);
        }

        [Fact]
        public async Task Handle_WhenInventoryWithoutProduct_ShouldReturnUnknownProductName()
        {
            // Arrange
            var storeId = "store1";
            var query = new GetInventoryByStoreQuery(storeId);


            var store = new Store { Id = storeId, Name = "Store 1" };

            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync(store);



            var inventories = new List<Core.Entities.Inventory>
        {
            new Core.Entities.Inventory
            {
                Id = "inv1",
                ProductId = "prod1",
                Product = null, // Producto no existe
                StoreId = storeId,
                Quantity = 10,
                MinStock = 5
            }
        };

            _mockInventoryRepository
                .Setup(x => x.GetByStoreIdAsync(storeId))
                .ReturnsAsync(inventories);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);

            var inventoryDto = result.Data.First();
            Assert.Equal("Unknown", inventoryDto.ProductName);
        }

        [Fact]
        public async Task Handle_WhenStoreDoesNotExists_ShouldReturnError()
        {
            // Arrange
            var storeId = "InexistentStoreId";
            var query = new GetInventoryByStoreQuery(storeId);


            _mockStoreRepository
                .Setup(x => x.GetByIdAsync(storeId))
                .ReturnsAsync((Store)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Null(result.Data); 
            Assert.Contains($"Store with ID {storeId} not found", result.Error); // Corrección: verificar mensaje de error


        }
    }


}

