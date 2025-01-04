using InventoryMan.Application.Common.DTOs;
using InventoryMan.Application.Common.Models;
using InventoryMan.Core.Interfaces;
using MediatR;

namespace InventoryMan.Application.Products.Queries.GetProductById
{
    /// <summary>
    /// Query para obtener un producto por su identificador
    /// </summary>
    /// <param name="Id">Identificador único del producto</param>
    public record GetProductByIdQuery(string Id) : IRequest<Result<ProductDto>>;

    /// <summary>
    /// Manejador para la consulta de producto por identificador
    /// </summary>
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor del manejador
        /// </summary>
        public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Procesa la consulta del producto
        /// </summary>
        /// <returns>Resultado con los detalles del producto</returns>
        public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {

                // Pasar el token de cancelación al método asíncrono
                var product = await _unitOfWork.Products.GetByIdAsync(request.Id);

                if (product == null)
                    return Result<ProductDto>.Failure("Product not found");

                var productDto = new ProductDto
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    CategoryId = product.CategoryId,
                    CategoryName = product.Category?.Name,
                    Price = product.Price,
                    Sku = product.Sku
                };

                return Result<ProductDto>.Success(productDto);
            }
            catch (Exception ex)
            {
                return Result<ProductDto>.Failure($"Error getting product: {ex.Message}");
            }
        }
    }
}
