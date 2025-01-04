using FluentValidation;
using InventoryMan.Application.Common;
using InventoryMan.Application.Common.Models;
using InventoryMan.Core.Interfaces;
using MediatR;

namespace InventoryMan.Application.Products.Commands.UpdateProduct
{
    /// <summary>
    /// Comando para actualizar un producto existente
    /// </summary>
    public record UpdateProductCommand : IRequest<Result<bool>>
    {
        /// <summary>
        /// Identificador único del producto
        /// </summary>
        /// <example>PROD001</example>
        public string Id { get; init; } = default!;

        /// <summary>
        /// Nombre actualizado del producto
        /// </summary>
        /// <example>Laptop HP Pavilion 2024</example>
        public string Name { get; init; } = default!;

        /// <summary>
        /// Nueva descripción del producto
        /// </summary>
        /// <example>Laptop HP Pavilion actualizada con procesador Intel i7, 16GB RAM, 512GB SSD</example>
        public string Description { get; init; } = default!;

        /// <summary>
        /// Nuevo código SKU del producto
        /// </summary>
        /// <example>SKU001-2024</example>
        public string Sku { get; init; } = default!;

        /// <summary>
        /// Nueva categoría del producto
        /// </summary>
        /// <example>1</example>
        public int CategoryId { get; init; } = default!;

        /// <summary>
        /// Nuevo precio del producto
        /// </summary>
        /// <example>1299.99</example>
        public decimal Price { get; init; }
    }

    /// <summary>
    /// Manejador para el comando de actualización de producto
    /// </summary>
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor del manejador
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo para operaciones de base de datos</param>
        public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Procesa la actualización del producto
        /// </summary>
        /// <param name="request">Comando con los datos actualizados del producto</param>
        /// <param name="cancellationToken">Token de cancelación</param>
        /// <returns>Resultado de la operación de actualización</returns>
        public async Task<Result<bool>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(request.Id);

                if (product == null)
                    return Result<bool>.Failure("Product not found");

                product.Name = request.Name;
                product.Description = request.Description;
                product.CategoryId = request.CategoryId;
                product.Sku = request.Sku;
                product.Price = request.Price;

                await _unitOfWork.Products.UpdateAsync(product);
                await _unitOfWork.SaveChangesAsync();

                return Result<bool>.Success(true);
            } catch (Exception ex)
            {
                return Result<bool>.Failure($"Error updating product: {ex.FullMessageError()}");
            }
        }

        /// <summary>
        /// Validador para el comando de actualización de producto
        /// </summary>
        public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
        {
            /// <summary>
            /// Constructor del validador con las reglas de validación
            /// </summary>
            public UpdateProductCommandValidator()
            {
                RuleFor(p => p.Name)
                    .NotEmpty().WithMessage("El nombre es requerido")
                    .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres");

                RuleFor(p => p.Price)
                    .GreaterThan(0).WithMessage("El precio debe ser mayor a 0");

                RuleFor(p => p.Sku)
                    .NotEmpty().WithMessage("El SKU es requerido")
                    .MaximumLength(25).WithMessage("El SKU no puede exceder 25 caracteres");

                RuleFor(p => p.CategoryId)
                    .GreaterThan(0).WithMessage("Debe especificar una categoría válida");
            }
        }
    }
}
