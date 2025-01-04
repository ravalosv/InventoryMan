using FluentValidation;
using InventoryMan.Application.Common;
using InventoryMan.Application.Common.Models;
using InventoryMan.Core.Interfaces;
using MediatR;

namespace InventoryMan.Application.Products.Commands.DeleteProduct
{
    /// <summary>
    /// Comando para eliminar un producto
    /// </summary>
    public record DeleteProductCommand : IRequest<Result<bool>>
    {
        /// <summary>
        /// Identificador único del producto a eliminar
        /// </summary>
        /// <example>PROD001</example>
        public string Id { get; init; } = default!;
    }

    /// <summary>
    /// Manejador para el comando de eliminación de producto
    /// </summary>
    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor del manejador
        /// </summary>
        public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Procesa la eliminación del producto
        /// </summary>
        /// <returns>Resultado de la operación de eliminación</returns>
        public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {

            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(request.Id);

                if (product == null)
                    return Result<bool>.Failure($"Product with id {request.Id} not found");


                await _unitOfWork.Products.DeleteAsync(product);
                await _unitOfWork.SaveChangesAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error deleting product: {ex.FullMessageError()}");
            }
        }

        /// <summary>
        /// Validador para el comando de eliminación de producto
        /// </summary>
        public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
        {
            /// <summary>
            /// Constructor del validador con las reglas de validación
            /// </summary>
            public DeleteProductCommandValidator()
            {
                RuleFor(p => p.Id)
                    .NotEmpty().WithMessage("Id is required");
            }
        }
    }
}
