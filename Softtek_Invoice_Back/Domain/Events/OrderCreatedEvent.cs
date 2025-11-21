namespace Softtek_Invoice_Back.Domain.Events;

/// <summary>
/// Event raised when a new order is created
/// </summary>
public class OrderCreatedEvent
{
    public Guid OrderId { get; set; }
    public string BranchId { get; set; } = string.Empty;
    public string ItemId { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
}

