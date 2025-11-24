using Microsoft.EntityFrameworkCore;
using Softtek_Invoice_Back.Data;
using Softtek_Invoice_Back.Data.Repositories;
using Softtek_Invoice_Back.Domain.Interfaces;
using Softtek_Invoice_Back.Domain.Services;
using Softtek_Invoice_Back.Domain.Events;
using Softtek_Invoice_Back.Infrastructure.EventBus;
using Softtek_Invoice_Back.Infrastructure.Sap;
using FluentValidation.AspNetCore;
using Softtek_Invoice_Back.Presentation.Validators;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure database (using SQLite for simulation)
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=orders.db"));

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

// Register FluentValidation
builder.Services.AddFluentValidation(fv =>
{
    fv.RegisterValidatorsFromAssemblyContaining<CreateOrderRequestValidator>();
});

// Configure CORS for frontend integration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Configure event bus subscriptions
var eventBus = app.Services.GetRequiredService<IEventBus>();
var sapAdapter = app.Services.GetRequiredService<ISapAdapter>();

eventBus.Subscribe<OrderCreatedEvent>(async (orderEvent, cancellationToken) =>
{
    await sapAdapter.ProcessOrderCreatedEventAsync(orderEvent, cancellationToken);
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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowReactApp");

app.UseAuthorization();

app.MapControllers();

app.Run();

// Make Program class accessible for integration tests
public partial class Program { }
