using FluentValidation;
using InventoryMan.Application.Common;
using InventoryMan.Application.Common.Models;
using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using MediatR;

namespace InventoryMan.Application.Inventory.Commands.UpdateStock
{
    public record TransferStockCommand : IRequest<Result<bool>>
    {
        public string ProductId { get; init; } = default!;
        public string SourceStoreId { get; init; } = default!;
        public string TargetStoreId { get; init; } = default!;
        public int Quantity { get; init; }
    }

    public class TransferStockCommandHandler : IRequestHandler<TransferStockCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public TransferStockCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(TransferStockCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Obtener y validar el inventario de origen
                var inventory = await _unitOfWork.Inventories
                    .GetByStoreIdAndProductIdAsync(request.SourceStoreId, request.ProductId)
                    .ContinueWith(t => t.Result.FirstOrDefault());

                if (inventory == null || inventory.Quantity < request.Quantity)
                {
                    throw new InvalidOperationException("There is not enough inventory");
                }

                // Actualizar la cantidad en el SourceStore
                inventory.Quantity -= request.Quantity;

                await _unitOfWork.Inventories.UpdateAsync(inventory);

                // Actualizar la cantidad en el TargetStore
                var tInventory = await _unitOfWork.Inventories
                    .GetByStoreIdAndProductIdAsync(request.TargetStoreId, request.ProductId)
                    .ContinueWith(t => t.Result.FirstOrDefault());


                if (tInventory == null)
                {
                    tInventory = new Core.Entities.Inventory
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductId = request.ProductId,
                        StoreId = request.TargetStoreId,
                        Quantity = request.Quantity,
                        MinStock = 0
                    };

                    await _unitOfWork.Inventories.AddAsync(tInventory);
                }
                else
                {
                    tInventory.Quantity += request.Quantity;
                    await _unitOfWork.Inventories.UpdateAsync(tInventory);
                }

                // Generar movimiento de TRANSFER
                var movement = new Movement
                {
                    Id = Guid.NewGuid().ToString(),
                    ProductId = request.ProductId,
                    Quantity = request.Quantity,
                    Type = MovementType.TRANSFER,
                    SourceStoreId = request.SourceStoreId,
                    TargetStoreId = request.TargetStoreId,
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


        public class TransferStockCommandValidator : AbstractValidator<TransferStockCommand>
        {
            // Valida utilizando FluentValidation
            public TransferStockCommandValidator()
            {
                RuleFor(p => p.ProductId)
                    .NotEmpty().WithMessage("ProductId is required");

                RuleFor(p => p.SourceStoreId)
                    .NotEmpty().WithMessage("SourceStoreId is required");

                RuleFor(p => p.TargetStoreId)
                    .NotEmpty().WithMessage("TargetStoreId is required");

                RuleFor(p => p)
                    .Must(p => p.SourceStoreId != p.TargetStoreId)
                    .WithMessage("SourceStoreId and TargetStoreId must be different");

                RuleFor(p => p.Quantity)
                    .GreaterThan(0).WithMessage("Quantity must be greater than 0");
            }
        }
    }
}
