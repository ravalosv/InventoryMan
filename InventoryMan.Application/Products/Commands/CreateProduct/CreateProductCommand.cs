using FluentValidation;
using InventoryMan.Application.Common;
using InventoryMan.Application.Common.Models;
using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using MediatR;

namespace InventoryMan.Application.Products.Commands.CreateProduct
{
    /// <summary>
    /// Comando para crear un nuevo producto
    /// </summary>
    public record CreateProductCommand : IRequest<Result<string>>
    {
        /// <summary>
        /// Identificador único del producto
        /// </summary>
        /// <example>PROD001</example>
        public string Id { get; init; } = default!;

        /// <summary>
        /// Nombre del producto
        /// </summary>
        /// <example>Laptop HP Pavilion</example>
        public string Name { get; init; } = default!;

        /// <summary>
        /// Descripción detallada del producto
        /// </summary>
        /// <example>Laptop HP Pavilion con procesador Intel i5, 8GB RAM, 256GB SSD</example>
        public string Description { get; init; } = default!;

        /// <summary>
        /// Identificador de la categoría del producto
        /// </summary>
        /// <example>1</example>
        public int CategoryId { get; init; } = default!;

        /// <summary>
        /// Precio del producto
        /// </summary>
        /// <example>999.99</example>
        public decimal Price { get; init; }

        /// <summary>
        /// Código SKU del producto
        /// </summary>
        /// <example>SKU001</example>
        public string Sku { get; init; } = default!;
    }

    /// <summary>
    /// Manejador para el comando de creación de producto
    /// </summary>
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor del manejador
        /// </summary>
        public CreateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Procesa la creación del producto
        /// </summary>
        /// <returns>Identificador del producto creado</returns>
        public async Task<Result<string>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new Product
            {
                Id = request.Id,
                Name = request.Name,
                Description = request.Description,
                CategoryId = request.CategoryId,
                Price = request.Price,
                Sku = request.Sku
            };

            try
            {
                await _unitOfWork.Products.AddAsync(product);
                await _unitOfWork.SaveChangesAsync();
                return Result<string>.Success(product.Id);
            }
            catch (Exception ex)
            {
                return Result<string>.Failure($"Error creating product: {ex.FullMessageError()}");
            }
        }

        /// <summary>
        /// Validador para el comando de creación de producto
        /// </summary>
        public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
        {
            /// <summary>
            /// Constructor del validador con las reglas de validación
            /// </summary>
            public CreateProductCommandValidator()
            {
                RuleFor(p => p.Id)
                    .NotEmpty().WithMessage("El Id es requerido")
                    .MaximumLength(30).WithMessage("El Id no puede exceder 30 caracteres");

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
