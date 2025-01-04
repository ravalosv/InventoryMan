using FluentValidation;
using InventoryMan.Application.Common;
using InventoryMan.Application.Common.Models;
using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using MediatR;

namespace InventoryMan.Application.Inventory.Commands.UpdateStock
{
    /// <summary>
    /// Comando para actualizar el stock de un producto
    /// </summary>
    public record UpdateStockCommand : IRequest<Result<bool>>
    {
        /// <summary>
        /// Identificador del producto a actualizar
        /// </summary>
        public string ProductId { get; init; } = default!;

        /// <summary>
        /// Identificador de la tienda donde se actualiza el stock
        /// </summary>
        public string StoreId { get; init; } = default!;

        /// <summary>
        /// Cantidad a modificar en el inventario (debe ser mayor que 0)
        /// </summary>
        public int Quantity { get; init; }

        /// <summary>
        /// Tipo de movimiento (IN para entradas, OUT para salidas)
        /// </summary>
        public MovementType MovementType { get; init; } = default!;
    }


    public class UpdateStockCommandHandler : IRequestHandler<UpdateStockCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor del manejador de actualización de stock
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo para operaciones de base de datos</param>
        public UpdateStockCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Procesa la actualización del stock
        /// </summary>
        /// <param name="request">Datos del movimiento de stock</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Resultado de la operación</returns>
        public async Task<Result<bool>> Handle(UpdateStockCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {

                var sInventory = await _unitOfWork.Inventories
                    .GetByStoreIdAsync(request.StoreId)
                    .ContinueWith(t => t.Result.FirstOrDefault(i => i.ProductId == request.ProductId));

                // Si el tipo de movimiento es OUT, la cantidad debe restar el inventario
                var inventoryQuantity = request.Quantity;
                if (request.MovementType == MovementType.OUT)
                {
                    inventoryQuantity = request.Quantity * -1;
                }

                if (sInventory == null)
                {
                    // Si no existe inventario actualmente
                    // Validar que el tipo de movimiento no sea OUT
                    if (request.MovementType == MovementType.OUT || inventoryQuantity <= 0)
                    {
                        throw new Exception("There is not enough inventory");
                    }

                    sInventory = new Core.Entities.Inventory
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductId = request.ProductId,
                        StoreId = request.StoreId,
                        Quantity = request.Quantity,
                        MinStock = 0
                    };

                    await _unitOfWork.Inventories.AddAsync(sInventory);
                }
                else
                {

                    // Validar que no se esté sacando mas mercancia de la que hay en inventario
                    if (request.MovementType == MovementType.OUT && sInventory.Quantity < request.Quantity)
                    {
                        throw new Exception("There is not enough inventory");
                    }

                    sInventory.Quantity += inventoryQuantity;



                    await _unitOfWork.Inventories.UpdateAsync(sInventory);
                }
                var movement = new Movement
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Type = request.MovementType,
                    SourceStoreId = inventoryQuantity < 0 ? request.StoreId : null,
                    TargetStoreId = inventoryQuantity > 0 ? request.StoreId : null,
                    Timestamp = DateTime.UtcNow
                };

                await _unitOfWork.Movements.AddAsync(movement);
                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<bool>.Failure($"Error updating product stock: {ex.FullMessageError()}");
            }
        }

        /// <summary>
        /// Validador para el comando de actualización de stock
        /// </summary>
        public class UpdateStockCommandValidator : AbstractValidator<UpdateStockCommand>
        {
            /// <summary>
            /// Constructor que define las reglas de validación
            /// </summary>
            public UpdateStockCommandValidator()
            {
                RuleFor(p => p.ProductId)
                    .NotEmpty().WithMessage("ProductId is required");

                RuleFor(p => p.StoreId)
                    .NotEmpty().WithMessage("StoreId is required");

                RuleFor(p => p.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than 0");

                RuleFor(p => p.MovementType)
                    .Must(mt => mt == MovementType.IN || mt == MovementType.OUT)
                    .WithMessage("MovementType must be IN or OUT");
            }
        }
    }
}
