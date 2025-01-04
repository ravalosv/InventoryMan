using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using InventoryMan.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Server.Kestrel.Core;


var builder = WebApplication.CreateBuilder(args);


builder.WebHost.ConfigureKestrel(serverOptions =>
{
    serverOptions.ListenAnyIP(8080);  // Cambiado de 80 a 8080
    serverOptions.ListenAnyIP(8443, listenOptions =>
    {
        listenOptions.UseHttps();
    }); // Cambiado de 443 a 8443
});


// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


// Database configuration
var mySqlConnectionStr = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING") ?? builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<InventoryDbContext>(options =>
    options.UseNpgsql(mySqlConnectionStr));

// Register AutoMapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

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

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
