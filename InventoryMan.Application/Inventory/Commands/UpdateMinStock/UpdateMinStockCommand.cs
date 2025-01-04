using FluentValidation;
using InventoryMan.Application.Common;
using InventoryMan.Application.Common.Models;
using InventoryMan.Core.Interfaces;
using MediatR;

namespace InventoryMan.Application.Inventory.Commands.UpdateStock
{
    public record UpdateMinStockCommand : IRequest<Result<bool>>
    {
        public string ProductId { get; init; } = default!;
        public string StoreId { get; init; } = default!;
        public int MinStock { get; init; } = default!;
    }

    public class UpdateMinStockCommandHandler : IRequestHandler<UpdateMinStockCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateMinStockCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(UpdateMinStockCommand request, CancellationToken cancellationToken)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var inventory = await _unitOfWork.Inventories
                    .GetByStoreIdAsync(request.StoreId)
                    .ContinueWith(t => t.Result.FirstOrDefault(i => i.ProductId == request.ProductId));

                if (inventory == null)
                {
                    inventory = new Core.Entities.Inventory
                    {
                        Id = Guid.NewGuid().ToString(),
                        ProductId = request.ProductId,
                        StoreId = request.StoreId,
                        Quantity = 0,
                        MinStock = request.MinStock
                    };

                    await _unitOfWork.Inventories.AddAsync(inventory);
                }
                else
                {
                    inventory.MinStock = request.MinStock;

                    await _unitOfWork.Inventories.UpdateAsync(inventory);
                }

                await _unitOfWork.SaveChangesAsync();

                await _unitOfWork.CommitTransactionAsync();

                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return Result<bool>.Failure($"Error updating product min stock: {ex.FullMessageError()}");
            }
        }
        

        public class UpdateMinStockCommandValidator : AbstractValidator<UpdateMinStockCommand>
        {
            // Valida utilizando FluentValidation
            public UpdateMinStockCommandValidator()
            {
                RuleFor(p => p.ProductId)
                    .NotEmpty().WithMessage("ProductId is required");

                RuleFor(p => p.StoreId)
                    .NotEmpty().WithMessage("StoreId is required");

                RuleFor(p => p.MinStock)
                    .GreaterThan(0).WithMessage("MinStock must be greater than 0");

            }
        }
    }
}
