using InventoryMan.Application.Common.DTOs;
using InventoryMan.Core.Entities;

namespace InventoryMan.Application.Common.Mappings
{
    public static class MappingExtensions
    {
        //public static ProductDto ToDto(this Product product)
        //{
        //    return new ProductDto
        //    {
        //        Id = product.Id,
        //        Name = product.Name,
        //        Description = product.Description,
        //        CategoryId = product.CategoryId,
        //        Price = product.Price,
        //        Sku = product.Sku
        //    };
        //}

        //public static InventoryDto ToDto(this Core.Entities.Inventory inventory)
        //{
        //    return new InventoryDto
        //    {
        //        Id = inventory.Id,
        //        ProductId = inventory.ProductId,
        //        ProductName = inventory.Product?.Name ?? "Unknown",
        //        StoreId = inventory.StoreId,
        //        Quantity = inventory.Quantity,
        //        MinStock = inventory.MinStock
        //    };
        //}

        //public static MovementDto ToDto(this Movement movement)
        //{
        //    return new MovementDto
        //    {
        //        Id = movement.Id,
        //        ProductId = movement.ProductId,
        //        ProductName = movement.Product?.Name ?? "Unknown",
        //        Quantity = movement.Quantity,
        //        Type = movement.Type,
        //        SourceStoreId = movement.SourceStoreId,
        //        TargetStoreId = movement.TargetStoreId,
        //        Timestamp = movement.Timestamp
        //    };
        //}
    }
}

