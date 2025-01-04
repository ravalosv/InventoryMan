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
    /// <summary>
    /// Query para obtener productos con filtros y paginación
    /// </summary>
    public class GetProductsQuery : IRequest<Result<PagedList<ProductDto>>>
    {
        /// <summary>
        /// Filtro por categoría
        /// </summary>
        /// <example>Electrónicos</example>
        public string? Category { get; init; }

        /// <summary>
        /// Precio mínimo
        /// </summary>
        /// <example>100.00</example>
        public decimal? MinPrice { get; init; }

        /// <summary>
        /// Precio máximo
        /// </summary>
        /// <example>500.00</example>
        public decimal? MaxPrice { get; init; }

        /// <summary>
        /// Stock mínimo
        /// </summary>
        /// <example>5</example>
        public int? MinStock { get; init; }

        /// <summary>
        /// Stock máximo
        /// </summary>
        /// <example>100</example>
        public int? MaxStock { get; init; }

        /// <summary>
        /// Número de página
        /// </summary>
        /// <example>1</example>
        public int PageNumber { get; init; } = 1;

        /// <summary>
        /// Elementos por página
        /// </summary>
        /// <example>10</example>
        public int PageSize { get; init; } = 10;

        /// <summary>
        /// Campo de ordenamiento
        /// </summary>
        /// <example>price</example>
        public string? SortBy { get; init; }

        /// <summary>
        /// Indica si el ordenamiento es descendente
        /// </summary>
        /// <example>false</example>
        public bool SortDesc { get; init; }
    }


    /// <summary>
    /// Manejador para la consulta de productos
    /// </summary>
    public class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, Result<PagedList<ProductDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        /// <summary>
        /// Constructor del manejador
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo para operaciones de base de datos</param>
        /// <param name="mapper">Servicio de mapeo de objetos</param>
        public GetProductsQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Procesa la consulta de productos
        /// </summary>
        /// <param name="request">Query con los filtros y parámetros de paginación</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Lista paginada de productos</returns>
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

