using InventoryMan.Application.Common.DTOs;
using InventoryMan.Application.Common.Models;
using InventoryMan.Core.Interfaces;
using MediatR;

namespace InventoryMan.Application.Products.Queries.GetProductById
{
    public record GetProductByIdQuery(string Id) : IRequest<Result<ProductDto>>;

    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, Result<ProductDto>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetProductByIdQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
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
    }
}
