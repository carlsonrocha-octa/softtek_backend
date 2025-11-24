using Moq;
using Microsoft.Extensions.Logging;
using Softtek_Invoice_Back.Domain.Entities;
using Softtek_Invoice_Back.Domain.Events;
using Softtek_Invoice_Back.Domain.Interfaces;
using Softtek_Invoice_Back.Domain.Services;
using Xunit;
using FluentAssertions;

namespace Softtek_Invoice_Back.Tests.Unit;

/// <summary>
/// Unit tests for OrderService
/// </summary>
public class OrderServiceTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<IEventBus> _eventBusMock;
    private readonly Mock<ILogger<OrderService>> _loggerMock;
    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _eventBusMock = new Mock<IEventBus>();
        _loggerMock = new Mock<ILogger<OrderService>>();
        _orderService = new OrderService(
            _orderRepositoryMock.Object,
            _eventBusMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldCreateOrderAndPublishEvent()
    {
        // Arrange
        var branchId = "BR001";
        var itemId = "ITEM001";
        var quantity = 10;

        var createdOrder = new Order
        {
            Id = Guid.NewGuid(),
            BranchId = branchId,
            ItemId = itemId,
            Quantity = quantity,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };

        _orderRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdOrder);

        // Act
        var result = await _orderService.CreateOrderAsync(branchId, itemId, quantity);

        // Assert
        result.Should().NotBeNull();
        result.BranchId.Should().Be(branchId);
        result.ItemId.Should().Be(itemId);
        result.Quantity.Should().Be(quantity);
        result.Status.Should().Be(OrderStatus.Pending);

        _orderRepositoryMock.Verify(
            x => x.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _eventBusMock.Verify(
            x => x.PublishAsync(
                It.Is<OrderCreatedEvent>(e =>
                    e.OrderId == result.Id &&
                    e.BranchId == branchId &&
                    e.ItemId == itemId &&
                    e.Quantity == quantity),
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateOrderAsync_ShouldSetCorrectOrderProperties()
    {
        // Arrange
        var branchId = "BR002";
        var itemId = "ITEM002";
        var quantity = 5;

        Order? capturedOrder = null;
        _orderRepositoryMock
            .Setup(x => x.CreateAsync(It.IsAny<Order>(), It.IsAny<CancellationToken>()))
            .Callback<Order, CancellationToken>((order, ct) => capturedOrder = order)
            .ReturnsAsync((Order order, CancellationToken ct) => order);

        // Act
        await _orderService.CreateOrderAsync(branchId, itemId, quantity);

        // Assert
        capturedOrder.Should().NotBeNull();
        capturedOrder!.Id.Should().NotBeEmpty();
        capturedOrder.BranchId.Should().Be(branchId);
        capturedOrder.ItemId.Should().Be(itemId);
        capturedOrder.Quantity.Should().Be(quantity);
        capturedOrder.Status.Should().Be(OrderStatus.Pending);
        capturedOrder.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }
}

