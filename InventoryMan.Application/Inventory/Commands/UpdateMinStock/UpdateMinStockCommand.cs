using FluentValidation;
using InventoryMan.Application.Common;
using InventoryMan.Application.Common.Models;
using InventoryMan.Core.Interfaces;
using MediatR;

namespace InventoryMan.Application.Inventory.Commands.UpdateStock
{
    /// <summary>
    /// Comando para actualizar el stock mínimo de un producto
    /// </summary>
    public record UpdateMinStockCommand : IRequest<Result<bool>>
    {
        /// <summary>
        /// Identificador del producto
        /// </summary>
        /// <example>PROD001</example>
        public string ProductId { get; init; } = default!;

        /// <summary>
        /// Identificador de la tienda
        /// </summary>
        /// <example>STORE001</example>
        public string StoreId { get; init; } = default!;

        /// <summary>
        /// Cantidad mínima de stock permitida
        /// </summary>
        /// <example>5</example>
        public int MinStock { get; init; } = default!;
    }


    /// <summary>
    /// Manejador para el comando de actualización de stock mínimo
    /// </summary>
    public class UpdateMinStockCommandHandler : IRequestHandler<UpdateMinStockCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor del manejador
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo para operaciones de base de datos</param>
        public UpdateMinStockCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Procesa la actualización del stock mínimo
        /// </summary>
        /// <returns>Resultado de la operación</returns>
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


        /// <summary>
        /// Validador para el comando de actualización de stock mínimo
        /// </summary>
        public class UpdateMinStockCommandValidator : AbstractValidator<UpdateMinStockCommand>
        {
            /// <summary>
            /// Constructor que define las reglas de validación
            /// </summary>
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
