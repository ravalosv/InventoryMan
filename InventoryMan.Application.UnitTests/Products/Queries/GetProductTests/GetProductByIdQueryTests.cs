using Moq;
using AutoMapper;
using FluentAssertions;
using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using InventoryMan.Application.Common.DTOs;
using InventoryMan.Application.Products.Queries.GetProductById;

namespace InventoryMan.Application.UnitTests.Products.Queries.GetProductTests
{

    public class GetProductByIdQueryTests
    {
        private readonly GetProductByIdQueryHandler _handler;
        private readonly Mock<IMapper> _mockMapper;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public GetProductByIdQueryTests()
        {
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _handler = new GetProductByIdQueryHandler(_unitOfWorkMock.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_ExistingProduct_ShouldReturnProductDto()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var product = new Product
            {
                Id = productId,
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = 1,
                Category = new ProductCategory { Id = 1, Name = "Test Category" },
                Price = 99.99m,
                Sku = "SKU123"
            };

            var productDtot = new ProductDto
            {
                Id = productId,
                Name = "Test Product",
                Description = "Test Description",
                CategoryId = 1,
                CategoryName = "Test Category",
                Price = 99.99m,
                Sku = "SKU123"
            };


            _mockMapper
                .Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
                .Returns(productDtot);

            _unitOfWorkMock.Setup(uow => uow.Products.GetByIdAsync(productId))
                .ReturnsAsync(product);

            var query = new GetProductByIdQuery(productId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Should().BeOfType<ProductDto>();

            var productDto = result.Data;
            productDto.Id.Should().Be(product.Id);
            productDto.Name.Should().Be(product.Name);
            productDto.Description.Should().Be(product.Description);
            productDto.CategoryId.Should().Be(product.CategoryId);
            productDto.CategoryName.Should().Be(product.Category.Name);
            productDto.Price.Should().Be(product.Price);
            productDto.Sku.Should().Be(product.Sku);

            _unitOfWorkMock.Verify(uow => uow.Products.GetByIdAsync(productId.ToLower()), Times.Once);
        }

        [Fact]
        public async Task Handle_NonExistentProduct_ShouldReturnFailure()
        {
            // Arrange
            var nonExistentId = Guid.NewGuid().ToString();
            _unitOfWorkMock.Setup(uow => uow.Products.GetByIdAsync(nonExistentId))
                .ReturnsAsync((Product)null);

            var query = new GetProductByIdQuery(nonExistentId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Product not found");
            result.Data.Should().BeNull();

            _unitOfWorkMock.Verify(uow => uow.Products.GetByIdAsync(nonExistentId), Times.Once);
        }


        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Handle_EmptyOrNullId_ShouldReturnFailure(string invalidId)
        {
            // Arrange
            var query = new GetProductByIdQuery(invalidId);

            _unitOfWorkMock.Setup(uow => uow.Products.GetByIdAsync(invalidId))
                .ReturnsAsync((Product)null);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Be("Product not found");
        }

        [Fact]
        public async Task Handle_DatabaseError_ShouldThrowException()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            _unitOfWorkMock.Setup(uow => uow.Products.GetByIdAsync(productId))
                .ThrowsAsync(new Exception("Database connection error"));

            var query = new GetProductByIdQuery(productId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Error.Should().Contain("Database connection error");

        }

        [Fact]
        public async Task Handle_ProductWithAllNullableProperties_ShouldReturnValidDto()
        {
            // Arrange
            var productId = Guid.NewGuid().ToString();
            var product = new Product
            {
                Id = productId,
                Name = null,
                Description = null,
                CategoryId = 0,
                Category = null,
                Price = 0,
                Sku = null
            };

            var productDtot = new ProductDto
            {
                Id = productId,
                Name = null,
                Description = null,
                CategoryId = 0,
                CategoryName = null,
                Price = 0,
                Sku = null
            };


            _unitOfWorkMock.Setup(uow => uow.Products.GetByIdAsync(productId))
                .ReturnsAsync(product);

            _mockMapper
                .Setup(m => m.Map<ProductDto>(It.IsAny<Product>()))
                .Returns(productDtot);


            var query = new GetProductByIdQuery(productId);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.Should().BeTrue();
            result.Data.Should().NotBeNull();
            result.Data.Id.Should().Be(productId);
            result.Data.Name.Should().BeNull();
            result.Data.Description.Should().BeNull();
            result.Data.CategoryId.Should().Be(0);
            result.Data.CategoryName.Should().BeNull();
            result.Data.Price.Should().Be(0);
            result.Data.Sku.Should().BeNull();
        }

    }


}

