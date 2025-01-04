using InventoryMan.Application.Common;
using InventoryMan.Application.Common.DTOs;
using InventoryMan.Application.Common.Models;
using InventoryMan.Core.Interfaces;
using MediatR;

namespace InventoryMan.Application.Inventory.Queries.GetInventoryByStore
{
    /// <summary>
    /// Query para obtener el inventario de una tienda
    /// </summary>
    /// <param name="StoreId">Identificador de la tienda a consultar</param>
    public record GetInventoryByStoreQuery(string StoreId) : IRequest<Result<IEnumerable<InventoryDto>>>;

    /// <summary>
    /// Manejador para la consulta de inventario por tienda
    /// </summary>
    public class GetInventoryByStoreQueryHandler : IRequestHandler<GetInventoryByStoreQuery, Result<IEnumerable<InventoryDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor del manejador
        /// </summary>
        public GetInventoryByStoreQueryHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Procesa la consulta de inventario
        /// </summary>
        /// <returns>Resultado que contiene la lista de productos en inventario o un mensaje de error</returns>
        /// <remarks>
        /// El método realiza las siguientes validaciones:
        /// 1. Verifica que la tienda exista
        /// 2. Obtiene el inventario asociado a la tienda
        /// 3. Mapea los resultados al DTO correspondiente
        /// </remarks>
        public async Task<Result<IEnumerable<InventoryDto>>> Handle(GetInventoryByStoreQuery request, CancellationToken cancellationToken)
        {
            try
            {
                // validar que la tienda exista
                var store = await _unitOfWork.Stores.GetByIdAsync(request.StoreId);
                if (store == null)
                    return Result<IEnumerable<InventoryDto>>.Failure($"Store with ID {request.StoreId} not found");

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
            } catch(Exception ex)
            {
                return Result<IEnumerable<InventoryDto>>.Failure(ex.FullMessageError());

            }
        }
    }
}
