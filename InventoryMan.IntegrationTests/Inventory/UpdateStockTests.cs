using MediatR;
using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using InventoryMan.IntegrationTests.Common;
using Microsoft.Extensions.DependencyInjection;
using InventoryMan.Application.Inventory.Commands.UpdateStock;

namespace InventoryMan.IntegrationTests.Inventory;

public class UpdateStockTests : IntegrationTestBase
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    private string testProductID = "ELEC-WIFI-001";
    private string sourceStoreId = "STR-01";
    private string targetStoreId = "STR-02";


    public UpdateStockTests()
    {
        _mediator = _serviceProvider.GetRequiredService<IMediator>();
        _unitOfWork = _serviceProvider.GetRequiredService<IUnitOfWork>();
    }

    private async Task SeedInitialData(int initialQuantity = 100)
    {

        var inventory = new Core.Entities.Inventory
        {
            Id = Guid.NewGuid().ToString(),
            ProductId = testProductID,
            StoreId = sourceStoreId,
            Quantity = initialQuantity,
            MinStock = 10
        };
        await _unitOfWork.Inventories.AddAsync(inventory);

        await _unitOfWork.SaveChangesAsync();
    }

    [Fact]
    public async Task UpdateStock_ShouldIncreaseInventory_WhenMovementTypeIsIN()
    {
        // Arrange
        await SeedInitialData();
        var command = new UpdateStockCommand
        {
            ProductId = testProductID,
            StoreId = sourceStoreId,
            Quantity = 50,
            MovementType = MovementType.IN
        };

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var inventory = await _unitOfWork.Inventories
            .GetByStoreIdAsync(sourceStoreId);
        Assert.Equal(150, inventory.First().Quantity); // 100 inicial + 50 agregados

        var movement = (await _unitOfWork.Movements.GetAllAsync()).First();
        Assert.Equal(MovementType.IN, movement.Type);
        Assert.Equal(50, movement.Quantity);
        Assert.Equal(sourceStoreId, movement.TargetStoreId);
        Assert.Null(movement.SourceStoreId);
    }

    [Fact]
    public async Task UpdateStock_ShouldDecreaseInventory_WhenMovementTypeIsOUT()
    {
        // Arrange
        await SeedInitialData();
        var command = new UpdateStockCommand
        {
            ProductId = testProductID,
            StoreId = sourceStoreId,
            Quantity = 30,
            MovementType = MovementType.OUT
        };

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var inventory = await _unitOfWork.Inventories
            .GetByStoreIdAsync(sourceStoreId);
        Assert.Equal(70, inventory.First().Quantity); // 100 inicial - 30 retirados

        var movement = (await _unitOfWork.Movements.GetAllAsync()).First();
        Assert.Equal(MovementType.OUT, movement.Type);
        Assert.Equal(30, movement.Quantity);
        Assert.Equal(sourceStoreId, movement.SourceStoreId);
        Assert.Null(movement.TargetStoreId);
    }

    [Fact]
    public async Task UpdateStock_ShouldCreateNewInventory_WhenNotExistsAndMovementTypeIsIN()
    {
        // Arrange
        await SeedInitialData(0);
        var command = new UpdateStockCommand
        {
            ProductId = testProductID,
            StoreId = targetStoreId, // Nueva tienda
            Quantity = 50,
            MovementType = MovementType.IN
        };

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.True(result.IsSuccess);

        var inventory = await _unitOfWork.Inventories
            .GetByStoreIdAsync(targetStoreId);
        Assert.Equal(50, inventory.First().Quantity);

        var movement = (await _unitOfWork.Movements.GetAllAsync()).First();
        Assert.Equal(MovementType.IN, movement.Type);
        Assert.Equal(50, movement.Quantity);
    }

    [Fact]
    public async Task UpdateStock_ShouldFail_WhenInsufficientInventoryForOUT()
    {
        // Arrange
        await SeedInitialData(50);
        var command = new UpdateStockCommand
        {
            ProductId = testProductID,
            StoreId = sourceStoreId,
            Quantity = 110, // Más que el inventario disponible
            MovementType = MovementType.OUT
        };

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("not enough inventory", result.Error);

        var inventory = await _unitOfWork.Inventories
            .GetByStoreIdAsync(sourceStoreId);
        Assert.Equal(50, inventory.First().Quantity); // El inventario no debe cambiar

        var movements = await _unitOfWork.Movements.GetAllAsync();
        Assert.Empty(movements);
    }

    [Fact]
    public async Task UpdateStock_ShouldFail_WhenMovementTypeIsOUTAndInventoryNotExists()
    {
        // Arrange
        var command = new UpdateStockCommand
        {
            ProductId = testProductID,
            StoreId = "NONEXISTENT-STORE",
            Quantity = 10,
            MovementType = MovementType.OUT
        };

        // Act
        var result = await _mediator.Send(command);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("not enough inventory", result.Error);

        var movements = await _unitOfWork.Movements.GetAllAsync();
        Assert.Empty(movements);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task UpdateStock_ShouldFail_WhenQuantityIsInvalid(int invalidQuantity)
    {
        // Arrange
        var command = new UpdateStockCommand
        {
            ProductId = testProductID,
            StoreId = sourceStoreId,
            Quantity = invalidQuantity,
            MovementType = MovementType.IN
        };

        // Act & Assert
        var result = await _mediator.Send(command);

        Assert.False(result.IsSuccess);
        Assert.Contains("Quantity must be greater than 0", result.Error);

        //await Assert.ThrowsAsync<ValidationException>(() => _mediator.Send(command));
    }
}
