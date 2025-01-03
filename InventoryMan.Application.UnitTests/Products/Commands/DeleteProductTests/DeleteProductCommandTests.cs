using Moq;
using FluentAssertions;
using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using System.Data.Entity.Infrastructure;
using InventoryMan.Application.Products.Commands.DeleteProduct;
using static InventoryMan.Application.Products.Commands.DeleteProduct.DeleteProductCommandHandler;

namespace InventoryMan.Application.UnitTests.Products.Commands.DeleteProductTests
{
    public class DeleteProductCommandTests
    {
        private readonly DeleteProductCommandHandler _handler;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public DeleteProductCommandTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new DeleteProductCommandHandler(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ValidId_ShouldDeleteSuccessfully()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var command = new DeleteProductCommand { Id = productId };
            var existingProduct = new Product { Id = productId, Name = "Test Product" };

            _unitOfWorkMock.Setup(uow => uow.Products.GetByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            _unitOfWorkMock.Setup(uow => uow.Products.DeleteAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeTrue();

            _unitOfWorkMock.Verify(uow => uow.Products.GetByIdAsync(productId), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Products.DeleteAsync(existingProduct), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistentId_ShouldReturnFailure()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid().ToString();
            var command = new DeleteProductCommand { Id = nonExistentId };

            _unitOfWorkMock.Setup(uow => uow.Products.GetByIdAsync(nonExistentId))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be($"Product with id {nonExistentId} not found");

            _unitOfWorkMock.Verify(uow => uow.Products.GetByIdAsync(nonExistentId), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.Products.DeleteAsync(It.IsAny<Product>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Handle_EmptyOrNullId_ValidationShouldFail(string invalidId)
        {
            // Arrange
            var command = new DeleteProductCommand { Id = invalidId };
            var validator = new DeleteProductCommandValidator();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Id));
            result.Errors.Should().Contain(x => x.ErrorMessage == "Id is required");
        }

        [Fact]
        public async Task Handle_DatabaseError_ShouldReturnFailure()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var command = new DeleteProductCommand { Id = productId };
            var existingProduct = new Product { Id = productId, Name = "Test Product" };

            _unitOfWorkMock.Setup(uow => uow.Products.GetByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            _unitOfWorkMock.Setup(uow => uow.Products.DeleteAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("Error deleting product");
            result.Error.Should().Contain("Database error");
        }

        [Fact]
        public async Task Handle_DeleteOperationError_ShouldReturnFailure()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var command = new DeleteProductCommand { Id = productId };
            var existingProduct = new Product { Id = productId, Name = "Test Product" };

            _unitOfWorkMock.Setup(uow => uow.Products.GetByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            _unitOfWorkMock.Setup(uow => uow.Products.DeleteAsync(It.IsAny<Product>()))
                .ThrowsAsync(new Exception("Delete operation failed"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("Error deleting product");
            result.Error.Should().Contain("Delete operation failed");
        }

        [Fact]
        public async Task Handle_GetByIdThrowsException_ShouldReturnFailure()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var command = new DeleteProductCommand { Id = productId };

            _unitOfWorkMock.Setup(uow => uow.Products.GetByIdAsync(productId))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("Error deleting product");
            result.Error.Should().Contain("Database connection error");
        }

        [Fact]
        public async Task Handle_ConcurrencyConflict_ShouldReturnFailure()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var command = new DeleteProductCommand { Id = productId };
            var existingProduct = new Product { Id = productId, Name = "Test Product" };

            _unitOfWorkMock.Setup(uow => uow.Products.GetByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            _unitOfWorkMock.Setup(uow => uow.Products.DeleteAsync(It.IsAny<Product>()))
                .Returns(Task.CompletedTask);

            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateConcurrencyException("Concurrency conflict"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("Error deleting product");
            result.Error.Should().Contain("Concurrency conflict");
        }

        [Fact]
        public async Task Handle_ValidGuidId_ShouldDeleteSuccessfully()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var command = new DeleteProductCommand { Id = productId };
            var existingProduct = new Product { Id = productId, Name = "Test Product" };

            _unitOfWorkMock.Setup(uow => uow.Products.GetByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeTrue();
        }
    }


}

