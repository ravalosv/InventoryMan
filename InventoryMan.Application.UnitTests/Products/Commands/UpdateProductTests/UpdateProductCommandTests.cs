using Moq;
using FluentAssertions;
using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using System.Data.Entity.Infrastructure;
using InventoryMan.Application.Products.Commands.UpdateProduct;
using static InventoryMan.Application.Products.Commands.UpdateProduct.UpdateProductCommandHandler;

namespace InventoryMan.Application.UnitTests.Products.Commands.UpdateProductTests
{

    public class UpdateProductCommandTests
    {
        private readonly UpdateProductCommandHandler _handler;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public UpdateProductCommandTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _handler = new UpdateProductCommandHandler(_unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_ValidUpdate_ShouldUpdateSuccessfully()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var command = new UpdateProductCommand
            {
                Id = productId,
                Name = "Updated Product",
                Description = "Updated Description",
                CategoryId = 1,
                Price = 149.99m,
                Sku = "SKU-UPD"
            };

            var existingProduct = new Product
            {
                Id = productId,
                Name = "Original Product",
                Description = "Original Description",
                CategoryId = 2,
                Price = 99.99m,
                Sku = "SKU-OLD"
            };

            _unitOfWorkMock.Setup(uow => uow.Products.GetByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().BeTrue();

            existingProduct.Name.Should().Be(command.Name);
            existingProduct.Description.Should().Be(command.Description);
            existingProduct.CategoryId.Should().Be(command.CategoryId);
            existingProduct.Price.Should().Be(command.Price);
            existingProduct.Sku.Should().Be(command.Sku);

            _unitOfWorkMock.Verify(uow => uow.Products.UpdateAsync(existingProduct), Times.Once);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistentProduct_ShouldReturnFailure()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var command = new UpdateProductCommand
            {
                Id = productId,
                Name = "Updated Product",
                Description = "Updated Description",
                CategoryId = 1,
                Price = 149.99m,
                Sku = "SKU-UPD"
            };

            _unitOfWorkMock.Setup(uow => uow.Products.GetByIdAsync(productId))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Product not found");

            _unitOfWorkMock.Verify(uow => uow.Products.UpdateAsync(It.IsAny<Product>()), Times.Never);
            _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Handle_EmptyOrNullName_ValidationShouldFail(string invalidName)
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Id = Guid.NewGuid().ToString(),
                Name = invalidName,
                Description = "Description",
                CategoryId = 1,
                Price = 99.99m,
                Sku = "SKU123"
            };

            var validator = new UpdateProductCommandValidator();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Name));
            result.Errors.Should().Contain(x => x.ErrorMessage == "El nombre es requerido");
        }

        [Fact]
        public async Task Handle_NameExceeds100Characters_ValidationShouldFail()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Id = Guid.NewGuid().ToString(),
                Name = new string('A', 101),
                Description = "Description",
                CategoryId = 1,
                Price = 99.99m,
                Sku = "SKU123"
            };

            var validator = new UpdateProductCommandValidator();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Name));
            result.Errors.Should().Contain(x => x.ErrorMessage == "El nombre no puede exceder 100 caracteres");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-99.99)]
        public async Task Handle_InvalidPrice_ValidationShouldFail(decimal invalidPrice)
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Product",
                Description = "Description",
                CategoryId = 1,
                Price = invalidPrice,
                Sku = "SKU123"
            };

            var validator = new UpdateProductCommandValidator();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Price));
            result.Errors.Should().Contain(x => x.ErrorMessage == "El precio debe ser mayor a 0");
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Handle_EmptyOrNullSku_ValidationShouldFail(string invalidSku)
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Product",
                Description = "Description",
                CategoryId = 1,
                Price = 99.99m,
                Sku = invalidSku
            };

            var validator = new UpdateProductCommandValidator();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Sku));
            result.Errors.Should().Contain(x => x.ErrorMessage == "El SKU es requerido");
        }

        [Fact]
        public async Task Handle_SkuExceeds25Characters_ValidationShouldFail()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Product",
                Description = "Description",
                CategoryId = 1,
                Price = 99.99m,
                Sku = new string('A', 26)
            };

            var validator = new UpdateProductCommandValidator();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Sku));
            result.Errors.Should().Contain(x => x.ErrorMessage == "El SKU no puede exceder 25 caracteres");
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Handle_InvalidCategoryId_ValidationShouldFail(int invalidCategoryId)
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Id = Guid.NewGuid().ToString(),
                Name = "Test Product",
                Description = "Description",
                CategoryId = invalidCategoryId,
                Price = 99.99m,
                Sku = "SKU123"
            };

            var validator = new UpdateProductCommandValidator();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == nameof(command.CategoryId));
            result.Errors.Should().Contain(x => x.ErrorMessage == "Debe especificar una categoría válida");
        }

        [Fact]
        public async Task Handle_DatabaseError_ShouldReturnFailure()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var command = new UpdateProductCommand
            {
                Id = productId,
                Name = "Test Product",
                Description = "Description",
                CategoryId = 1,
                Price = 99.99m,
                Sku = "SKU123"
            };

            var existingProduct = new Product { Id = productId };

            _unitOfWorkMock.Setup(uow => uow.Products.GetByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync())
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("Error updating product");
            result.Error.Should().Contain("Database error");
        }

        [Fact]
        public async Task Handle_ConcurrencyConflict_ShouldReturnFailure()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var command = new UpdateProductCommand
            {
                Id = productId,
                Name = "Test Product",
                Description = "Description",
                CategoryId = 1,
                Price = 99.99m,
                Sku = "SKU123"
            };

            var existingProduct = new Product { Id = productId };

            _unitOfWorkMock.Setup(uow => uow.Products.GetByIdAsync(productId))
                .ReturnsAsync(existingProduct);

            _unitOfWorkMock.Setup(uow => uow.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateConcurrencyException("Concurrency conflict"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("Error updating product");
            result.Error.Should().Contain("Concurrency conflict");
        }

        [Fact]
        public async Task Handle_MultipleValidationErrors_ShouldReturnAllErrors()
        {
            // Arrange
            var command = new UpdateProductCommand
            {
                Id = Guid.NewGuid().ToString(),
                Name = "",
                Description = "Description",
                CategoryId = -1,
                Price = -10m,
                Sku = new string('A', 26)
            };

            var validator = new UpdateProductCommandValidator();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().HaveCount(4); // Errores para nombre, categoryId, price y sku
            result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Name));
            result.Errors.Should().Contain(x => x.PropertyName == nameof(command.CategoryId));
            result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Price));
            result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Sku));
        }

        [Fact]
        public async Task Handle_UpdateWithSameValues_ShouldUpdateSuccessfully()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var existingProduct = new Product
            {
                Id = productId,
                Name = "Test Product",
                Description = "Description",
                CategoryId = 1,
                Price = 99.99m,
                Sku = "SKU123"
            };

            var command = new UpdateProductCommand
            {
                Id = productId,
                Name = existingProduct.Name,
                Description = existingProduct.Description,
                CategoryId = existingProduct.CategoryId,
                Price = existingProduct.Price,
                Sku = existingProduct.Sku
            };

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

