using InventoryMan.Application.Common.DTOs;
using InventoryMan.Application.Common.Models;
using InventoryMan.Core.Interfaces;
using MediatR;

namespace InventoryMan.Application.Inventory.Queries.GetInventoryByStore
{
    public record GetInventoryByStoreQuery(string StoreId) : IRequest<Result<IEnumerable<InventoryDto>>>;

    public class GetInventoryByStoreQueryHandler : IRequestHandler<GetInventoryByStoreQuery, Result<IEnumerable<InventoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetInventoryByStoreQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<InventoryDto>>> Handle(GetInventoryByStoreQuery request, CancellationToken cancellationToken)
        {
            var inventories = await _unitOfWork.Inventories.GetByStoreIdAsync(request.StoreId);

            var inventoryDtos = inventories.Select(i => new InventoryDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                ProductName = i.Product?.Name ?? "Unknown",
                StoreId = i.StoreId,
                Quantity = i.Quantity,
                MinStock = i.MinStock
            });

            return Result<IEnumerable<InventoryDto>>.Success(inventoryDtos);
        }
    }
}
