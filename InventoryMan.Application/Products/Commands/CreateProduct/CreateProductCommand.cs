using FluentValidation;
using InventoryMan.Application.Common;
using InventoryMan.Application.Common.Models;
using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using MediatR;

namespace InventoryMan.Application.Products.Commands.CreateProduct
{
    public record CreateProductCommand : IRequest<Result<string>>
    {
        public string Id { get; init; } = default!;
        public string Name { get; init; } = default!;
        public string Description { get; init; } = default!;
        public int CategoryId { get; init; } = default!;
        public decimal Price { get; init; }
        public string Sku { get; init; } = default!;
    }

    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<string>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

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

        public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
        {
            // Valida utilizando FluentValidation
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
