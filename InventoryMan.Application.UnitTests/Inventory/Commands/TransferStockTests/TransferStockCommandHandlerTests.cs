using Moq;
using System.Data;
using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using InventoryMan.Application.Inventory.Commands.UpdateStock;
using Microsoft.EntityFrameworkCore.Storage;

namespace InventoryMan.Application.UnitTests.Inventory.Commands.TransferStockTests
{

    public class TransferStockCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly TransferStockCommandHandler _handler;
        private readonly Mock<IDbContextTransaction> _mockTransaction;
        private readonly Mock<IMovementRepository> _mockMovementRepository;
        private readonly Mock<IInventoryRepository> _mockInventoryRepository;


        public TransferStockCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockTransaction = new Mock<IDbContextTransaction>();
            _mockMovementRepository = new Mock<IMovementRepository>();
            _mockInventoryRepository = new Mock<IInventoryRepository>();

            // Setup UnitOfWork para devolver los repositorios mock
            _mockUnitOfWork
                .Setup(x => x.BeginTransactionAsync())
                .ReturnsAsync(_mockTransaction.Object);

            _mockUnitOfWork
                .Setup(x => x.Movements)
                .Returns(_mockMovementRepository.Object);

            _mockUnitOfWork
                .Setup(x => x.Inventories)
                .Returns(_mockInventoryRepository.Object);

            _handler = new TransferStockCommandHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WithValidRequest_ShouldTransferStock()
        {
            // Arrange
            var command = new TransferStockCommand
            {
                ProductId = "product1",
                SourceStoreId = "store1",
                TargetStoreId = "store2",
                Quantity = 5
            };

            var sourceInventory = new Core.Entities.Inventory
            {
                Id = "inv1",
                ProductId = command.ProductId,
                StoreId = command.SourceStoreId,
                Quantity = 10
            };

            var targetInventory = new Core.Entities.Inventory
            {
                Id = "inv2",
                ProductId = command.ProductId,
                StoreId = command.TargetStoreId,
                Quantity = 3
            };

            // Setup del repositorio de inventarios
            _mockInventoryRepository
                .Setup(x => x.GetByStoreIdAsync(command.SourceStoreId))
                .ReturnsAsync(new List<Core.Entities.Inventory> { sourceInventory });

            _mockInventoryRepository
                .Setup(x => x.GetByStoreIdAsync(command.TargetStoreId))
                .ReturnsAsync(new List<Core.Entities.Inventory> { targetInventory });

            // Setup para las operaciones de actualización y creación
            _mockInventoryRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Core.Entities.Inventory>()))
                .Returns(Task.CompletedTask);

            _mockMovementRepository
            .Setup(x => x.AddAsync(It.IsAny<Movement>()))
            .ReturnsAsync((Movement m) => m);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);

            // Verify source inventory was updated
            _mockInventoryRepository.Verify(x => x.UpdateAsync(
                It.Is<Core.Entities.Inventory>(i => i.Id == "inv1" && i.Quantity == 5)),
                Times.Once);

            // Verify target inventory was updated
            _mockInventoryRepository.Verify(x => x.UpdateAsync(
                It.Is<Core.Entities.Inventory>(i => i.Id == "inv2" && i.Quantity == 8)),
                Times.Once);

            // Verify movement was created
            _mockMovementRepository.Verify(x => x.AddAsync(
                It.Is<Movement>(m =>
                    m.ProductId == command.ProductId &&
                    m.Quantity == command.Quantity &&
                    m.Type == MovementType.TRANSFER &&
                    m.SourceStoreId == command.SourceStoreId &&
                    m.TargetStoreId == command.TargetStoreId)),
                Times.Once);

            // Verify transaction was committed
            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
            _mockTransaction.Verify(x => x.CommitAsync(CancellationToken.None), Times.Once);

        }

        [Fact]
        public async Task Handle_WithNonExistentSourceInventory_ShouldReturnFailure()
        {
            // Arrange
            var command = new TransferStockCommand
            {
                ProductId = "product1",
                SourceStoreId = "store1",
                TargetStoreId = "store2",
                Quantity = 5
            };

            _mockUnitOfWork
                .Setup(x => x.Inventories.GetByStoreIdAsync(command.SourceStoreId))
                .ReturnsAsync(new List<Core.Entities.Inventory>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("There is not enough inventory", result.Error);

            // Verify transaction was rolled back
            _mockTransaction.Verify(x => x.RollbackAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_WithInsufficientSourceInventory_ShouldReturnFailure()
        {
            // Arrange
            var command = new TransferStockCommand
            {
                ProductId = "product1",
                SourceStoreId = "store1",
                TargetStoreId = "store2",
                Quantity = 10
            };

            var sourceInventory = new Core.Entities.Inventory
            {
                Id = "inv1",
                ProductId = command.ProductId,
                StoreId = command.SourceStoreId,
                Quantity = 5  // Less than requested quantity
            };

            _mockUnitOfWork
                .Setup(x => x.Inventories.GetByStoreIdAsync(command.SourceStoreId))
                .ReturnsAsync(new List<Core.Entities.Inventory> { sourceInventory });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("There is not enough inventory", result.Error);

            // Verify transaction was rolled back
            _mockTransaction.Verify(x => x.RollbackAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_WithNonExistentTargetInventory_ShouldCreateNewInventory()
        {
            // Arrange
            var command = new TransferStockCommand
            {
                ProductId = "product1",
                SourceStoreId = "store1",
                TargetStoreId = "store2",
                Quantity = 5
            };

            var sourceInventory = new Core.Entities.Inventory
            {
                Id = "inv1",
                ProductId = command.ProductId,
                StoreId = command.SourceStoreId,
                Quantity = 10
            };

            _mockUnitOfWork
                .Setup(x => x.Inventories.GetByStoreIdAsync(command.SourceStoreId))
                .ReturnsAsync(new List<Core.Entities.Inventory> { sourceInventory });

            _mockUnitOfWork
                .Setup(x => x.Inventories.GetByStoreIdAsync(command.TargetStoreId))
                .ReturnsAsync(new List<Core.Entities.Inventory>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);

            // Verify new inventory was created
            _mockUnitOfWork.Verify(x => x.Inventories.AddAsync(
                It.Is<Core.Entities.Inventory>(i =>
                    i.ProductId == command.ProductId &&
                    i.StoreId == command.TargetStoreId &&
                    i.Quantity == command.Quantity)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WithDatabaseError_ShouldRollbackAndReturnFailure()
        {
            // Arrange
            var command = new TransferStockCommand
            {
                ProductId = "product1",
                SourceStoreId = "store1",
                TargetStoreId = "store2",
                Quantity = 5
            };

            var sourceInventory = new Core.Entities.Inventory
            {
                Id = "inv1",
                ProductId = command.ProductId,
                StoreId = command.SourceStoreId,
                Quantity = 10
            };

            _mockUnitOfWork
                .Setup(x => x.Inventories.GetByStoreIdAsync(command.SourceStoreId))
                .ReturnsAsync(new List<Core.Entities.Inventory> { sourceInventory });

            _mockUnitOfWork
                .Setup(x => x.SaveChangesAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Error updating product stock", result.Error);

            // Verify transaction was rolled back
            _mockTransaction.Verify(x => x.RollbackAsync(CancellationToken.None), Times.Once);
        }
    }
}

