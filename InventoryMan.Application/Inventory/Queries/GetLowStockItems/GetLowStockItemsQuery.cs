using MediatR;
using InventoryMan.Core.Interfaces;
using InventoryMan.Application.Common;
using InventoryMan.Application.Common.DTOs;
using InventoryMan.Application.Common.Models;

namespace InventoryMan.Application.Inventory.Queries.GetLowStockItems
{
    /// <summary>
    /// Query para obtener los items con stock bajo
    /// </summary>
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
            try
            {
                var lowStockItems = await _unitOfWork.Inventories.GetLowStockItemsAsync();

                var inventoryDtos = lowStockItems.Select(i => new InventoryDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    ProductName = i.Product?.Name,
                    StoreId = i.StoreId,
                    Quantity = i.Quantity,
                    MinStock = i.MinStock
                });

                return Result<IEnumerable<InventoryDto>>.Success(inventoryDtos);
            } catch(Exception ex)
            {
                return Result<IEnumerable<InventoryDto>>.Failure($"Error getting low stock items: {ex.FullMessageError()}");
            }
        }
    }
}
