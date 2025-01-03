using AutoMapper;
using InventoryMan.Application.Common.DTOs;
using InventoryMan.Application.Common.Models;
using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using LinqKit;
using MediatR;
using System.Linq.Expressions;

namespace InventoryMan.Application.Products.Queries.GetProducts
{
    public class GetProductsQuery : IRequest<Result<PagedList<ProductDto>>>
    {
        public string? Category { get; init; }
        public decimal? MinPrice { get; init; }
        public decimal? MaxPrice { get; init; }
        public int? MinStock { get; init; }
        public int? MaxStock { get; init; }
        public int PageNumber { get; init; } = 1;
        public int PageSize { get; init; } = 10;
        public string? SortBy { get; init; }
        public bool SortDesc { get; init; }
    }

    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PagedList<ProductDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;


        public GetProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<Result<PagedList<ProductDto>>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // Construir la expresión de filtrado
                var filterExpression = BuildFilterExpression(request);

                // Obtener productos paginados con filtros
                var products = await _unitOfWork.Products
                    .GetPagedAsync(
                        filter: filterExpression,
                        orderBy: query => BuildOrderByExpression(request), 
                        includeProperties: "Category,Inventories",
                        pageNumber: request.PageNumber,
                        pageSize: request.PageSize
                    );

                // Mapear a DTOs
                var productDtos = _mapper.Map<List<ProductDto>>(products);

                // Crear lista paginada de DTOs
                var pagedList = new PagedList<ProductDto>(
                    productDtos,
                    products.RowCount,
                    request.PageNumber,
                    request.PageSize
                );

                return Result<PagedList<ProductDto>>.Success(pagedList);
            }
            catch (Exception ex)
            {
                return Result<PagedList<ProductDto>>.Failure($"Error getting products: {ex.Message}");
            }
        }

        private Expression<Func<Product, bool>> BuildFilterExpression(GetProductsQuery request)
        {
            var predicate = PredicateBuilder.New<Product>(true);

            if (!string.IsNullOrEmpty(request.Category))
            {
                predicate = predicate.And(p => p.Category.Name.ToLower().Contains(request.Category.ToLower()));
            }

            if (request.MinPrice.HasValue)
            {
                predicate = predicate.And(p => p.Price >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                predicate = predicate.And(p => p.Price <= request.MaxPrice.Value);
            }

            if (request.MinStock.HasValue || request.MaxStock.HasValue)
            {
                predicate = predicate.And(p => p.Inventories.Any(i =>
                    (!request.MinStock.HasValue || i.Quantity >= request.MinStock.Value) &&
                    (!request.MaxStock.HasValue || i.Quantity <= request.MaxStock.Value)
                ));
            }

            return predicate;
        }

        private IOrderedQueryable<Product> BuildOrderByExpression(GetProductsQuery request)
        {
            return request.SortBy?.ToLower() switch
            {
                "name" => request.SortDesc ?
                    _unitOfWork.Products.Query().OrderByDescending(p => p.Name) :
                    _unitOfWork.Products.Query().OrderBy(p => p.Name),
                "price" => request.SortDesc ?
                    _unitOfWork.Products.Query().OrderByDescending(p => p.Price) :
                    _unitOfWork.Products.Query().OrderBy(p => p.Price),
                "category" => request.SortDesc ?
                    _unitOfWork.Products.Query().OrderByDescending(p => p.Category.Name) :
                    _unitOfWork.Products.Query().OrderBy(p => p.Category.Name),
                _ => _unitOfWork.Products.Query().OrderBy(p => p.Name)
            };
        }
    }
}

