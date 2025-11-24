using Moq;
using Microsoft.Extensions.Logging;
using Softtek_Invoice_Back.Domain.Events;
using Softtek_Invoice_Back.Domain.Interfaces;
using Softtek_Invoice_Back.Infrastructure.Sap;
using Xunit;
using FluentAssertions;

namespace Softtek_Invoice_Back.Tests.Unit;

/// <summary>
/// Unit tests for SapAdapter
/// </summary>
public class SapAdapterTests
{
    private readonly Mock<ILogger<SapAdapter>> _loggerMock;
    private readonly Mock<ISapApiClient> _sapApiClientMock;
    private readonly SapAdapter _sapAdapter;

    public SapAdapterTests()
    {
        _loggerMock = new Mock<ILogger<SapAdapter>>();
        _sapApiClientMock = new Mock<ISapApiClient>();
        _sapAdapter = new SapAdapter(_loggerMock.Object, _sapApiClientMock.Object);
    }

    [Fact]
    public async Task ProcessOrderCreatedEventAsync_ShouldCallSapApiClient()
    {
        // Arrange
        var orderEvent = new OrderCreatedEvent
        {
            OrderId = Guid.NewGuid(),
            BranchId = "BR001",
            ItemId = "ITEM001",
            Quantity = 10,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        await _sapAdapter.ProcessOrderCreatedEventAsync(orderEvent);

        // Assert
        _sapApiClientMock.Verify(
            x => x.SendOrderToSapAsync(
                orderEvent.OrderId,
                orderEvent.BranchId,
                orderEvent.ItemId,
                orderEvent.Quantity,
                It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ProcessOrderCreatedEventAsync_ShouldPropagateException()
    {
        // Arrange
        var orderEvent = new OrderCreatedEvent
        {
            OrderId = Guid.NewGuid(),
            BranchId = "BR001",
            ItemId = "ITEM001",
            Quantity = 10,
            CreatedAt = DateTime.UtcNow
        };

        _sapApiClientMock
            .Setup(x => x.SendOrderToSapAsync(
                It.IsAny<Guid>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new Exception("SAP API Error"));

        // Act & Assert
        await Assert.ThrowsAsync<Exception>(
            () => _sapAdapter.ProcessOrderCreatedEventAsync(orderEvent));
    }
}

