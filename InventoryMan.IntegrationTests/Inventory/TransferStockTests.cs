using MediatR;
using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using InventoryMan.IntegrationTests.Common;
using Microsoft.Extensions.DependencyInjection;
using InventoryMan.Application.Inventory.Commands.UpdateStock;

namespace InventoryMan.IntegrationTests.Inventory;

public class TransferStockTests : IntegrationTestBase
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    private string testProductID = "18b43125-e3f2-435a-98f5-d70314cc8824";
    private string sourceStoreId = "859251b6-f0fc-480d-87f2-edd3de9bc817";
    private string targetStoreId = "5f7b74c4-1ff7-4c7e-b54d-54f3d382b7d1";

    public TransferStockTests()
    {
        _mediator = _serviceProvider.GetRequiredService<IMediator>();
        _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
    }

    private async Task SeedInitialData()
    {
        // Crear inventario inicial en tienda origen
        var sourceInventory = new Core.Entities.Inventory
        {
            Id = Guid.NewGuid().ToString(),
            ProductId = testProductID,
            StoreId = sourceStoreId,
            Quantity = 100,
            MinStock = 10
        };
        await _unitOfWork.Inventories.AddAsync(sourceInventory);

        await _unitOfWork.SaveChangesAsync();
    }

    [Fact]
    public async Task Transfer_ShouldSucceed_WhenEnoughInventory()
    {
        // Arrange
        await SeedInitialData();

        var command = new TransferStockCommand
        {
            ProductId = testProductID,
            SourceStoreId = sourceStoreId,
            TargetStoreId = targetStoreId,
            Quantity = 50
        };

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        // Verificar inventario origen
        var sourceInventory = await _unitOfWork.Inventories
            .GetByStoreIdAsync(sourceStoreId);
        Assert.Equal(50, sourceInventory.First().Quantity);

        // Verificar inventario destino
        var targetInventory = await _unitOfWork.Inventories
            .GetByStoreIdAsync(targetStoreId);
        Assert.Equal(50, targetInventory.First().Quantity);

        // Verificar movimiento
        var movements = await _unitOfWork.Movements.GetAllAsync();
        var movement = movements.First();
        Assert.Equal(MovementType.TRANSFER, movement.Type);
        Assert.Equal(50, movement.Quantity);
        Assert.Equal(sourceStoreId, movement.SourceStoreId);
        Assert.Equal(targetStoreId, movement.TargetStoreId);
    }

    [Fact]
    public async Task Transfer_ShouldFail_WhenInsufficientInventory()
    {
        // Arrange
        await SeedInitialData();

        var command = new TransferStockCommand
        {
            ProductId = testProductID,
            SourceStoreId = sourceStoreId,
            TargetStoreId = targetStoreId,
            Quantity = 150 // Más que el inventario disponible
        };

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("not enough inventory", result.Error);

        // Verificar que no se realizaron cambios
        var sourceInventory = await _unitOfWork.Inventories
            .GetByStoreIdAsync(sourceStoreId);
        Assert.Equal(100, sourceInventory.First().Quantity);

        var movements = await _unitOfWork.Movements.GetAllAsync();
        Assert.Empty(movements);
    }

    [Fact]
    public async Task Transfer_ShouldFail_WhenSameSourceAndTarget()
    {
        // Arrange
        await SeedInitialData();

        var command = new TransferStockCommand
        {
            ProductId = testProductID,
            SourceStoreId = sourceStoreId,
            TargetStoreId = sourceStoreId, // Misma tienda
            Quantity = 50
        };

        // Act & Assert

        var result = await _mediator.Send(command);

        Assert.False(result.IsSuccess);
        Assert.Contains("SourceStoreId and TargetStoreId must be different", result.Error);

        //await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));

        // Verificar que no se realizaron cambios
        var sourceInventory = await _unitOfWork.Inventories
            .GetByStoreIdAsync(sourceStoreId);
        Assert.Equal(100, sourceInventory.First().Quantity);

        var movements = await _unitOfWork.Movements.GetAllAsync();
        Assert.Empty(movements);
    }

    [Fact]
    public async Task Transfer_ShouldFail_WhenProductNotExists()
    {
        // Arrange
        await SeedInitialData();

        var command = new TransferStockCommand
        {
            ProductId = "NONEXISTENT-PROD",
            SourceStoreId = sourceStoreId,
            TargetStoreId = targetStoreId,
            Quantity = 50
        };

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("not enough inventory", result.Error);

        var movements = await _unitOfWork.Movements.GetAllAsync();
        Assert.Empty(movements);
    }
}

