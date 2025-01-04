using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using InventoryMan.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Polly;
using Npgsql;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Warning);


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
builder.Services.AddSwaggerGen();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Health Check endpoint
app.MapGet("/health", () => Results.Ok("Healthy"));

// CORS
app.UseCors("AllowAll");

// Error handling
app.UseExceptionHandler("/error");
app.MapGet("/error", () => Results.Problem());

app.UseAuthorization();
app.MapControllers();

app.Run();
