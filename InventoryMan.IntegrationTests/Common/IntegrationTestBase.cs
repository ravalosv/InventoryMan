using MediatR;
using FluentValidation;
using InventoryMan.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using InventoryMan.Infrastructure.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using InventoryMan.Infrastructure.Data.Context;
using InventoryMan.Application.Common.Behaviors;
using InventoryMan.Application.Inventory.Commands.UpdateStock;
using Npgsql;

namespace InventoryMan.IntegrationTests.Common
{
    public abstract class IntegrationTestBase : IDisposable
    {
        protected readonly IServiceProvider _serviceProvider;
        protected readonly InventoryDbContext _dbContext;
        protected readonly string _dbName;


        protected IntegrationTestBase()
        {
            var services = new ServiceCollection();

            // Configuración
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.Testing.json", optional: false)
                .Build();

            // Cambiar a esta línea para crear una base de datos única por cada test
            // Se debe tambien revisar el archivo AssemblyInfo.cs en la carpeta raíz del proyecto
            //_dbName = $"inventoryManTest_{Guid.NewGuid()}";

            _dbName = "inventoryManTest";


            var connectionString = configuration.GetConnectionString("TestDatabase");
            var builder = new NpgsqlConnectionStringBuilder(connectionString)
            {
                Database = _dbName
            };

            // Configurar DbContext
            services.AddDbContext<InventoryDbContext>(options =>
                options.UseNpgsql(builder.ConnectionString));

            // Registrar servicios
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(TransferStockCommand).Assembly));

            // Registrar validadores
            services.AddValidatorsFromAssembly(typeof(TransferStockCommand).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            _serviceProvider = services.BuildServiceProvider();
            _dbContext = _serviceProvider.GetRequiredService<InventoryDbContext>();

            _dbContext.Database.EnsureDeleted();
            _dbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            _dbContext.Database.EnsureDeleted();
            _dbContext.Dispose();
        }
    }

}
