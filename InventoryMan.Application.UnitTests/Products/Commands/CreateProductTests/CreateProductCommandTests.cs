using FluentAssertions;
using InventoryMan.Application.Products.Commands.CreateProduct;
using InventoryMan.Application.UnitTests.Common;
using InventoryMan.Core.Entities;
using Moq;
using System.Data.Entity.Infrastructure;
using static InventoryMan.Application.Products.Commands.CreateProduct.CreateProductCommandHandler;

namespace InventoryMan.Application.UnitTests.Products.Commands.CreateProductTests
{
    public class CreateProductCommandTests : TestBase
    {
        private readonly CreateProductCommandHandler _handler;
        private readonly CreateProductCommandHandler.CreateProductCommandValidator _validator;

        public CreateProductCommandTests()
        {
            // Configurar el handler con el mock
            _handler = new CreateProductCommandHandler(UnitOfWorkMock.Object);
            _validator = new CreateProductCommandHandler.CreateProductCommandValidator();
        }

        [Fact]
        public async Task Handle_ValidProduct_ShouldCreateSuccessfully()
        {
            // Arrange
            var command = new CreateProductCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = 1,
                Price = 99.99m,
                Sku = "SKU123"
            };

            Product? capturedProduct = null;

            UnitOfWorkMock.Setup(uow => uow.Products.AddAsync(It.IsAny<Product>()))
                .Callback<Product>(p => capturedProduct = p)
                .ReturnsAsync((Product p) => p);

            UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNullOrEmpty();

            // Verificar que el producto fue capturado correctamente
            capturedProduct.Should().NotBeNull();
            capturedProduct!.Name.Should().Be(command.Name);
            capturedProduct.Description.Should().Be(command.Description);
            capturedProduct.CategoryId.Should().Be(command.CategoryId);
            capturedProduct.Price.Should().Be(command.Price);
            capturedProduct.Sku.Should().Be(command.Sku);

            // Verificar que los métodos fueron llamados
            UnitOfWorkMock.Verify(uow => uow.Products.AddAsync(It.IsAny<Product>()), Times.Once);
            UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task Handle_MinimalValidValues_ShouldCreateSuccessfully()
        {
            // Arrange
            var command = new CreateProductCommand
            {
                Name = "X",
                Description = "",
                CategoryId = 1,
                Price = 0.01m,
                Sku = "SKU1"
            };

            Product? capturedProduct = null;

            // Configuramos el mock para capturar el producto y verificar valores mínimos
            UnitOfWorkMock.Setup(uow => uow.Products.AddAsync(It.IsAny<Product>()))
                .Callback<Product>(p => capturedProduct = p)
                .ReturnsAsync((Product p) => p);

            UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNullOrEmpty();

            // Verificamos que el producto fue capturado correctamente
            capturedProduct.Should().NotBeNull();

            // Verificamos que los valores mínimos se guardaron correctamente
            capturedProduct!.Name.Should().Be("X");
            capturedProduct.Description.Should().Be("");
            capturedProduct.CategoryId.Should().Be(1);
            capturedProduct.Price.Should().Be(0.01m);
            capturedProduct.Sku.Should().Be("SKU1");

            // Verificamos que se generó un ID válido
            Guid.TryParse(capturedProduct.Id, out _).Should().BeTrue();

            // Verificamos que el mock fue llamado
            UnitOfWorkMock.Verify(uow => uow.Products.AddAsync(It.IsAny<Product>()), Times.Once);
            UnitOfWorkMock.Verify(uow => uow.SaveChangesAsync(), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Handle_EmptyOrNullName_ShouldFail(string invalidName)
        {
            // Arrange
            var command = new CreateProductCommand
            {
                Name = invalidName,
                Description = "Test Description",
                CategoryId = 1,
                Price = 99.99m,
                Sku = "SKU123"
            };

            var validator = new CreateProductCommandValidator();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Name));
            result.Errors.Should().Contain(x => x.ErrorMessage == "El nombre es requerido");
        }

        [Fact]
        public async Task Handle_NameExceeds100Characters_ShouldFail()
        {
            // Arrange
            var command = new CreateProductCommand
            {
                Name = new string('A', 101),
                Description = "Test Description",
                CategoryId = 1,
                Price = 99.99m,
                Sku = "SKU123"
            };

            var validator = new CreateProductCommandValidator();

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
        public async Task Handle_InvalidPrice_ShouldFail(decimal invalidPrice)
        {
            // Arrange
            var command = new CreateProductCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = 1,
                Price = invalidPrice,
                Sku = "SKU123"
            };

            var validator = new CreateProductCommandValidator();

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
        public async Task Handle_EmptyOrNullSku_ShouldFail(string invalidSku)
        {
            // Arrange
            var command = new CreateProductCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = 1,
                Price = 99.99m,
                Sku = invalidSku
            };

            var validator = new CreateProductCommandValidator();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == nameof(command.Sku));
            result.Errors.Should().Contain(x => x.ErrorMessage == "El SKU es requerido");
        }

        [Fact]
        public async Task Handle_SkuExceeds25Characters_ShouldFail()
        {
            // Arrange
            var command = new CreateProductCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = 1,
                Price = 99.99m,
                Sku = new string('A', 26)
            };

            var validator = new CreateProductCommandValidator();

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
        public async Task Handle_InvalidCategoryId_ShouldFail(int invalidCategoryId)
        {
            // Arrange
            var command = new CreateProductCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = invalidCategoryId,
                Price = 99.99m,
                Sku = "SKU123"
            };

            var validator = new CreateProductCommandValidator();

            // Act
            var result = await validator.ValidateAsync(command);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Errors.Should().Contain(x => x.PropertyName == nameof(command.CategoryId));
            result.Errors.Should().Contain(x => x.ErrorMessage == "Debe especificar una categoría válida");
        }

        [Fact]
        public async Task Handle_WhenDatabaseThrowsException_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new CreateProductCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = 1,
                Price = 99.99m,
                Sku = "SKU123"
            };

            UnitOfWorkMock.Setup(uow => uow.Products.AddAsync(It.IsAny<Product>()))
                .ThrowsAsync(new Exception("Database connection error"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("Database connection error");
        }

        [Fact]
        public async Task Handle_WhenSaveChangesFails_ShouldReturnFailureResult()
        {
            // Arrange
            var command = new CreateProductCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = 1,
                Price = 99.99m,
                Sku = "SKU123"
            };

            UnitOfWorkMock.Setup(uow => uow.Products.AddAsync(It.IsAny<Product>()))
                .ReturnsAsync(new Product());

            UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync())
                .ThrowsAsync(new DbUpdateException("Constraint violation"));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("Constraint violation");
        }

        [Fact]
        public async Task Handle_ShouldGenerateValidGuid()
        {
            // Arrange
            var command = new CreateProductCommand
            {
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = 1,
                Price = 99.99m,
                Sku = "SKU123"
            };

            Product? capturedProduct = null;
            UnitOfWorkMock.Setup(uow => uow.Products.AddAsync(It.IsAny<Product>()))
                .Callback<Product>(p => capturedProduct = p)
                .ReturnsAsync((Product p) => p);

            UnitOfWorkMock.Setup(uow => uow.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            capturedProduct.Should().NotBeNull();
            Guid.TryParse(capturedProduct!.Id, out _).Should().BeTrue();
        }

        [Fact]
        public async Task Handle_MultipleValidationErrors_ShouldReturnAllErrors()
        {
            // Arrange
            var command = new CreateProductCommand
            {
                Name = "",
                Description = "Test Description",
                CategoryId = -1,
                Price = -10m,
                Sku = new string('A', 26)
            };

            var validator = new CreateProductCommandValidator();

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

    }
}

