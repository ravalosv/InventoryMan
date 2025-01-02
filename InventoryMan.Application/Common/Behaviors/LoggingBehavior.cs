using MediatR;
using Microsoft.Extensions.Logging;

namespace InventoryMan.Application.Common.Behaviors
{
    public class LoggingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly ILogger<LoggingBehavior<TRequest, TResponse>> _logger;

        public LoggingBehavior(
            ILogger<LoggingBehavior<TRequest, TResponse>> logger)
        {
            _logger = logger;
        }

        public async Task<TResponse> Handle(
            TRequest request,
            RequestHandlerDelegate<TResponse> next,
            CancellationToken cancellationToken)
        {
            var requestName = typeof(TRequest).Name;
            //var userId = _currentUserService.UserId ?? string.Empty;

            _logger.LogInformation(
                "Handling {RequestName} - Request: {@Request}",
                requestName, request);

            try
            {
                var response = await next();

                _logger.LogInformation(
                    "Handled {RequestName} - Response: {@Response}",
                    requestName, response);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error Handling {RequestName} - Error: {Error}",
                    requestName, ex.Message);
                throw;
            }
        }
    }

}
