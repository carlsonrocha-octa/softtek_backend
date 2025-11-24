using Microsoft.EntityFrameworkCore;
using Softtek_Invoice_Back.Data;
using Softtek_Invoice_Back.Data.Repositories;
using Softtek_Invoice_Back.Domain.Interfaces;
using Softtek_Invoice_Back.Domain.Services;
using Softtek_Invoice_Back.Domain.Events;
using Softtek_Invoice_Back.Infrastructure.EventBus;
using Softtek_Invoice_Back.Infrastructure.Sap;
using FluentValidation;
using FluentValidation.AspNetCore;
using Softtek_Invoice_Back.Presentation.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Add FluentValidation
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddValidatorsFromAssemblyContaining<CreateOrderRequestValidator>();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "Portal de Pedidos de Insumos API",
        Version = "v1",
        Description = "API REST para gerenciamento de pedidos de insumos para farmácias"
    });
});

// Configure database (using SQLite for simulation)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=orders.db"));

// Alternative: Use InMemory database for testing
// builder.Services.AddDbContext<ApplicationDbContext>(options =>
//     options.UseInMemoryDatabase("OrdersDb"));

// Register repositories
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

// Register services
builder.Services.AddScoped<IOrderService, OrderService>();

// Register infrastructure
builder.Services.AddSingleton<IEventBus, InMemoryEventBus>();
builder.Services.AddScoped<ISapAdapter, SapAdapter>();
builder.Services.AddScoped<ISapApiClient, MockSapApiClient>();

// Configure CORS for frontend integration
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
    ?? new[] { "http://localhost:3000", "http://localhost:3001" };

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add health checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<ApplicationDbContext>();

var app = builder.Build();

// Configure event bus subscriptions using a hosted service approach
var eventBus = app.Services.GetRequiredService<IEventBus>();

eventBus.Subscribe<OrderCreatedEvent>(async (orderEvent, cancellationToken) =>
{
    using var scope = app.Services.CreateScope();
    var sapAdapter = scope.ServiceProvider.GetRequiredService<ISapAdapter>();
    var orderService = scope.ServiceProvider.GetRequiredService<IOrderService>();
    
    try
    {
        await orderService.UpdateOrderStatusAsync(orderEvent.OrderId, Softtek_Invoice_Back.Domain.Entities.OrderStatus.Processing, cancellationToken);
        await sapAdapter.ProcessOrderCreatedEventAsync(orderEvent, cancellationToken);
        await orderService.UpdateOrderStatusAsync(orderEvent.OrderId, Softtek_Invoice_Back.Domain.Entities.OrderStatus.SentToSap, cancellationToken);
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Error processing order {OrderId}", orderEvent.OrderId);
        await orderService.UpdateOrderStatusAsync(orderEvent.OrderId, Softtek_Invoice_Back.Domain.Entities.OrderStatus.Failed, cancellationToken);
    }
});

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    dbContext.Database.EnsureCreated();
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Portal de Pedidos API v1");
    });
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthorization();

// Map health check endpoint
app.MapHealthChecks("/health");

app.MapControllers();

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
