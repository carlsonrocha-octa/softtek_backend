using Softtek_Invoice_Back.Domain.Entities;

namespace Softtek_Invoice_Back.Domain.Interfaces;

/// <summary>
/// Repository interface for Order entity
/// </summary>
public interface IOrderRepository
{
    Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default);
    Task<Order> UpdateAsync(Order order, CancellationToken cancellationToken = default);
    Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default);
}

