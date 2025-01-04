using FluentValidation;
using InventoryMan.Application.Common.Behaviors;
using InventoryMan.Application.Common.Mappings;
using InventoryMan.Application.Products.Commands.CreateProduct;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace InventoryMan.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            });

            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            //services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PerformanceBehavior<,>));

            services.AddAutoMapper(typeof(AutoMapperProfiles));

            // Registro de validadores
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }

    }
}
