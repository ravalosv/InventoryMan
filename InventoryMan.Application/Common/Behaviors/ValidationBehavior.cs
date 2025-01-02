using FluentValidation;
using InventoryMan.Application.Common.Models;
using MediatR;


namespace InventoryMan.Application.Common.Behaviors
{
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(
                    TRequest request,
                    RequestHandlerDelegate<TResponse> next,
                    CancellationToken cancellationToken)
        {
            if (!_validators.Any())
                return await next();

            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(
                _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

            var failures = validationResults
                .Where(r => r.Errors.Any())
                .SelectMany(r => r.Errors)
                .ToList();

            if (failures.Any())
            {
                // Agrupamos los errores por propiedad
                var errorsByProperty = failures
                    .GroupBy(x => x.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToList()
                    );

                // Creamos un mensaje de error formateado
                var errorMessage = string.Join("\n", errorsByProperty
                    .Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value)}"));

                // Asumiendo que TResponse es Result<T>
                if (typeof(TResponse).IsGenericType &&
                    typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
                {
                    var resultType = typeof(TResponse).GetGenericArguments()[0];
                    var method = typeof(Result<>)
                        .MakeGenericType(resultType)
                        .GetMethod("Failure", new[] { typeof(string) });

                    return (TResponse)method.Invoke(null, new object[] { errorMessage });
                }

                throw new ValidationException(failures);
            }

            return await next();
        }
    }
}

