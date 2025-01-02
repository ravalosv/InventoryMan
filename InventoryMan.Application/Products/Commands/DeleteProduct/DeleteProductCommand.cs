using FluentValidation;
using InventoryMan.Application.Common;
using InventoryMan.Application.Common.Models;
using InventoryMan.Core.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InventoryMan.Application.Products.Commands.DeleteProduct
{
    public record DeleteProductCommand : IRequest<Result<bool>>
    {
        public string Id { get; init; } = default!;
    }

    public class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand, Result<bool>>
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteProductCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Result<bool>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
        {
            var product = await _unitOfWork.Products.GetByIdAsync(request.Id);

            if (product == null)
                return Result<bool>.Failure($"Product with id {request.Id} not found");

            try
            {
                await _unitOfWork.Products.DeleteAsync(product);
                await _unitOfWork.SaveChangesAsync();
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error deleting product: {ex.FullMessageError()}");
            }
        }

        public class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
        {
            public DeleteProductCommandValidator()
            {
                RuleFor(p => p.Id)
                    .NotEmpty().WithMessage("Id is required");
            }
        }
    }
}
