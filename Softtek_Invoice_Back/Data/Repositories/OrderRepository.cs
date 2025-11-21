using Microsoft.EntityFrameworkCore;
using Softtek_Invoice_Back.Domain.Entities;
using Softtek_Invoice_Back.Domain.Interfaces;

namespace Softtek_Invoice_Back.Data.Repositories;

/// <summary>
/// Repository implementation for Order entity
/// </summary>
public class OrderRepository : IOrderRepository
{
    private readonly ApplicationDbContext _context;

    public OrderRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .FirstOrDefaultAsync(o => o.Id == id, cancellationToken);
    }

    public async Task<Order> CreateAsync(Order order, CancellationToken cancellationToken = default)
    {
        _context.Orders.Add(order);
        await _context.SaveChangesAsync(cancellationToken);
        return order;
    }

    public async Task<IEnumerable<Order>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Orders
            .OrderByDescending(o => o.CreatedAt)
            .ToListAsync(cancellationToken);
    }
}

