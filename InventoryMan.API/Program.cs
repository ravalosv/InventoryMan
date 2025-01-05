using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using InventoryMan.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Polly;
using Npgsql;
using Microsoft.OpenApi.Models;
using System.Reflection;
using InventoryMan.API.Documentation;
using Microsoft.AspNetCore.Mvc.Controllers;
using Swashbuckle.AspNetCore.Filters;
using Serilog.Events;
using Serilog.Formatting.Json;
using Serilog;
using InventoryMan.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Configuración de Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)  // Lee la configuración base del appsettings.json
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithEnvironmentName()
    .WriteTo.Console(new JsonFormatter(), restrictedToMinimumLevel: LogEventLevel.Warning)  // Solo un Console
    .WriteTo.File(new JsonFormatter(),
        Path.Combine("logs", "inventory-api-.json"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 30)  // Agregamos la restricción
    .CreateLogger();

builder.Host.UseSerilog();

// Prueba de niveles de log
//Log.Information("Test Information - NO debería aparecer");
//Log.Warning("Test Warning - SÍ debería aparecer");
//Log.Error("Test Error - SÍ debería aparecer");


builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxConcurrentConnections = 1000;
    options.Limits.MaxConcurrentUpgradedConnections = 1000;
    options.Limits.MaxRequestBodySize = 10 * 1024 * 1024;
    options.Limits.MinRequestBodyDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
    options.Limits.MinResponseDataRate = new MinDataRate(bytesPerSecond: 100, gracePeriod: TimeSpan.FromSeconds(10));
});


// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = ApiDocumentation.ApiTitle,
        Version = ApiDocumentation.ApiVersion,
        Description = ApiDocumentation.ApiDescription,
        Contact = new OpenApiContact
        {
            Name = ApiDocumentation.ApiContactName,
            Email = ApiDocumentation.ApiContactEmail
        }
    });

    // Habilita las anotaciones
    c.EnableAnnotations();

    // Habilita los ejemplos
    c.ExampleFilters();

    // Configuración de agrupamiento de endpoints
    c.TagActionsBy(api =>
    {
        if (api.GroupName != null)
        {
            return new[] { api.GroupName };
        }

        if (api.ActionDescriptor is ControllerActionDescriptor controllerActionDescriptor)
        {
            return new[] { controllerActionDescriptor.ControllerName };
        }

        return new[] { "Test" };
    });

    // Ordenar las operaciones por tag
    c.OrderActionsBy(apiDesc =>
        $"{apiDesc.ActionDescriptor.RouteValues["controller"]}_{apiDesc.RelativePath}");

    // Configuración del archivo XML
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);

    // Verifica si el archivo existe
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
});

builder.Services.AddSwaggerExamplesFromAssemblyOf<Program>();


// Database configuration
var mySqlConnectionStr = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<InventoryDbContext>(options =>
{
    var connectionStringBuilder = new NpgsqlConnectionStringBuilder(mySqlConnectionStr)
    {
        MaxPoolSize = 100,
        MinPoolSize = 20,
        KeepAlive = 10,
        Timeout = 30
    };

    options.UseNpgsql(connectionStringBuilder.ConnectionString, npgsqlOptions =>
    {
        npgsqlOptions.CommandTimeout(30);
    });

    // Deshabilitar tracking por defecto para mejorar performance
    //options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
});


builder.Services.AddHttpClient("InventoryClient")
    .AddTransientHttpErrorPolicy(p =>
        p.CircuitBreakerAsync(5, TimeSpan.FromSeconds(30)))
    .SetHandlerLifetime(TimeSpan.FromMinutes(5))  // Tiempo de vida del handler
    .ConfigureHttpClient(c => c.Timeout = TimeSpan.FromSeconds(30));  // Timeout global




// Register AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder =>
        {
            builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// Register dependencies
InventoryMan.Infrastructure.DependencyInjection.AddApplication(builder.Services);
InventoryMan.Application.DependencyInjection.AddApplication(builder.Services);

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Health Check endpoint
//app.MapGet("/health", () => Results.Ok("Healthy"));

// CORS
app.UseCors("AllowAll");

// Error handling
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/get-error");
    app.MapGet("/get-error", () => Results.Problem());
}

//app.MapGet("/test-error", () => {
//    throw new Exception("Esta es una excepción de prueba");
//});

app.UseMiddleware<LogEnricherMiddleware>();

app.UseAuthorization();
app.MapControllers();

app.Run();
