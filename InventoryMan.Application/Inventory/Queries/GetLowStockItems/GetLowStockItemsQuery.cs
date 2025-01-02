using InventoryMan.Application.Common.DTOs;
using InventoryMan.Application.Common.Models;
using InventoryMan.Application.Inventory.Queries.GetInventoryByStore;
using InventoryMan.Core.Interfaces;
using MediatR;

namespace InventoryMan.Application.Inventory.Queries.GetLowStockItems
{
    public record GetLowStockItemsQuery : IRequest<Result<IEnumerable<InventoryDto>>>;

    public class GetLowStockItemsQueryHandler : IRequestHandler<GetLowStockItemsQuery, Result<IEnumerable<InventoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public GetLowStockItemsQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<IEnumerable<InventoryDto>>> Handle(GetLowStockItemsQuery request, CancellationToken cancellationToken)
        {
            var lowStockItems = await _unitOfWork.Inventories.GetLowStockItemsAsync();

            var inventoryDtos = lowStockItems.Select(i => new InventoryDto
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
