using Softtek_Invoice_Back.Domain.Entities;

namespace Softtek_Invoice_Back.Domain.Interfaces;

/// <summary>
/// Service interface for order business logic
/// </summary>
public interface IOrderService
{
    Task<Order> CreateOrderAsync(string branchId, string itemId, int quantity, CancellationToken cancellationToken = default);
}

