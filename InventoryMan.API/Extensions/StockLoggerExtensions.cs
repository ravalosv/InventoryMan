using InventoryMan.Application.Inventory.Commands.UpdateStock;

namespace InventoryMan.API.Extensions
{
    // InventoryMan.API/Extensions/StockLoggerExtensions.cs
    public static class StockLoggerExtensions
    {
        public static void LogStockUpdate(
            this ILogger logger,
            string stage,
            UpdateStockCommand command,
            object additionalData = null,
            Exception exception = null,
            LogLevel level = LogLevel.Information)
        {
            var logData = new
            {
                Stage = stage,
                Operation = "UpdateStock",
                ProductId = command.ProductId,
                StoreId = command.StoreId,
                NewQuantity = command.Quantity,
                AdditionalData = additionalData,
                Exception = exception != null ? new
                {
                    Message = exception.Message,
                    Type = exception.GetType().Name,
                    StackTrace = exception.StackTrace
                } : null,
                Timestamp = DateTime.UtcNow
            };

            if (exception != null)
            {
                logger.Log(level, exception, "Stock Update Operation: {Stage} {@LogData}", stage, logData);
            }
            else
            {
                logger.Log(level, "Stock Update Operation: {Stage} {@LogData}", stage, logData);
            }
        }


        public static void LogMinStockUpdate(
                        this ILogger logger,
                        string stage,
                        UpdateMinStockCommand command,
                        object additionalData = null,
                        Exception exception = null,
                        LogLevel level = LogLevel.Information)
        {
            var logData = new
            {
                Stage = stage,
                Operation = "UpdateMinStock",
                ProductId = command.ProductId,
                StoreId = command.StoreId,
                MinimumQuantity = command.MinStock,
                AdditionalData = additionalData,
                Exception = exception != null ? new
                {
                    Message = exception.Message,
                    Type = exception.GetType().Name,
                    StackTrace = exception.StackTrace
                } : null,
                Timestamp = DateTime.UtcNow
            };

            if (exception != null)
            {
                logger.Log(level, exception, "Min Stock Update Operation: {Stage} {@LogData}", stage, logData);
            }
            else
            {
                logger.Log(level, "Min Stock Update Operation: {Stage} {@LogData}", stage, logData);
            }
        }

        public static void LogStockTransfer(
                    this ILogger logger,
                    string stage,
                    TransferStockCommand command,
                    object additionalData = null,
                    Exception exception = null,
                    LogLevel level = LogLevel.Information)
        {
            var logData = new
            {
                Stage = stage,
                Operation = "StockTransfer",
                ProductId = command.ProductId,
                SourceStoreId = command.SourceStoreId,
                DestinationStoreId = command.TargetStoreId,
                Quantity = command.Quantity,
                AdditionalData = additionalData,
                Exception = exception != null ? new
                {
                    Message = exception.Message,
                    Type = exception.GetType().Name,
                    StackTrace = exception.StackTrace
                } : null,
                Timestamp = DateTime.UtcNow
            };

            if (exception != null)
            {
                logger.Log(level, exception, "Stock Transfer Operation: {Stage} {@LogData}", stage, logData);
            }
            else
            {
                logger.Log(level, "Stock Transfer Operation: {Stage} {@LogData}", stage, logData);
            }
        }


    }
}
