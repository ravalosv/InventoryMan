using Moq;
using InventoryMan.Core.Interfaces;
using InventoryMan.Application.Inventory.Commands.UpdateStock;
using Microsoft.EntityFrameworkCore.Storage;
using static InventoryMan.Application.Inventory.Commands.UpdateStock.UpdateMinStockCommandHandler;

namespace InventoryMan.Application.UnitTests.Inventory.Commands.UpdateMinStockTests
{
    public class UpdateMinStockCommandHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IInventoryRepository> _mockInventoryRepository;
        private readonly Mock<IDbContextTransaction> _mockTransaction;
        private readonly UpdateMinStockCommandHandler _handler;

        public UpdateMinStockCommandHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockInventoryRepository = new Mock<IInventoryRepository>();
            _mockTransaction = new Mock<IDbContextTransaction>();

            _mockUnitOfWork
                .Setup(x => x.BeginTransactionAsync())
                .ReturnsAsync(_mockTransaction.Object);

            _mockUnitOfWork
                .Setup(x => x.Inventories)
                .Returns(_mockInventoryRepository.Object);

            _handler = new UpdateMinStockCommandHandler(_mockUnitOfWork.Object);
        }

        [Fact]
        public async Task Handle_WhenInventoryExists_ShouldUpdateMinStock()
        {
            // Arrange
            var command = new UpdateMinStockCommand
            {
                ProductId = "product1",
                StoreId = "store1",
                MinStock = 10
            };

            var existingInventory = new Core.Entities.Inventory
            {
                Id = "inv1",
                ProductId = command.ProductId,
                StoreId = command.StoreId,
                Quantity = 5,
                MinStock = 5
            };

            _mockInventoryRepository
                .Setup(x => x.GetByStoreIdAsync(command.StoreId))
                .ReturnsAsync(new List<Core.Entities.Inventory> { existingInventory });

            _mockInventoryRepository
                .Setup(x => x.UpdateAsync(It.IsAny<Core.Entities.Inventory>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);

            _mockInventoryRepository.Verify(x => x.UpdateAsync(
                It.Is<Core.Entities.Inventory>(i =>
                    i.Id == existingInventory.Id &&
                    i.MinStock == command.MinStock)),
                Times.Once);

            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
            _mockTransaction.Verify(x => x.CommitAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenInventoryDoesNotExist_ShouldCreateNewInventory()
        {
            // Arrange
            var command = new UpdateMinStockCommand
            {
                ProductId = "product1",
                StoreId = "store1",
                MinStock = 10
            };

            _mockInventoryRepository
                .Setup(x => x.GetByStoreIdAsync(command.StoreId))
                .ReturnsAsync(new List<Core.Entities.Inventory>());

            _mockInventoryRepository
                .Setup(x => x.AddAsync(It.IsAny<Core.Entities.Inventory>()))
                .ReturnsAsync((Core.Entities.Inventory inv) => inv);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.Data);

            _mockInventoryRepository.Verify(x => x.AddAsync(
                It.Is<Core.Entities.Inventory>(i =>
                    i.ProductId == command.ProductId &&
                    i.StoreId == command.StoreId &&
                    i.MinStock == command.MinStock &&
                    i.Quantity == 0)),
                Times.Once);

            _mockUnitOfWork.Verify(x => x.SaveChangesAsync(), Times.Once);
            _mockTransaction.Verify(x => x.CommitAsync(CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenExceptionOccurs_ShouldRollbackAndReturnFailure()
        {
            // Arrange
            var command = new UpdateMinStockCommand
            {
                ProductId = "product1",
                StoreId = "store1",
                MinStock = 10
            };

            _mockInventoryRepository
                .Setup(x => x.GetByStoreIdAsync(command.StoreId))
                .ThrowsAsync(new Exception("Test exception"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Error updating product min stock", result.Error);

            _mockTransaction.Verify(x => x.RollbackAsync(CancellationToken.None), Times.Once);
            _mockTransaction.Verify(x => x.CommitAsync(CancellationToken.None), Times.Never);
        }

        [Fact]
        public async Task Validator_WhenCommandIsValid_ShouldNotHaveValidationErrors()
        {
            // Arrange
            var command = new UpdateMinStockCommand
            {
                ProductId = "product1",
                StoreId = "store1",
                MinStock = 10
            };

            var validator = new UpdateMinStockCommandValidator();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            Assert.True(result.IsValid);
            Assert.Empty(result.Errors);
        }

        [Theory]
        [InlineData("", "store1", 10, "ProductId is required")]
        [InlineData("product1", "", 10, "StoreId is required")]
        [InlineData("product1", "store1", 0, "MinStock must be greater than 0")]
        [InlineData("product1", "store1", -1, "MinStock must be greater than 0")]
        public async Task Validator_WhenCommandIsInvalid_ShouldHaveValidationErrors(
            string productId, string storeId, int minStock, string expectedError)
        {
            // Arrange
            var command = new UpdateMinStockCommand
            {
                ProductId = productId,
                StoreId = storeId,
                MinStock = minStock
            };

            var validator = new UpdateMinStockCommandValidator();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.Contains(result.Errors, error => error.ErrorMessage == expectedError);
        }
    }
}

