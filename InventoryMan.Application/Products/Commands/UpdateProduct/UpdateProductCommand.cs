using FluentValidation;
using InventoryMan.Application.Common;
using InventoryMan.Application.Common.Models;
using InventoryMan.Core.Interfaces;
using MediatR;

namespace InventoryMan.Application.Products.Commands.UpdateProduct
{
    public record UpdateProductCommand : IRequest<Result<bool>>
    {
        public string Id { get; init; } = default!;
        public string Name { get; init; } = default!;
        public string Description { get; init; } = default!;
        public string Sku { get; init; } = default!;
        public int CategoryId { get; init; } = default!;
        public decimal Price { get; init; }
    }

    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public UpdateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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

        public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
        {
            // Valida utilizando FluentValidation
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
