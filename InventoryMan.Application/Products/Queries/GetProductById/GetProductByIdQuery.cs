using AutoMapper;
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
        private readonly IMapper mapper;

        public GetProductByIdQueryHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Result<ProductDto>> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            try
            {

                // Pasar el token de cancelación al método asíncrono
                var product = await _unitOfWork.Products.GetByIdAsync(request.Id);

                if (product == null)
                    return Result<ProductDto>.Failure("Product not found");

                var productDto = mapper.Map<ProductDto>(product);

                //var productDto = new ProductDto
                //{
                //    Id = product.Id,
                //    Name = product.Name,
                //    Description = product.Description,
                //    CategoryId = product.CategoryId,
                //    CategoryName = product.Category?.Name,
                //    Price = product.Price,
                //    Sku = product.Sku
                //};

                return Result<ProductDto>.Success(productDto);
            }
            catch (Exception ex)
            {
                return Result<ProductDto>.Failure($"Error getting product: {ex.Message}");
            }
        }
    }
}
