namespace InventoryMan.API.Extensions
{
    // ProductLoggerExtensions.cs
    public static class ProductLoggerExtensions
    {
        public static void LogProductOperation(
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
                logger.Log(level, exception, "Product Operation: {Operation} - {Stage} {@LogData}",
                    operation, stage, logData);
            }
            else
            {
                logger.Log(level, "Product Operation: {Operation} - {Stage} {@LogData}",
                    operation, stage, logData);
            }
        }
    }

}
