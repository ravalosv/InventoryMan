using InventoryMan.Application.Common.Models;
using InventoryMan.Application.Common;
using InventoryMan.Core.Interfaces;
using MediatR;
using InventoryMan.Application.Common.DTOs;
using AutoMapper;

namespace InventoryMan.Application.Test.Query
{
    public record TestCommand : IRequest<Result<List<TestDto>>>
    {
    }

    public class TestCommandHandler : IRequestHandler<TestCommand, Result<List<TestDto>>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper mapper;

        public TestCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        public async Task<Result<List<TestDto>>> Handle(TestCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var testDB = await _unitOfWork.Tests.GetAllAsync();

                var testData = mapper.Map<List<TestDto>>(testDB);

                return Result<List<TestDto>>.Success(testData);
            }
            catch (Exception ex)
            {
                return Result<List<TestDto>>.Failure($"Error testing: {ex.FullMessageError()}");
            }
        }


    }
}
