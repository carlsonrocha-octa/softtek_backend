using Microsoft.EntityFrameworkCore;
using Softtek_Invoice_Back.Data;
using Softtek_Invoice_Back.Data.Repositories;
using Softtek_Invoice_Back.Domain.Entities;
using Xunit;
using FluentAssertions;

namespace Softtek_Invoice_Back.Tests.Integration;

/// <summary>
/// Integration tests for OrderRepository
/// </summary>
public class OrderRepositoryIntegrationTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly OrderRepository _repository;

    public OrderRepositoryIntegrationTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: "TestDb_" + Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _repository = new OrderRepository(_context);
    }

    [Fact]
    public async Task CreateAsync_ShouldSaveOrderToDatabase()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            BranchId = "BR001",
            ItemId = "ITEM001",
            Quantity = 10,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };

        // Act
        var result = await _repository.CreateAsync(order);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(order.Id);

        var savedOrder = await _context.Orders.FindAsync(order.Id);
        savedOrder.Should().NotBeNull();
        savedOrder!.BranchId.Should().Be(order.BranchId);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOrder()
    {
        // Arrange
        var order = new Order
        {
            Id = Guid.NewGuid(),
            BranchId = "BR002",
            ItemId = "ITEM002",
            Quantity = 5,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };

        _context.Orders.Add(order);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetByIdAsync(order.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(order.Id);
        result.BranchId.Should().Be(order.BranchId);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllOrders()
    {
        // Arrange
        var order1 = new Order
        {
            Id = Guid.NewGuid(),
            BranchId = "BR003",
            ItemId = "ITEM003",
            Quantity = 3,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };

        var order2 = new Order
        {
            Id = Guid.NewGuid(),
            BranchId = "BR004",
            ItemId = "ITEM004",
            Quantity = 7,
            CreatedAt = DateTime.UtcNow.AddMinutes(1),
            Status = OrderStatus.Pending
        };

        _context.Orders.AddRange(order1, order2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _repository.GetAllAsync();

        // Assert
        result.Should().HaveCountGreaterThanOrEqualTo(2);
        result.Should().Contain(o => o.Id == order1.Id);
        result.Should().Contain(o => o.Id == order2.Id);
    }

    public void Dispose()
    {
        _context.Dispose();
    }
}

