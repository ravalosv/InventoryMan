namespace InventoryMan.API.Extensions
{
    public static class InventoryLoggerExtensions
    {
        public static void LogInventoryOperation(
            this ILogger logger,
            string operation,
            string stage,
            object data,
            Exception exception = null,
            LogLevel level = LogLevel.Information)
        {
            var logData = new
            {
                Operation = operation,
                Stage = stage,
                Data = data,
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
                logger.Log(level, exception, "Inventory Operation: {Operation} - {Stage} {@LogData}",
                    operation, stage, logData);
            }
            else
            {
                logger.Log(level, "Inventory Operation: {Operation} - {Stage} {@LogData}",
                    operation, stage, logData);
            }
        }
    }


}
