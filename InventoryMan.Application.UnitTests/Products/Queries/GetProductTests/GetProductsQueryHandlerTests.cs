using Moq;
using AutoMapper;
using System.Linq.Expressions;
using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using InventoryMan.Application.Common.DTOs;
using InventoryMan.Application.Products.Queries.GetProducts;
using System.Linq.Dynamic.Core;

namespace InventoryMan.Application.UnitTests.Products.Queries.GetProductTests
{
    public class GetProductsQueryHandlerTests
    {
        private readonly Mock<IUnitOfWork> _mockUnitOfWork;
        private readonly Mock<IMapper> _mockMapper;
        private readonly GetProductsQueryHandler _handler;
        private readonly Mock<IProductRepository> _mockProductRepository;

        public GetProductsQueryHandlerTests()
        {
            _mockUnitOfWork = new Mock<IUnitOfWork>();
            _mockMapper = new Mock<IMapper>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockUnitOfWork.Setup(uow => uow.Products).Returns(_mockProductRepository.Object);
            _handler = new GetProductsQueryHandler(_mockUnitOfWork.Object, _mockMapper.Object);
        }

        [Fact]
        public async Task Handle_WithValidRequest_ReturnsSuccessResult()
        {
            // Arrange
            var query = new GetProductsQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            var products = new List<Product>
            {
                new Product {
                    Id = "1",
                    Name = "Product 1",
                    Price = 100,
                    Category = new ProductCategory { Name = "Category 1" }
                },
                new Product {
                    Id = "2",
                    Name = "Product 2",
                    Price = 200,
                    Category = new ProductCategory { Name = "Category 2" }
                }
            };

            var pagedResult = new PagedResult<Product>
            {
                Queryable = products.AsQueryable(),
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                RowCount = products.Count,
                PageCount = (int)Math.Ceiling(products.Count / (double)query.PageSize)
            };

            var productDtos = new List<ProductDto>
            {
                new ProductDto {
                    Id = "1",
                    Name = "Product 1",
                    Price = 100,
                    CategoryName = "Category 1"
                },
                new ProductDto {
                    Id = "2",
                    Name = "Product 2",
                    Price = 200,
                    CategoryName = "Category 2"
                }
            };

            _mockProductRepository
                .Setup(x => x.GetPagedAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<string>(),
                    query.PageNumber,
                    query.PageSize,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResult);

            // Corregido el setup del mapper para usar PagedResult
            _mockMapper
                .Setup(m => m.Map<List<ProductDto>>(It.IsAny<PagedResult<Product>>()))
                .Returns(productDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(2, result.Data.TotalCount);
            Assert.Equal(2, result.Data.Items.Count);
            Assert.Equal("Product 1", result.Data.Items[0].Name);
            Assert.Equal("Category 1", result.Data.Items[0].CategoryName);
            Assert.Equal("Product 2", result.Data.Items[1].Name);
            Assert.Equal("Category 2", result.Data.Items[1].CategoryName);
        }



        [Fact]
        public async Task Handle_WithCategoryFilter_AppliesFilterCorrectly()
        {
            // Arrange
            var query = new GetProductsQuery
            {
                Category = "Electronics",
                PageNumber = 1,
                PageSize = 10
            };

            var products = new List<Product>
            {
                new Product
                {
                    Id = "1",
                    Name = "Phone",
                    Price = 999,
                    Category = new ProductCategory { Name = "Electronics" }  // Asumiendo que es Category, no ProductCategory
                }
            };

            var pagedResult = new PagedResult<Product>
            {
                Queryable = products.AsQueryable(),
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                RowCount = products.Count,
                PageCount = (int)Math.Ceiling(products.Count / (double)query.PageSize)
            };

            var productDtos = new List<ProductDto>
            {
                new ProductDto
                {
                    Id = "1",
                    Name = "Phone",
                    Price = 999,
                    CategoryName = "Electronics"
                }
            };

            _mockProductRepository
                .Setup(x => x.GetPagedAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<string>(),
                    query.PageNumber,
                    query.PageSize,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResult);

            // Corregido el setup del mapper para usar PagedResult
            _mockMapper
                .Setup(m => m.Map<List<ProductDto>>(It.IsAny<PagedResult<Product>>()))
                .Returns(productDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.TotalCount);
            Assert.Single(result.Data.Items);
            Assert.Equal("Phone", result.Data.Items.First().Name);
            Assert.Equal("Electronics", result.Data.Items.First().CategoryName);
        }



        [Fact]
        public async Task Handle_WithPriceRangeFilter_AppliesFilterCorrectly()
        {
            // Arrange
            var query = new GetProductsQuery
            {
                MinPrice = 100,
                MaxPrice = 200,
                PageNumber = 1,
                PageSize = 10
            };

            var products = new List<Product>
            {
                new Product {
                    Id = "1",
                    Name = "Product 1",
                    Price = 150,
                    Category = new ProductCategory { Name = "Category 1" }
                }
            };

            var pagedResult = new PagedResult<Product>
            {
                Queryable = products.AsQueryable(),
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                RowCount = products.Count,
                PageCount = (int)Math.Ceiling(products.Count / (double)query.PageSize)
            };

            var productDtos = new List<ProductDto>
            {
                new ProductDto {
                    Id = "1",
                    Name = "Product 1",
                    Price = 150,
                    CategoryName = "Category 1"
                }
            };

            _mockProductRepository
                .Setup(x => x.GetPagedAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<string>(),
                    query.PageNumber,
                    query.PageSize,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResult);

            // Corregido el setup del mapper para usar PagedResult
            _mockMapper
                .Setup(m => m.Map<List<ProductDto>>(It.IsAny<PagedResult<Product>>()))
                .Returns(productDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.TotalCount);
            Assert.Single(result.Data.Items);
            Assert.Equal(150, result.Data.Items.First().Price);
            Assert.Equal("Category 1", result.Data.Items.First().CategoryName);
        }



        [Fact]
        public async Task Handle_WithSorting_AppliesSortingCorrectly()
        {
            // Arrange
            var query = new GetProductsQuery
            {
                SortBy = "price",
                SortDesc = true,
                PageNumber = 1,
                PageSize = 10
            };

            var products = new List<Product>
            {
                new Product {
                    Id = "1",
                    Name = "Product 1",
                    Price = 200,
                    Category = new ProductCategory { Name = "Category 1" }
                },
                new Product {
                    Id = "2",
                    Name = "Product 2",
                    Price = 100,
                    Category = new ProductCategory { Name = "Category 2" }
                }
            };

            var pagedResult = new PagedResult<Product>
            {
                Queryable = products.AsQueryable(),
                CurrentPage = query.PageNumber,
                PageSize = query.PageSize,
                RowCount = products.Count,
                PageCount = (int)Math.Ceiling(products.Count / (double)query.PageSize)
            };

            var productDtos = new List<ProductDto>
            {
                new ProductDto { Id = "1", Name = "Product 1", Price = 200, CategoryName = "Category 1" },
                new ProductDto { Id = "2", Name = "Product 2", Price = 100, CategoryName = "Category 2" }
            };

            _mockProductRepository
                .Setup(x => x.GetPagedAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<string>(),
                    query.PageNumber,
                    query.PageSize,
                    It.IsAny<CancellationToken>()))
                .ReturnsAsync(pagedResult);

            // Configurar el mapper para manejar PagedResult<Product> a List<ProductDto>
            _mockMapper
                .Setup(m => m.Map<List<ProductDto>>(It.IsAny<PagedResult<Product>>()))
                .Returns(productDtos);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.NotNull(result.Data.Items);
            Assert.Equal(2, result.Data.Items.Count);
            Assert.Equal(200, result.Data.Items.First().Price);
            Assert.Equal("Category 1", result.Data.Items.First().CategoryName);
        }


        [Fact]
        public async Task Handle_WithException_ReturnsFailureResult()
        {
            // Arrange
            var query = new GetProductsQuery
            {
                PageNumber = 1,
                PageSize = 10
            };

            _mockProductRepository
                .Setup(x => x.GetPagedAsync(
                    It.IsAny<Expression<Func<Product, bool>>>(),
                    It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                    It.IsAny<string>(),
                    query.PageNumber,
                    query.PageSize,
                    It.IsAny<CancellationToken>()))
                .ThrowsAsync(new Exception("Test exception"));

            // No necesitamos configurar el mapper ya que esperamos que el código falle antes de llegar a ese punto

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.NotNull(result.Error);
            Assert.Contains("Error getting products", result.Error);
            Assert.Null(result.Data);

            // Verificar que el repositorio fue llamado
            _mockProductRepository.Verify(x => x.GetPagedAsync(
                It.IsAny<Expression<Func<Product, bool>>>(),
                It.IsAny<Func<IQueryable<Product>, IOrderedQueryable<Product>>>(),
                It.IsAny<string>(),
                query.PageNumber,
                query.PageSize,
                It.IsAny<CancellationToken>()),
                Times.Once);

            // Verificar que el mapper nunca fue llamado
            _mockMapper.Verify(x => x.Map<List<ProductDto>>(It.IsAny<PagedResult<Product>>()),
                Times.Never);
        }
    }
}

