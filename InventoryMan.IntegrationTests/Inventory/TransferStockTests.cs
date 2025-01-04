using MediatR;
using InventoryMan.Core.Entities;
using InventoryMan.Core.Interfaces;
using InventoryMan.IntegrationTests.Common;
using Microsoft.Extensions.DependencyInjection;
using InventoryMan.Application.Inventory.Commands.UpdateStock;
using InventoryMan.Application.Common.Models;

namespace InventoryMan.IntegrationTests.Inventory;

public class TransferStockTests : IntegrationTestBase
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;

    private string testProductID = "ELEC-WIFI-001";
    private string sourceStoreId = "STR-01";
    private string targetStoreId = "STR-02";

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

    [Fact]
    public async Task MultipleTransfers_ShouldMaintainConsistency()
    {
        // Arrange - Configuración inicial de inventarios
        var store1 = "STR-01";
        var store2 = "STR-02";
        var store3 = "STR-03";

        var products = new[] { "ELEC-WIFI-001", "ELEC-SMART-001", "SPRT-BALL-001" };

        // Crear inventarios iniciales
        var initialInventories = new List<Core.Entities.Inventory>
        {
            new() { Id = Guid.NewGuid().ToString(), ProductId = products[0], StoreId = store1, Quantity = 1000, MinStock = 10 },
            new() { Id = Guid.NewGuid().ToString(), ProductId = products[1], StoreId = store1, Quantity = 500, MinStock = 10 },
            new() { Id = Guid.NewGuid().ToString(), ProductId = products[2], StoreId = store1, Quantity = 300, MinStock = 10 },
            new() { Id = Guid.NewGuid().ToString(), ProductId = products[0], StoreId = store2, Quantity = 200, MinStock = 10 },
            new() { Id = Guid.NewGuid().ToString(), ProductId = products[1], StoreId = store2, Quantity = 150, MinStock = 10 }
        };

        // Guardar las cantidades iniciales por producto
        var initialTotals = products.ToDictionary(
            productId => productId,
            productId => initialInventories
                .Where(i => i.ProductId == productId)
                .Sum(i => i.Quantity)
        );

        foreach (var inv in initialInventories)
        {
            await _unitOfWork.Inventories.AddAsync(inv);
        }
        await _unitOfWork.SaveChangesAsync();

        // Act - Ejecutar múltiples transferencias
        var transfers = new List<TransferStockCommand>
        {
            // Transferencias exitosas
            new() { ProductId = products[0], SourceStoreId = store1, TargetStoreId = store2, Quantity = 200 },
            new() { ProductId = products[0], SourceStoreId = store2, TargetStoreId = store3, Quantity = 100 },
            new() { ProductId = products[1], SourceStoreId = store1, TargetStoreId = store2, Quantity = 150 },
        
            // Transferencias que deberían fallar
            new() { ProductId = products[0], SourceStoreId = store3, TargetStoreId = store1, Quantity = 200 }, // No hay suficiente stock
            new() { ProductId = products[2], SourceStoreId = store2, TargetStoreId = store3, Quantity = 50 },  // No existe inventario origen
            new() { ProductId = "NONEXISTENT", SourceStoreId = store1, TargetStoreId = store2, Quantity = 10 } // Producto no existe
        };

        var results = new List<Result<bool>>();
        foreach (var transfer in transfers)
        {
            results.Add(await _mediator.Send(transfer));
        }

        // Assert
        // Verificar resultados de las transferencias
        Assert.True(results[0].IsSuccess);  // Primera transferencia exitosa
        Assert.True(results[1].IsSuccess);  // Segunda transferencia exitosa
        Assert.True(results[2].IsSuccess);  // Tercera transferencia exitosa
        Assert.False(results[3].IsSuccess); // Cuarta transferencia fallida
        Assert.False(results[4].IsSuccess); // Quinta transferencia fallida
        Assert.False(results[5].IsSuccess); // Sexta transferencia fallida

        // Verificar cantidades finales en inventarios
        var finalInventories = await _unitOfWork.Inventories.GetAllAsync();

        // Store 1 validations
        var store1Prod0 = finalInventories.First(i => i.StoreId == store1 && i.ProductId == products[0]);
        Assert.Equal(800, store1Prod0.Quantity); // 1000 - 200

        var store1Prod1 = finalInventories.First(i => i.StoreId == store1 && i.ProductId == products[1]);
        Assert.Equal(350, store1Prod1.Quantity); // 500 - 150

        // Store 2 validations
        var store2Prod0 = finalInventories.First(i => i.StoreId == store2 && i.ProductId == products[0]);
        Assert.Equal(300, store2Prod0.Quantity); // 200 + 200 - 100

        var store2Prod1 = finalInventories.First(i => i.StoreId == store2 && i.ProductId == products[1]);
        Assert.Equal(300, store2Prod1.Quantity); // 150 + 150

        // Store 3 validations
        var store3Prod0 = finalInventories.First(i => i.StoreId == store3 && i.ProductId == products[0]);
        Assert.Equal(100, store3Prod0.Quantity); // 0 + 100

        // Verificar movimientos
        var movements = await _unitOfWork.Movements.GetAllAsync();
        Assert.Equal(3, movements.Count()); // Solo deben existir 3 movimientos exitosos

        // Verificar que todos los movimientos son de tipo TRANSFER
        Assert.All(movements, m => Assert.Equal(MovementType.TRANSFER, m.Type));

        // Verificar la suma total de inventario por producto
        foreach (var productId in products)
        {
            var totalInitial = initialTotals[productId];
            var totalFinal = finalInventories
                .Where(i => i.ProductId == productId)
                .Sum(i => i.Quantity);

            Assert.Equal(totalInitial, totalFinal);
        }
    }

}

