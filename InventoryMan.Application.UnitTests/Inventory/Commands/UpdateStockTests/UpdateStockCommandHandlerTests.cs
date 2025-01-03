using Moq;
using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using InventoryMan.Application.Inventory.Commands.UpdateStock;
using Microsoft.EntityFrameworkCore.Storage;
using static InventoryMan.Application.Inventory.Commands.UpdateStock.UpdateStockCommandHandler;

namespace InventoryMan.Application.UnitTests.Inventory.Commands.UpdateStockTests
{
    public class UpdateStockCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IInventoryRepository> _mockInventoryRepository;
        private readonly Mock<IMovementRepository> _mockMovementRepository;
        private readonly Mock<IDbContextTransaction> _mockTransaction;
        private readonly UpdateStockCommandHandler _handler;

        public UpdateStockCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockInventoryRepository = new Mock<IInventoryRepository>();
            _mockMovementRepository = new Mock<IMovementRepository>();
            _mockTransaction = new Mock<IDbContextTransaction>();

            _mockUnitOfWork
                .Setup(x => x.BeginTransactionAsync())
                .ReturnsAsync(_mockTransaction.Object);

            _mockUnitOfWork
                .Setup(x => x.Inventories)
                .Returns(_mockInventoryRepository.Object);

            _mockUnitOfWork
                .Setup(x => x.Movements)
                .Returns(_mockMovementRepository.Object);

            _mockMovementRepository
                .Setup(x => x.AddAsync(It.IsAny<Movement>()))
                .ReturnsAsync((Movement m) => m);

            _handler = new UpdateStockCommandHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WhenInventoryExistsAndMovementTypeIn_ShouldIncreaseStock()
        {
            // Arrange
            var command = new UpdateStockCommand
            {
                ProductId = "product1",
                StoreId = "store1",
                Quantity = 10,
                MovementType = MovementType.IN
            };

            var existingInventory = new Core.Entities.Inventory
            {
                Id = "inv1",
                ProductId = command.ProductId,
                StoreId = command.StoreId,
                Quantity = 5,
                MinStock = 0
            };

            _mockInventoryRepository
                .Setup(x => x.GetByStoreIdAsync(command.StoreId))
                .ReturnsAsync(new List<Core.Entities.Inventory> { existingInventory });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);

            _mockInventoryRepository.Verify(x => x.UpdateAsync(
                It.Is<Core.Entities.Inventory>(i =>
                    i.Id == existingInventory.Id &&
                    i.Quantity == 15)), // 5 + 10
                Times.Once);

            _mockMovementRepository.Verify(x => x.AddAsync(
                It.Is<Movement>(m =>
                    m.ProductId == command.ProductId &&
                    m.Quantity == command.Quantity &&
                    m.Type == MovementType.IN &&
                    m.SourceStoreId == null &&
                    m.TargetStoreId == command.StoreId)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenInventoryExistsAndMovementTypeOut_ShouldDecreaseStock()
        {
            // Arrange
            var command = new UpdateStockCommand
            {
                ProductId = "product1",
                StoreId = "store1",
                Quantity = 3,
                MovementType = MovementType.OUT
            };

            var existingInventory = new Core.Entities.Inventory
            {
                Id = "inv1",
                ProductId = command.ProductId,
                StoreId = command.StoreId,
                Quantity = 5,
                MinStock = 0
            };

            _mockInventoryRepository
                .Setup(x => x.GetByStoreIdAsync(command.StoreId))
                .ReturnsAsync(new List<Core.Entities.Inventory> { existingInventory });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);

            _mockInventoryRepository.Verify(x => x.UpdateAsync(
                It.Is<Core.Entities.Inventory>(i =>
                    i.Id == existingInventory.Id &&
                    i.Quantity == 2)), // 5 - 3
                Times.Once);

            _mockMovementRepository.Verify(x => x.AddAsync(
                It.Is<Movement>(m =>
                    m.ProductId == command.ProductId &&
                    m.Quantity == command.Quantity &&
                    m.Type == MovementType.OUT &&
                    m.SourceStoreId == command.StoreId &&
                    m.TargetStoreId == null)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenInventoryDoesNotExistAndMovementTypeIn_ShouldCreateNewInventory()
        {
            // Arrange
            var command = new UpdateStockCommand
            {
                ProductId = "product1",
                StoreId = "store1",
                Quantity = 10,
                MovementType = MovementType.IN
            };

            _mockInventoryRepository
                .Setup(x => x.GetByStoreIdAsync(command.StoreId))
                .ReturnsAsync(new List<Core.Entities.Inventory>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);

            _mockInventoryRepository.Verify(x => x.AddAsync(
                It.Is<Core.Entities.Inventory>(i =>
                    i.ProductId == command.ProductId &&
                    i.StoreId == command.StoreId &&
                    i.Quantity == command.Quantity &&
                    i.MinStock == 0)),
                Times.Once);
        }

        [Fact]
        public async Task Handle_WhenInventoryDoesNotExistAndMovementTypeOut_ShouldThrowException()
        {
            // Arrange
            var command = new UpdateStockCommand
            {
                ProductId = "product1",
                StoreId = "store1",
                Quantity = 10,
                MovementType = MovementType.OUT
            };

            _mockInventoryRepository
                .Setup(x => x.GetByStoreIdAsync(command.StoreId))
                .ReturnsAsync(new List<Core.Entities.Inventory>());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("There is not enough inventory", result.Error);
        }

        [Fact]
        public async Task Handle_WhenOutQuantityExceedsInventory_ShouldThrowException()
        {
            // Arrange
            var command = new UpdateStockCommand
            {
                ProductId = "product1",
                StoreId = "store1",
                Quantity = 10,
                MovementType = MovementType.OUT
            };

            var existingInventory = new Core.Entities.Inventory
            {
                Id = "inv1",
                ProductId = command.ProductId,
                StoreId = command.StoreId,
                Quantity = 5,
                MinStock = 0
            };

            _mockInventoryRepository
                .Setup(x => x.GetByStoreIdAsync(command.StoreId))
                .ReturnsAsync(new List<Core.Entities.Inventory> { existingInventory });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("There is not enough inventory", result.Error);
        }

        [Fact]
        public async Task Validator_WhenCommandIsValid_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var command = new UpdateStockCommand
            {
                ProductId = "product1",
                StoreId = "store1",
                Quantity = 10,
                MovementType = MovementType.IN
            };

            var validator = new UpdateStockCommandValidator();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Theory]
        [InlineData("", "store1", 10, MovementType.IN, "ProductId is required")]
        [InlineData("product1", "", 10, MovementType.IN, "StoreId is required")]
        [InlineData("product1", "store1", 0, MovementType.IN, "Quantity must be greater than 0")]
        [InlineData("product1", "store1", -1, MovementType.IN, "Quantity must be greater than 0")]
        public async Task Validator_WhenCommandIsInvalid_ShouldHaveValidationErrors(
            string productId, string storeId, int quantity, MovementType movementType, string expectedError)
        {
            // Arrange
            var command = new UpdateStockCommand
            {
                ProductId = productId,
                StoreId = storeId,
                Quantity = quantity,
                MovementType = movementType
            };

            var validator = new UpdateStockCommandValidator();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.ErrorMessage == expectedError);
        }
    }

}

