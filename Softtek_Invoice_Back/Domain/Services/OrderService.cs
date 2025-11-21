using Softtek_Invoice_Back.Domain.Entities;
using Softtek_Invoice_Back.Domain.Events;
using Softtek_Invoice_Back.Domain.Interfaces;

namespace Softtek_Invoice_Back.Domain.Services;

/// <summary>
/// Service implementation for order business logic
/// </summary>
public class OrderService : IOrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IEventBus _eventBus;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository orderRepository,
        IEventBus eventBus,
        ILogger<OrderService> logger)
    {
        _orderRepository = orderRepository;
        _eventBus = eventBus;
        _logger = logger;
    }

    public async Task<Order> CreateOrderAsync(
        string branchId,
        string itemId,
        int quantity,
        CancellationToken cancellationToken = default)
    {
        _logger.LogInformation(
            "Creating order - BranchId: {BranchId}, ItemId: {ItemId}, Quantity: {Quantity}",
            branchId,
            itemId,
            quantity);

        var order = new Order
        {
            Id = Guid.NewGuid(),
            BranchId = branchId,
            ItemId = itemId,
            Quantity = quantity,
            CreatedAt = DateTime.UtcNow,
            Status = OrderStatus.Pending
        };

        var createdOrder = await _orderRepository.CreateAsync(order, cancellationToken);

        var orderCreatedEvent = new OrderCreatedEvent
        {
            OrderId = createdOrder.Id,
            BranchId = createdOrder.BranchId,
            ItemId = createdOrder.ItemId,
            Quantity = createdOrder.Quantity,
            CreatedAt = createdOrder.CreatedAt
        };

        await _eventBus.PublishAsync(orderCreatedEvent, cancellationToken);

        _logger.LogInformation("Order {OrderId} created and event published", createdOrder.Id);

        return createdOrder;
    }
}

